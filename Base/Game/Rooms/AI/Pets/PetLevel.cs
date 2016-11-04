namespace Sahara.Base.Game.Rooms.AI.Pets
{
    internal class PetLevel
    {
        private readonly int _petLevelId;
        private readonly int _petLevelXp;

        public PetLevel(int petLevelId, int petLevelXp)
        {
            _petLevelId = petLevelId;
            _petLevelXp = petLevelXp;
        }
    }
}
