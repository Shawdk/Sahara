using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Base.Game.Permissions
{
    internal class Permission
    {
        private readonly int _permissionId;
        private readonly string _permissionName;

        public Permission(int permissionId, string permissionName)
        {
            _permissionId = permissionId;
            _permissionName = permissionName;
        }

        public string PermissionName => _permissionName;
    }
}
