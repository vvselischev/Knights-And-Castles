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

        public ArmyStorageItem(BoardButton boardButton, Army army, GameObject iconGO) : base(boardButton, iconGO)
        {
           Army = army;
        }   
    }
}
