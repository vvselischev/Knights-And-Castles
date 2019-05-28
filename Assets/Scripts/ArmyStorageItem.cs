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
                ObjectMover mover = iconGO.AddComponent<ObjectMover>();
                Follower follower = iconGO.AddComponent<Follower>();
                mover.parentTransform = iconGO.GetComponentInParent<Transform>();
                mover.follower = follower;
            }
        }

        public ArmyStorageItem CloneWithoutIcon()
        {
            return new ArmyStorageItem(Army.CloneArmy(), null);
        }
    }
}