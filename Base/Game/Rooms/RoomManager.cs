using Sahara.Core.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Sahara.Base.Game.Rooms
{
    internal class RoomManager
    {
        private readonly Dictionary<string, RoomModel> _roomModels;
        private ConcurrentDictionary<int, Room> _rooms;
        private ConcurrentDictionary<int, RoomInformation> _roomInformation;

        public RoomManager()
        {
            var stopwatch = Stopwatch.StartNew();

            _roomModels = new Dictionary<string, RoomModel>();
            _rooms = new ConcurrentDictionary<int, Room>();
            _roomInformation = new ConcurrentDictionary<int, RoomInformation>();

            LoadRoomModels();

            stopwatch.Stop();
            Sahara.GetServer().GetLogManager().Log("Loaded Room Manager [" + stopwatch.ElapsedMilliseconds + "ms]", LogType.Information);
        }

        private void LoadRoomModels(bool clearBefore = false)
        {
            if (clearBefore && _roomModels.Count > 0)
            {
                _roomModels.Clear();
            }

            using (var mysqlConnection = Sahara.GetServer().GetMySql().GetConnection())
            {
                mysqlConnection.OpenConnection();
                mysqlConnection.SetQuery("SELECT `id`, `door_x`, `door_y`, `door_z`, `door_dir`, `heightmap`, `public_items`, `club_only`, `poolmap`, `wall_height` FROM `room_models` WHERE `custom` = '0'");
                var roomModelsTable = mysqlConnection.GetTable();

                if (roomModelsTable == null)
                {
                    return;
                }

                foreach (DataRow roomModelRow in roomModelsTable.Rows)
                {
                    _roomModels.Add(Convert.ToString(roomModelRow["id"]), new RoomModel(Convert.ToInt32(roomModelRow["door_x"]), Convert.ToInt32(roomModelRow["door_y"]), Convert.ToDouble(roomModelRow["door_z"]), Convert.ToInt32(roomModelRow["door_dir"]), Convert.ToString(roomModelRow["heightmap"]), Convert.ToString(roomModelRow["public_items"]), Convert.ToInt32(roomModelRow["club_only"]).ToString() == "1", Convert.ToString(roomModelRow["poolmap"]), Convert.ToInt32(roomModelRow["wall_height"])));
                }

                mysqlConnection.CloseConnection();
            }
        }

        public bool TryGetRoomInformation(int roomId, out RoomInformation roomInformation)
        {
            return _roomInformation.TryGetValue(roomId, out roomInformation);
        }
    }
}

