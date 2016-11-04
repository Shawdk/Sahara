namespace Sahara.Base.Game.Players.Clothing
{
    internal class ClothingParts
    {
        private int _id;
        private string _partString;
        private int _partId;

        public ClothingParts(int id, string partString, int partId)
        {
            _id = id;
            _partString = partString;
            _partId = partId;
        }

        public int PartId
        {
            get { return this._partId; }
            set { this._partId = value; }
        }

        public string Parts
        {
            get { return this._partString; }
            set { this._partString = value; }
        }
    }
}
