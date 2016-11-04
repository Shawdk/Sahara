using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Base.Game.Subscriptions
{
    internal class SubscriptionInformation
    {
        private readonly int _id;
        private readonly string _name;
        private readonly string _badge;
        private readonly int _credits;
        private readonly int _duckets;
        private readonly int _respects;

        public SubscriptionInformation(int id, string badge, string name, int credits, int respects, int duckets)
        {
            _id = id;
            _badge = badge;
            _name = name;
            _credits = credits;
            _respects = respects;
            _duckets = duckets;
        }
        
        public string Badge => _badge;
        public int Credits => _credits;
        public int Duckets => _duckets;
        public int Respects => _respects;
    }
}
