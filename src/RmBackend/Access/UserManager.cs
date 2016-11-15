using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using RmBackend.Models;

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
            // TODO: for MSSSUG they should have a list of their full members and check it here, but I don't, so just grant everyone full membership
            return user != null;
        }

        public static bool IsFullMember(ISession session)
        {
            return IsFullMember(GetUser(session));
        }

#if DEBUG
        public static void AssignUser(ISession session, User user)
        {
            session.SetInt32(UserLoginKey, 2);
            session.SetString(UserEntityKey, SerializeUser(user));
        }
#endif
    }
}
