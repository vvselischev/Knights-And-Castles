﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public abstract class BoardStorageItem
    {
        public GameObject StoredObject { get; }

        protected BoardStorageItem(GameObject targetObject)
        {
            StoredObject = targetObject;
        }
    }
}