using UnityEngine;

namespace Assets.Scripts
{
    public class ObjectMover : MonoBehaviour
    {
        private Follower follower;
        public Transform ParentTransform { get; set; }
        public event VoidHandler ReachedTarget;
        
        private GameObject sourceObject;

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

        public void MoveTo(GameObject wayPoint)
        {
            StartCoroutine(follower.MoveTo(sourceObject.transform.localPosition, wayPoint.transform.localPosition));
        }
    }
}