using UnityEngine.Analytics;

namespace Assets.Scripts
{
    public class ArmyStorageItemEmulation
    {
        public bool IsAvailable { get; }

        public Army Army { get; set; }

        public ArmyStorageItemEmulation(BoardStorageItem boardStorageItem)
        {
            if (boardStorageItem is ArmyStorageItem)
            {
                var item = boardStorageItem as ArmyStorageItem;
                if (item.Army != null)
                {
                    Army = item.Army.Clone() as Army;

                    if (item.Army is UserArmy)
                    {
                        var userArmy = item.Army as UserArmy;
                        if (!userArmy.IsActive())
                        {
                            ((UserArmy) Army).SetInactive();
                        }
                    }
                }
            }

            IsAvailable = true;
        }

        public ArmyStorageItemEmulation(Army army)
        {
            Army = army;
        }
    }
}