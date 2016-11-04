namespace Sahara.Base.Game.Players.Messenger
{
    internal class MessengerBuddy
    {
        private int _buddyUserId;

        public MessengerBuddy(int buddyUserId, string buddyUsername, string buddyClothing, string buddyMotto, int buddyLastOnline, MessengerState buddyMessengerState, bool buddyHideInRoom)
        {
            _buddyUserId = buddyUserId;
        }
    }
}
