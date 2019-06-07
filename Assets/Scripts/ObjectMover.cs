using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Script for moving to the given GameObject.
    /// Should be attached to the object, supposed to move.
    /// </summary>
    public class ObjectMover : MonoBehaviour
    {
        private Follower follower;
        public Transform ParentTransform { get; set; }
        
        /// <summary>
        /// Invoked when object reaches the target.
        /// </summary>
        public event VoidHandler ReachedTarget;
        
        private GameObject sourceObject;

        /// <summary>
        /// Initialization. Must be called before every usage.
        /// </summary>
        public void PrepareMovement(GameObject startPointObject)
        {
            sourceObject.transform.SetParent(ParentTransform);
            sourceObject.transform.localPosition = startPointObject.transform.localPosition;
        }

        private void Awake()
        {
            follower = gameObject.GetComponent<Follower>();
            follower.ReachedTarget += FollowerReachedTarget;
            sourceObject = gameObject;
        }

        private void FollowerReachedTarget()
        {
            ReachedTarget?.Invoke();
        }

        /// <summary>
        /// Starts the movement.
        /// </summary>
        public void MoveTo(GameObject wayPoint)
        {
            StartCoroutine(follower.MoveTo(sourceObject.transform.localPosition, 
                wayPoint.transform.localPosition));
        }
    }
}