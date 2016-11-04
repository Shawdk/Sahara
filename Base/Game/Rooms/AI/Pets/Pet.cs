namespace Sahara.Base.Game.Rooms.AI.Pets
{
    internal class Pet
    {
        private readonly int _petId;
        private readonly PetInformation _petInformation;

        public Pet(int petId, int petOwnerId, int petRoomId, string petName, int petType, string petRaceString, string petColorString, int petExperience, int petEnergy, int petNutrition, int petRespectPoints, double petCreatedTimestamp, int petCordinateX, int petCordinateY, double petCordinateZ, int horseSaddleId, bool canAnyoneRide)
        {
            _petId = petId;

            _petInformation = new PetInformation(petId, petOwnerId, petRoomId, petName,
                petType, petRaceString, petColorString, petExperience, petEnergy,
                petNutrition, petRespectPoints, petCreatedTimestamp, petCordinateX,
                petCordinateY, petCordinateZ, horseSaddleId, canAnyoneRide);
        }
    }
}
