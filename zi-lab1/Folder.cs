using System;
using System.Collections.Generic;
using System.Text;

namespace zi_lab1
{
    public class Folder : Entity
    {
        public Folder(string serializedObj) : base(serializedObj)
        {

        }

        public Folder(string name, string path, int permissionLevel) : base(name, path, permissionLevel)
        {

        }

        public override string ToString()
        {
            return $"d,{Guid},{Name},{Path},{PermissionLevel}";
        }
        public override void Open()
        {
            throw new NotImplementedException();
        }
    }
}
