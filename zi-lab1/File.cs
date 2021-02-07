using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zi_lab1
{
    public class File : Entity
    {
        public AccessRights AccessRights { get; private set; }

        public File(Dictionary<User, List<Permission>> accessMatrix) : base()
        {
            AccessRights = new AccessRights(accessMatrix);
        }

        public File(string name, string path, int permissionLevel, 
            Dictionary<User, List<Permission>> accessMatrix) : base(name, path, permissionLevel)
        {
            AccessRights = new AccessRights(accessMatrix);
        }

        public File(string serializedObj) : base(serializedObj)
        {
            var matrix = serializedObj.Split(",");
            var dict = new Dictionary<User, List<Permission>>();
            for(var i = 5; i < matrix.Length; i++)
            {
                var access = matrix[i].Split(":");
                var user = Context.Users.Single(u => u.Guid.ToString() == access[0]);
                var permissions = new List<Permission>();
                if (access[1].Contains("r"))
                    permissions.Add(Permission.Read);
                if (access[1].Contains("w"))
                    permissions.Add(Permission.Write);
                dict.Add(user, permissions);
            }
            AccessRights = new AccessRights(dict);
        }

        public override void Open()
        {
            throw new NotImplementedException();
        }
    }
}
