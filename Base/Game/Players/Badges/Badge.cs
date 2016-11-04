namespace Sahara.Base.Game.Players.Badges
{
    internal class Badge
    {
        private readonly string _badgeCode;
        private readonly int _badgeSlotId;

        public Badge(string badgeCode, int badgeSlotId)
        {
            _badgeCode = badgeCode;
            _badgeSlotId = badgeSlotId;
        }

        public string BadgeCode => _badgeCode;
        public int BadgeSlotId => _badgeSlotId;
    }
}
