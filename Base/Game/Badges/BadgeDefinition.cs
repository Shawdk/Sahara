namespace Sahara.Base.Game.Badges
{
    internal class BadgeDefinition
    {
        private readonly string _badgeCode;
        private readonly string _permissionRequired;

        public BadgeDefinition(string badgeCode, string permissionRequired)
        {
            _badgeCode = badgeCode;
            _permissionRequired = permissionRequired;
        }

        public string BadgeCode => _badgeCode;
        public string PermissionRequired => _permissionRequired;
    }
}
