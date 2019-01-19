using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ArmyStorageItem : BoardStorageItem
    {
        public Army Army
        { get; set; }

        public ArmyStorageItem(Army army, GameObject iconGO) : base(iconGO)
        {
            Army = army;
            ObjectMover mover = iconGO.AddComponent<ObjectMover>();
            Follower follower = iconGO.AddComponent<Follower>();
            mover.parentTransform = iconGO.GetComponentInParent<Transform>();
            mover.follower = follower;
        }
    }
}
