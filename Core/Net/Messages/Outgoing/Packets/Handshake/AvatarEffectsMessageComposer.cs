using Sahara.Base.Game.Players.Effects;
using Sahara.Core.Packets.Outgoing;
using Sahara.Core.Packets.Server;
using System;
using System.Collections.Generic;

namespace Sahara.Core.Net.Messages.Outgoing.Packets.Handshake
{
    internal class AvatarEffectsMessageComposer : OutgoingPacket
    {
        public AvatarEffectsMessageComposer(ICollection<Effect> effects)
            : base(OutgoingHeaders.AvatarEffectsMessageComposer)
        {
            base.WriteInteger(effects.Count);

            foreach (var effect in effects)
            {
                base.WriteInteger(effect.SpriteId);
                base.WriteInteger(0);
                base.WriteInteger(Convert.ToInt32(effect.EffectDuration));
                base.WriteInteger(effect.EffectEnabled ? effect.Amount - 1 : effect.Amount);
                base.WriteInteger(effect.EffectEnabled ? (int)effect.SecondsLeft : -1);
                base.WriteBoolean(false);
            }
        }
    }
}
