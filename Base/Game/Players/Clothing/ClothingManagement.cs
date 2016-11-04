using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace Sahara.Base.Game.Players.Clothing
{
    internal class ClothingManagement
    {
        private readonly Player _player;
        private readonly ConcurrentDictionary<int, ClothingParts> _clothingParts;

        public ClothingManagement(Player player)
        {
            _player = player;
            _clothingParts = new ConcurrentDictionary<int, ClothingParts>();
        }

        public void LoadClothingParts()
        {
            using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
            {
                mysqlConnection.OpenConnection();
                mysqlConnection.AddParameter("id", _player.GetPlayerData().PlayerId);
                mysqlConnection.SetQuery("SELECT `id`, `part_id`, `part` FROM `user_clothing` WHERE `user_id` = @id");
                var clothingTable = mysqlConnection.GetTable();

                if (clothingTable == null)
                {
                    return;
                }

                foreach (DataRow clothing in clothingTable.Rows)
                {
                    _clothingParts.TryAdd(Convert.ToInt32(clothing["part_id"]), new ClothingParts(Convert.ToInt32(clothing["id"]), Convert.ToString(clothing["part"]), Convert.ToInt32(clothing["part_id"])));
                }

                mysqlConnection.CloseConnection();
            }
        }

        public ICollection<ClothingParts> ClothingParts => _clothingParts.Values;
    }
}
