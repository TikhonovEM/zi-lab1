using System;
using System.Collections.Generic;
using System.Text;

namespace zi_lab1
{
    public abstract class Entity
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int PermissionLevel { get; set; }
        public abstract void Open();

        public Entity()
        {
            Guid = Guid.NewGuid();
        }

        public Entity(string name, string path, int permissionLevel) : this()
        {
            Name = name;
            Path = path;
            PermissionLevel = permissionLevel;
        }

        public Entity(string serializedObj)
        {
            var props = serializedObj.Split(",");
            Guid = Guid.Parse(props[1]);
            Name = props[2];
            Path = props[3].Equals("@") ? string.Empty : props[3];
            PermissionLevel = Convert.ToInt32(props[4]);
        }
    }
}
