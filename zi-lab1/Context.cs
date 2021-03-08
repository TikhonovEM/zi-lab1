using System;
using System.Collections.Generic;
using System.Text;

namespace zi_lab1
{
    public static class Context
    {
        public static User CurrentUser { get; set; }
        public static List<Entity> Entities { get; } = new List<Entity>();
        public static List<User> Users { get; } = new List<User>()
        {
            new User()
            {
                Guid = Guid.Parse("d78fef6f-9678-4db3-ba8f-748a5df7a383"),
                Username = "qwe",
                PasswordHash = "123",
                PermissionLevel = 1
            },
            new User()
            {
                Guid = Guid.Parse("95308e67-ae52-442e-8b47-e49070622a87"),
                Username = "test",
                PasswordHash = "123",
                PermissionLevel = 0
            }
        };
    }
}
