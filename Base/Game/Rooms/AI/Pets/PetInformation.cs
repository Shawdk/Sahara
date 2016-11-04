using System.Collections.Generic;

namespace Sahara.Base.Game.Rooms.AI.Pets
{
    internal class PetInformation
    {
        private readonly int _petId;
        private readonly int _petOwnerId;
        private readonly int _petRoomId;
        private readonly string _petName;
        private readonly int _petType;
        private readonly string _petRaceString;
        private readonly string _petColorString;
        private readonly int _petExperience;
        private readonly Dictionary<int, PetLevel> _petLevels;
        private readonly int _petEnergy;
        private readonly int _petNutrition;
        private readonly int _petRespectPoints;
        private readonly double _petCreatedTimestamp;
        private readonly int _petPositionX;
        private readonly int _petPositionY;
        private readonly double _petPositionZ;
        private readonly int _horseSaddleId;
        private readonly bool _canAnyoneRide;

        public PetInformation(int petId, int petOwnerId, int petRoomId, string petName, int petType, string petRaceString, string petColorString, int petExperience, int petEnergy, int petNutrition, int petRespectPoints, double petCreatedTimestamp, int petPositionX, int petPositionY, double petPositionZ, int horseSaddleId, bool canAnyoneRide)
        {
            _petId = petId;
            _petOwnerId = petOwnerId;
            _petRoomId = petRoomId;
            _petName = petName;
            _petType = petType;
            _petRaceString = petRaceString;
            _petColorString = petColorString;
            _petExperience = petExperience;

            _petLevels = new Dictionary<int, PetLevel>
            {
                {1, new PetLevel(1, 100)},
                {2, new PetLevel(2, 200)},
                {3, new PetLevel(3, 400)},
                {4, new PetLevel(4, 600)},
                {5, new PetLevel(5, 1000)},
                {6, new PetLevel(6, 1300)},
                {7, new PetLevel(7, 1800)},
                {8, new PetLevel(8, 2400)},
                {9, new PetLevel(9, 3200)},
                {10, new PetLevel(10, 4300)},
                {11, new PetLevel(11, 7200)},
                {12, new PetLevel(12, 8500)},
                {13, new PetLevel(13, 10100)},
                {14, new PetLevel(14, 13300)},
                {15, new PetLevel(15, 17500)},
                {16, new PetLevel(16, 23000)},
                {17, new PetLevel(17, 51900)},
                {18, new PetLevel(18, 75000)},
                {19, new PetLevel(19, 128000)},
                {20, new PetLevel(20, 150000)}
            };

            _petEnergy = petEnergy;
            _petNutrition = petNutrition;
            _petRespectPoints = petRespectPoints;
            _petCreatedTimestamp = petCreatedTimestamp;
            _petPositionX = petPositionX;
            _petPositionY = petPositionY;
            _petPositionZ = petPositionZ;
            _horseSaddleId = horseSaddleId;
            _canAnyoneRide = canAnyoneRide;
        }
    }
}
