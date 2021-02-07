using System;
using System.Collections.Generic;
using System.Text;

namespace zi_lab1
{
    public class AccessRights
    {
        public Dictionary<User, List<Permission>> AccessMatrix { get; private set; }

        public AccessRights(Dictionary<User, List<Permission>> accessMatrix)
        {
            AccessMatrix = accessMatrix;
        }

        public bool CanRead()
        {
            if (!AccessMatrix.ContainsKey(Context.CurrentUser))
                return false;

            return AccessMatrix[Context.CurrentUser].Contains(Permission.Read);

        }
        public bool CanWrite()
        {
            if (!AccessMatrix.ContainsKey(Context.CurrentUser))
                return false;

            return AccessMatrix[Context.CurrentUser].Contains(Permission.Write);
        }
    }
}
