using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Script for moving an object from one position to another.
    /// Velocity is set in the editor.
    /// Must be attached to game object with RectTransform component.
    /// </summary>
    public class Follower : MonoBehaviour
    {
        [SerializeField] private float velocity = 120;
        public event VoidHandler ReachedTarget;
        private float fraction;

        public IEnumerator MoveTo(Vector3 startPosition, Vector3 targetPosition)
        {
            fraction = 0;
            var direction = targetPosition - startPosition;
            while (fraction < 1)
            {
                fraction += velocity / direction.magnitude * Time.deltaTime;
                gameObject.GetComponent<RectTransform>().localPosition =
                    Vector3.Lerp(startPosition, targetPosition, fraction);
                yield return null;
            }
            ReachedTarget?.Invoke();
        }
    }
}