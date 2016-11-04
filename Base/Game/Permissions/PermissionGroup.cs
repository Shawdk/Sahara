namespace Sahara.Base.Game.Permissions
{
    internal class PermissionGroup
    {
        private readonly string _permissionGroupName;
        private readonly string _permissionBadge;

        public PermissionGroup(string permissionGroupName, string permissionBadge)
        {
            _permissionGroupName = permissionGroupName;
            _permissionBadge = permissionBadge;
        }

        public string PermissionBadge => _permissionBadge;
    }
}
