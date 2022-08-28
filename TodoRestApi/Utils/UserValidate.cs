using TodoRestApi.Models;
using System.Linq;
using TodoRestApi.Utils;

namespace TodoRestApi.Utils
{
    public class UserValidate
    {
        private static TodoDbEntities db = new TodoDbEntities();

        public static bool Login(string username, string password)
        {
            User user = db.Users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());
            if(user != null)
            {
                return Encrypt.DecryptPassword(user.Password) == password;
            }

            return false;
        }
    }
}