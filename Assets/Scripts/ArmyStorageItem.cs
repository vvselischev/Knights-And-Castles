using UnityEngine;

namespace Assets.Scripts
{
    public class ArmyStorageItem : BoardStorageItem
    {
        public Army Army
        { get; set; }

        public ArmyStorageItem(Army army, GameObject iconGO) : base(iconGO)
        {
            Army = army;
            if (iconGO != null)
            {
                iconGO.AddComponent<Follower>();
                ObjectMover mover = iconGO.AddComponent<ObjectMover>();
                mover.ParentTransform = iconGO.GetComponentInParent<Transform>();
            }
        }

        public ArmyStorageItem CloneWithoutIcon()
        {
            return new ArmyStorageItem(Army.CloneArmy(), null);
        }
    }
}