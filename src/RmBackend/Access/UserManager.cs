using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using RmBackend.Models;
using RmBackend.Utilities;
using Microsoft.EntityFrameworkCore;

namespace RmBackend.Access
{
    public static class UserManager
    {
        /*
         * userLogin -> 0: not logged in; 1: not admin; 2: admin
         */
        private const string UserLoginKey = "userLogin";
        private const string UserEntityKey = "userEntity";

        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private static object _rndLock = new object();
        private static Random _rnd = new Random();
        private static object _loginHashLock = new object();
        private static List<string> _acceptedHashes = new List<string>();
        private static object _loginEntryLock = new object();
        private static List<Tuple<DateTime, string, string>> _thirdPartyLogins = new List<Tuple<DateTime, string, string>>();
        private static object _userCreationLock = new object();
        private static Dictionary<string, EventWaitHandle> _creatingUser = new Dictionary<string, EventWaitHandle>();

        private static User DeserializeUser(string str)
        {
            var serializer = new XmlSerializer(typeof(User));
            return serializer.Deserialize(new StringReader(str)) as User;
        }

        private static string SerializeUser(User user)
        {
            var serializer = new XmlSerializer(typeof(User));
            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), user);

            return sb.ToString();
        }

        private static void AssignUser(ISession session, User user)
        {
            session.SetInt32(UserLoginKey, user.IsAdmin ? 2 : 1);
            session.SetString(UserEntityKey, SerializeUser(user));
        }

        private static string HashFromPwdHash(string pwdhash, RmLoginSettings settings)
        {
            return CryptoHelper.GetMd5String(
                CryptoHelper.GetMd5String(pwdhash + settings.HashSalt) + settings.HashSalt
            );
        }

        /// <summary>
        /// Create login for user. Throws exceptions if invalid, username exists or fails
        /// </summary>
        public static void CreateLoginForUser(User user, string name, string pwdhash, RmLoginSettings settings, RmContext context)
        {
            if (user == null || String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(pwdhash))
                throw new ArgumentNullException();

            var existing = context.UserLogins.FirstOrDefault(u => u.Name == name);
            if (existing != null)
                throw new Exception("username exists");

            var hash = HashFromPwdHash(pwdhash, settings);
            var login = new UserLogin
            {
                Name = name,
                Hash = hash,
                UserId = user.UserId
            };

            try
            {
                context.UserLogins.Add(login);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("operation failed", ex);
            }
        }

        public static bool LoginWithCredentials(ISession session, string name, string pwdhash, RmLoginSettings settings, RmContext context)
        {
            var hash = HashFromPwdHash(pwdhash, settings);
            var login = context.UserLogins.Include(u => u.User).FirstOrDefault(u => u.Name == name && u.Hash == hash);

            if (login?.User != null)
            {
                AssignUser(session, login.User);
                return true;
            }
            return false;
        }

        private static string GenerateToken()
        {
            var sb = new StringBuilder();

            lock (_rndLock)
            {
                for (int i = 0; i < 64; ++i)
                {
                    sb.Append(Chars[_rnd.Next(0, Chars.Length - 1)]);
                }
            }

            return sb.ToString();
        }

        public static string ThirdPartyLogin(string itsc, string timestr, string hash, RmLoginSettings settings, RmContext context)
        {
            // verify third party identity
            DateTime time;
            if (!DateTime.TryParseExact(timestr, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out time))
            {
                return "R_INVALID_TIME";
            }

            var diff = (DateTime.UtcNow - time).TotalSeconds;
            if (diff < 0)
            {
                return "R_FUTURE_TIME";
            }
            else if (diff > 10)
            {
                return "R_TIME_EXPIRED";
            }

            var target = CryptoHelper.GetMd5String(itsc + timestr + settings.ThirdPartyPsk);
            if (hash != target)
            {
                return "R_HASH_REJECTED";
            }

            lock (_loginHashLock)
            {
                if (_acceptedHashes.Contains(hash))
                    return "R_REPLAY";

                _acceptedHashes.Add(hash);
            }

            var token = GenerateToken();
            lock (_loginEntryLock)
            {
                var tuple = _thirdPartyLogins.FirstOrDefault(t => t.Item2 == itsc);
                if (tuple != null)
                    _thirdPartyLogins.Remove(tuple);

                tuple = new Tuple<DateTime, string, string>(time, itsc, token);
                _thirdPartyLogins.Add(tuple);
            }

            return token;
        }

        public static string RedeemToken(ISession session, string token, RmContext context)
        {
            Tuple<DateTime, string, string> entry;
            lock (_loginEntryLock)
            {
                entry = _thirdPartyLogins.FirstOrDefault(t => t.Item3 == token);
                if (entry == null)
                {
                    return "invalid token";
                }

                _thirdPartyLogins.Remove(entry);
            }

            var diff = (DateTime.UtcNow - entry.Item1).TotalSeconds;
            if (diff > 120)
            {
                return "token expired";
            }

            var itsc = entry.Item2;
            var user = context.Users.FirstOrDefault(u => u.Itsc == itsc);

            // create user with itsc if not exist
            // this complicated logic is to prevent someone from logging in from two places at the same time and get two Users created
            // this code will not be tested :P
            if (user == null)
            {
                var shallCreate = true;
                EventWaitHandle handle = null;
                lock (_userCreationLock)
                {
                    if (_creatingUser.ContainsKey(itsc))
                    {
                        // Some thread is creating the user. Wait until that thread completes creation and set the handle.
                        shallCreate = false;
                        handle = _creatingUser[itsc];
                    }
                    else
                    {
                        // This is the first thread in the area. Create the handle.
                        handle = new EventWaitHandle(false, EventResetMode.ManualReset);
                        _creatingUser[itsc] = handle;
                    }
                }

                if (shallCreate)
                {
                    try
                    {
                        user = new User
                        {
                            Itsc = itsc,
                            Nickname = itsc,
                            IsAdmin = false,
                            IsFullMember = true
                        };
                        context.Users.Add(user);
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception?.WriteLine(ex.GetExceptionString("UserManager", "RedeemToken CreateUser"));
                        return "server error";
                    }
                    finally
                    {
                        handle.Set();
                    }
                }
                else
                {
                    handle.WaitOne();
                    user = context.Users.FirstOrDefault(u => u.Itsc == itsc);
                    if (user == null)
                        return "server error";
                }
            }

            AssignUser(session, user);
            return "success";
        }

        public static void Logout(ISession session)
        {
            session.SetInt32(UserLoginKey, 0);
            session.SetString(UserEntityKey, String.Empty);
        }

        public static User GetUser(ISession session)
        {
            var userStr = session.GetString(UserEntityKey);
            if (String.IsNullOrEmpty(userStr))
                return null;

            try
            {
                return DeserializeUser(userStr);
            }
            catch (Exception ex)
            {
                Logger.Exception?.WriteLine(ex.GetExceptionString("UserManager", "GetUser Deserialize"));
                return null;
            }
        }

        public static bool IsLoggedIn(ISession session)
        {
            var state = session.GetInt32(UserLoginKey);
            return state.HasValue && state.Value > 0;
        }

        public static bool IsAdmin(ISession session)
        {
            var state = session.GetInt32(UserLoginKey);
            return state.HasValue && state.Value == 2;
        }

        public static bool IsFullMember(User user)
        {
            return user?.IsFullMember ?? false;
        }

        public static bool IsFullMember(ISession session)
        {
            return IsFullMember(GetUser(session));
        }

#if DEBUG
        public static void AssignUserAdmin(ISession session, User user)
        {
            session.SetInt32(UserLoginKey, 2);
            session.SetString(UserEntityKey, SerializeUser(user));
        }
#endif
    }
}
