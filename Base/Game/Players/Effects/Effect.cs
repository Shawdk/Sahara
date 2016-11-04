using Sahara.Base.Utility;

namespace Sahara.Base.Game.Players.Effects
{
    internal class Effect
    {
        private readonly int _effectId;
        private readonly int _spriteId;
        private readonly double _effectDuration;
        private readonly bool _effectEnabled;
        private readonly double _timestampEnabled;
        private readonly int _amount;

        public Effect(int effectId, int spriteId, double effectDuration, bool effectEnabled, double timestampEnabled, int amount)
        {
            _effectId = effectId;
            _spriteId = spriteId;
            _effectDuration = effectDuration;
            _effectEnabled = effectEnabled;
            _timestampEnabled = timestampEnabled;
            _amount = amount;
        }

        public int EffectId => _effectId;
        public int SpriteId => _spriteId;
        public double EffectDuration => _effectDuration;
        public bool EffectEnabled => _effectEnabled;
        public int Amount => _amount;

        private double SecondsUsed => UnixTimestampGenerator.GetNow() - _timestampEnabled;

        public double SecondsLeft
        {
            get
            {
                var timeLeft = _effectEnabled ? _effectDuration - SecondsUsed : _effectDuration;

                if (timeLeft < 0)
                {
                    timeLeft = 0;
                }

                return timeLeft;
            }
        }
    }
}
