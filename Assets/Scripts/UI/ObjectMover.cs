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
        
        /// <summary>
        /// The object to move.
        /// </summary>
        private GameObject sourceObject;

        /// <summary>
        /// Initialization. Must be called before every usage.
        /// </summary>
        public void PrepareMovement(GameObject startPointObject)
        {
            sourceObject.transform.SetParent(ParentTransform);
            sourceObject.transform.localPosition = startPointObject.transform.localPosition;
        }

        /// <summary>
        /// Initialization on startup.
        /// Gets the follower component and subscribes on the finish movement event.
        /// </summary>
        private void Awake()
        {
            follower = gameObject.GetComponent<Follower>();
            follower.ReachedTarget += () =>
            {
                ReachedTarget?.Invoke();
            };
            sourceObject = gameObject;
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