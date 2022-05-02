//Created: 4/12/2022
//Finished: 4/29/2022
//Author: Duy Le
//This class contains methods to manage users.
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

        public static bool UpdateUser(String oldUsername, String username, String password, String role)
        {
            // attempt to update current user with new information
            Users updateUser = SoftwareEng.PreparedStatements.UpdateUser(oldUsername, username, password, role);

            // if success
            if (updateUser != null)
            {
                // then return true 
                return true;
            } else
            {
                // if not return false
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
            // check if a provided information is matched with any user information
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
