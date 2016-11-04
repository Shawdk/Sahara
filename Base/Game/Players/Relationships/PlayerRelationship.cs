namespace Sahara.Base.Game.Players.Relationships
{
    internal class PlayerRelationship
    {
        private readonly int _relationshipId;
        private readonly int _playerId;
        private readonly PlayerRelationshipType _relationshipType;

        public PlayerRelationship(int relationshipId, int playerId, PlayerRelationshipType relationshipType)
        {
            _relationshipId = relationshipId;
            _playerId = playerId;
            _relationshipType = relationshipType;
        }
    }
}
