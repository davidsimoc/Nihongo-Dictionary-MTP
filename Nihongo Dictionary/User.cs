using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nihongo_Dictionary
{
    public class User
    {
        public string Username {  get; set; }
        public string Role {  get; set; }
        public string AboutMe { get; set; }

        public User(string username, string role, string aboutMe)
        {
            Username = username;
            Role = role;
            AboutMe = aboutMe;
        }
    }
}
