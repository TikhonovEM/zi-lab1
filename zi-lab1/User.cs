using System;
using System.Collections.Generic;
using System.Text;

namespace zi_lab1
{
    public class User
    {
        public Guid Guid { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public int PermissionLevel { get; set; }
    }
}
