using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Base.Game.Players.Inventory.Bots
{
    internal class Bot
    {
        private readonly int _botId;
        private readonly int _botOwnerId;
        private readonly string _botName;
        private readonly string _botMotto;
        private readonly string _botFigure;
        private readonly BotGender _botGender;

        public Bot(int botId, int botOwnerId, string botName, string botMotto, string botFigure, BotGender botGender)
        {
            _botId = botId;
            _botOwnerId = botOwnerId;
            _botName = botName;
            _botMotto = botMotto;
            _botFigure = botFigure;
            _botGender = botGender;
        }
    }
}
