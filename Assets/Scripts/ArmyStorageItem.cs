using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Base class for an army on board.
    /// Contains its icon game object.
    /// Object mover component is attached in the constructor.
    /// </summary>
    public class ArmyStorageItem : BoardStorageItem
    {
        public Army Army
        { get; set; }

        public ArmyStorageItem(Army army, GameObject iconGO) : base(iconGO)
        {
            Army = army;
            if (iconGO == null)
            {
                return;
            }
            iconGO.AddComponent<Follower>();
            var mover = iconGO.AddComponent<ObjectMover>();
            mover.ParentTransform = iconGO.GetComponentInParent<Transform>();
        }

        public ArmyStorageItem CloneWithoutIcon()
        {
            return new ArmyStorageItem(Army.CloneArmy(), null);
        }
    }
}