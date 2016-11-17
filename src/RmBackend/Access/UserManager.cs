using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            catch (Exception)
            {
                // TODO: log exception
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
