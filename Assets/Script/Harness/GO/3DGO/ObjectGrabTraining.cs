/****************************************************
	功能：物体抓取训练
    作者：ZZQ
    创建日期：#2025/03/06#
    修改内容：
        1.新增完成任务后，开启轴唛抓取练习    2025/03/11 ZH
        2.抓取释放时更新抓取距离，防止抓取时某只手带射线覆盖UI导致抓取距离改变   2025/03/24 ZZQ
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voltage;
using Autohand;
using System;
using UnityEngine.UI;

namespace Voltage
{
    public class ObjectGrabTraining : GO3DBase
    {
        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            PlayerManager.Instance.SetControllerModelState(EHandType.Both, false);
            PlayerManager.Instance.SetHandModelState(EHandType.Both, true);
            PlayerManager.Instance.SetReachDistance(0.8f);
            PlayerManager.Instance.SetHandProjectorState(true);
            PlayerManager.Instance.SetRayState(EHandType.Both, true);
            view["Canvas/uiPanel/Step/btn_skip"].GetComponent<Button>().onClick.AddListener(OnComplete);

            foreach (Transform child in view["Grabbable"].transform)
            {
                child.GetComponent<Grabbable>().onGrab.AddListener(
                    (Hand hand, Grabbable grabbable) => { SetRayState(false); }
                );
                child.GetComponent<Grabbable>().onRelease.AddListener(
                    (Hand hand, Grabbable grabbable) => { SetRayState(true); }
                );
            }
        }

        private void OnComplete()
        {
            GOManager.Instance.RemoveGO("ObjectGrabTraining");
            GOManager.Instance.ShowGO3D("GrabTraining");

            PlayerManager.Instance.SetControllerModelState(EHandType.Both, true);
            PlayerManager.Instance.SetHandModelState(EHandType.Both, false);
            PlayerManager.Instance.SetReachDistance(0.8f);
            PlayerManager.Instance.SetHandProjectorState(false);
            PlayerManager.Instance.SetRayState(EHandType.Both, false);
        }
        
        private void SetRayState(bool state)
        {
            if (state)
            {
                foreach (Hand hand in FindObjectsOfType<Hand>())
                {
                    if (hand.IsHolding() && hand.holdingObj.IsHeld()) return;
                }
            }
            PlayerManager.Instance.SetRayState(EHandType.Both, state);
            PlayerManager.Instance.SetReachDistance(0.8f);//更新抓取距离
        }
    }
}