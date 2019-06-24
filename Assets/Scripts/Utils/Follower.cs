using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Script for moving an object from one position to another directly.
    /// Velocity is set in the editor.
    /// Must be attached to game object with RectTransform component.
    /// </summary>
    public class Follower : MonoBehaviour
    {
        /// <summary>
        /// The velocity of an object movement.
        /// </summary>
        [SerializeField] private float velocity = 120;
        /// <summary>
        /// This event is risen when the movement is finished, i.e. the object reaches the destination.
        /// </summary>
        public event VoidHandler ReachedTarget;
        /// <summary>
        /// The current part of the way.
        /// </summary>
        private float fraction;

        /// <summary>
        /// Starts a movement between the given positions directly.
        /// </summary>
        public IEnumerator MoveTo(Vector3 startPosition, Vector3 targetPosition)
        {
            fraction = 0;
            //Compute the direction vector.
            var direction = targetPosition - startPosition;
            while (fraction < 1) //while not all the path is passed
            {
                //Compute the part of path that is passed and move the object.
                fraction += velocity / direction.magnitude * Time.deltaTime;
                gameObject.GetComponent<RectTransform>().localPosition = 
                    Vector3.Lerp(startPosition, targetPosition, fraction);
                yield return null;
            }
            //The movement is finished.
            ReachedTarget?.Invoke();
        }
    }
}