using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand.Demo;
using Autohand;

namespace Voltage
{
    /// <summary>
    /// Provide call functions such as remote grasping and projection for grasping
    /// </summary>
    public class HandToolControl : MonoBehaviour
    {
        //wake up Distance Grabber pointer
        public void WakeDistanceGrabberPointer()
        {
            //UIRayManager.Instance.SetRayGraberStatus(true);//远程抓取射线对象启用并StartPointer
        }
        //cancel Distance Grabber pointer
        public void CancelDistanceGrabberPointer()
        {
            //UIRayManager.Instance.SetRayGraberStatus(false);
        }

        //wake hand highlight projection
        public void EnableHighlightProjection()
        {
            var projections = AutoHandExtensions.CanFindObjectsOfType<HandProjector>(true);

            foreach (var projection in projections)
            {
                projection.gameObject.SetActive(true);
                if (!projection.useGrabTransition)
                    projection.enabled = true;
            }
        }
        //disable hand highlight projection
        public void DisableHighlightProjection()
        {
            var projections = AutoHandExtensions.CanFindObjectsOfType<HandProjector>(true);
            foreach (var projection in projections)
            {
                projection.gameObject.SetActive(false);
                if (!projection.useGrabTransition)
                    projection.enabled = false;
            }
        }

        float[] initialreachDistances;
        //change reach distance for all hands
        public void SetHandReachDistance(float _reachDistance = 1.2f)
        {
            var hands = AutoHandExtensions.CanFindObjectsOfType<Hand>(false);
            initialreachDistances = new float[hands.Length];
            for (int i = 0; i < hands.Length; i++)
            {
                initialreachDistances[i] = hands[i].reachDistance;
                hands[i].reachDistance = _reachDistance;
            }
        }
        //recovery reach distance to default
        public void RecoveryHandReachDistance()
        {
            if (initialreachDistances == null) return;

            var hands = AutoHandExtensions.CanFindObjectsOfType<Hand>(false);
            for (int i = 0; i < hands.Length; i++)
            {
                hands[i].reachDistance = initialreachDistances[i];
            }
            initialreachDistances = null;
        }


    }
}
