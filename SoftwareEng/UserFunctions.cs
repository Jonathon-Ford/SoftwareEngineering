using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareEng
{
    internal class UserFunctions
    {
        public static Users AddUser(String username, String password, String role)
        {
            // find if user exists or not
            Users existUser = SoftwareEng.UserFunctions.FindUser(username);

            // if user exists
            if (existUser == null)
            {
                // then add that user with provided username, password, and role
                Users newUser = SoftwareEng.PreparedStatements.AddUser(username, password, role);
                return newUser;
            }
            else
            {
                return null;
            }
        }

        public static bool UpdateUser(String username, String password, String role)
        {
            // find if user exists or note
            Users existUser = SoftwareEng.UserFunctions.FindUser(username);

            // if user exists
            if (existUser != null)
            {
                // then update that user with provided username, password, and role
                SoftwareEng.PreparedStatements.UpdateUser(username, password, role);
                return true; 
            }
            else
            {
                return false;
            }
        }

        public static Users FindUser(String username)
        {
            // find if user exists or note
            Users existUser = SoftwareEng.PreparedStatements.FindUser(username);
            return existUser;
        }

        public static Users ValidateUser(String username, String password)
        {
            Users validUser = SoftwareEng.PreparedStatements.ValidateUser(username, password);
            return validUser;
        }
        public static bool DeleteUser(String username)
        {
            bool success = SoftwareEng.PreparedStatements.DeleteUser(username);
            return success;
        }
    }
}
