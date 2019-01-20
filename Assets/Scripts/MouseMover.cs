using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class MouseMover : MonoBehaviour
    {
        public Follower follower;
        public GameObject mouseGO;
        public Transform parentTransform;
        public event VoidHandler ReachedTarget;

        public void PrepareMovement(GameObject startPointObject)
        {
            mouseGO.transform.SetParent(parentTransform);
            mouseGO.transform.localPosition = startPointObject.transform.localPosition;
        }

        void Awake()
        {
            follower.ReachedTarget += FollowerReachedTarget;
        }

        private void FollowerReachedTarget()
        {
            if (ReachedTarget != null)
            {
                ReachedTarget();
            }
        }

        public void MoveTo(GameObject wayPoint)
        {
            StartCoroutine(follower.MoveTo(mouseGO.transform.localPosition, wayPoint.transform.localPosition));
        }
    }
}