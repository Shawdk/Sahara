using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Base.Game.Items
{
    internal class GameItem
    {
        private readonly int _gameItemId;
        private readonly int _gameItemRoomId;
        private readonly int _gameItemBaseId;
        private readonly string _additionalGameItemData;
        private readonly int _gameItemPositionX;
        private readonly int _gameItemPositionY;
        private readonly double _gameItemPositionZ;
        private readonly int _gameItemRotation;
        private readonly int _gameItemOwnerId;
        private readonly int _gameItemGroupId;
        private readonly int _gameItemLimitedAmount;
        private readonly int _gameItemLimitedStack;
        private readonly string _gameItemWallCordination;

        public GameItem(int gameItemId, int gameItemRoomId, int gameItemBaseId, string additionalGameItemData, int gameItemPositionX, int gameItemPositionY, double gameItemPositionZ, int gameItemRotation, int gameItemOwnerId, int gameItemGroupId, int gameItemLimitedAmount, int gameItemLimitedStack, string gameItemWallCordination)
        {
            _gameItemId = gameItemId;
            _gameItemRoomId = gameItemRoomId;
            _gameItemBaseId = gameItemBaseId;
            _additionalGameItemData = additionalGameItemData;
            _gameItemPositionX = gameItemPositionX;
            _gameItemPositionY = gameItemPositionY;
            _gameItemPositionZ = gameItemPositionZ;
            _gameItemRotation = gameItemRotation;
            _gameItemOwnerId = gameItemOwnerId;
            _gameItemGroupId = gameItemGroupId;
            _gameItemLimitedAmount = gameItemLimitedAmount;
            _gameItemLimitedStack = gameItemLimitedStack;
            _gameItemWallCordination = gameItemWallCordination;
        }
    }
}
