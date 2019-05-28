using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ObjectMover : MonoBehaviour
    {
        public Follower follower;
        public Transform parentTransform;
        public event VoidHandler ReachedTarget;
        
        private GameObject sourceObject;

        public void PrepareMovement(GameObject startPointObject)
        {
            sourceObject.transform.SetParent(parentTransform);
            sourceObject.transform.localPosition = startPointObject.transform.localPosition;
        }

        void Awake()
        {
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