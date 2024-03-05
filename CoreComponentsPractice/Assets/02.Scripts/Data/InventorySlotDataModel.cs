namespace DiceGame.Data
{
    public class InventorySlotDataModel
    {
        public InventorySlotDataModel(int itemID, int itemNum)
        {
            this.itemID = itemID;
            this.itemNum = itemNum;
        }

        public bool isEmpty => itemID == 0 || itemNum == 0;

        public int itemID;
        public int itemNum;
    }
}