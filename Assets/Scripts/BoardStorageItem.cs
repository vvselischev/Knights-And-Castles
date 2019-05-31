using UnityEngine;

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