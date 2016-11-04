using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace Sahara.Base.Game.Players.Effects
{
    internal class EffectManagement
    {
        private readonly Player _player;
        private readonly ConcurrentDictionary<int, Effect> _effects;

        public EffectManagement(Player player)
        {
            _player = player;
            _effects = new ConcurrentDictionary<int, Effect>();
        }

        public void LoadEffects()
        {
            using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
            {
                mysqlConnection.OpenConnection();
                mysqlConnection.AddParameter("id", _player.GetPlayerData().PlayerId);
                mysqlConnection.SetQuery("SELECT * FROM `user_effects` WHERE `user_id` = @id");
                var effectsTable = mysqlConnection.GetTable();

                if (effectsTable == null)
                {
                    return;
                }

                foreach (DataRow effect in effectsTable.Rows)
                {
                    _effects.TryAdd(Convert.ToInt32(effect["id"]), new Effect(Convert.ToInt32(effect["id"]), Convert.ToInt32(effect["effect_id"]), Convert.ToDouble(effect["total_duration"]), Convert.ToInt32(effect["is_activated"]).ToString() == "1", Convert.ToDouble(effect["activated_stamp"]), Convert.ToInt32(effect["quantity"])));
                }

                mysqlConnection.CloseConnection();
            }
        }

        public bool TryAdd(Effect effect)
        {
            return _effects.TryAdd(effect.EffectId, effect);
        }

        public ICollection<Effect> GetAllEffects => _effects.Values;
    }
}
