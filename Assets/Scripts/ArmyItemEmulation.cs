namespace Assets.Scripts
{
    public class ArmyItemEmulation
    {
        public Army Army { get; set; }

        public ArmyItemEmulation(BoardStorageItem boardStorageItem)
        {
            if (boardStorageItem is ArmyStorageItem)
            {
                var item = boardStorageItem as ArmyStorageItem;
                Army = item.Army.Clone() as Army;
            }
        }
    }
}