using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Base class for a gameObject on board.
    /// </summary>
    public abstract class BoardStorageItem
    {
        public GameObject StoredObject { get; }

        protected BoardStorageItem(GameObject targetObject)
        {
            StoredObject = targetObject;
        }
    }
}