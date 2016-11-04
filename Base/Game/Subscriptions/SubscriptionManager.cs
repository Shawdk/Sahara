using System;
using System.Collections.Generic;
using System.Data;

namespace Sahara.Base.Game.Subscriptions
{
    internal class SubscriptionManager
    {
        private readonly Dictionary<int, SubscriptionInformation> _subscriptions;

        public SubscriptionManager()
        {
            _subscriptions = new Dictionary<int, SubscriptionInformation>();
        }

        private void LoadSubscriptions()
        {
            using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
            {
                mysqlConnection.OpenConnection();
                mysqlConnection.SetQuery("SELECT * FROM `subscriptions`");
                var subscriptionTable = mysqlConnection.GetTable();

                if (subscriptionTable == null)
                {
                    return;
                }

                foreach (DataRow subscription in subscriptionTable.Rows)
                {
                    if (!_subscriptions.ContainsKey(Convert.ToInt32(subscription["id"])))
                    {
                        _subscriptions.Add(Convert.ToInt32(subscription["id"]), new SubscriptionInformation(Convert.ToInt32(subscription["id"]), Convert.ToString(subscription["badge_code"]), Convert.ToString(subscription["name"]), Convert.ToInt32(subscription["credits"]), Convert.ToInt32(subscription["respects"]), Convert.ToInt32(subscription["duckets"])));
                    }
                }

                mysqlConnection.CloseConnection();
            }
        }

        public bool TryGetSubscriptionInformation(int subscriptionId, out SubscriptionInformation subscriptionInformation)
        {
            return _subscriptions.TryGetValue(subscriptionId, out subscriptionInformation);
        }
    }
}
