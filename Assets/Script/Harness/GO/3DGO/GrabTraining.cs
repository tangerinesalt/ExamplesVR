/****************************************************
	功能：抓取训练物体
    作者：ZH
    创建日期：#2025/03/03#
    修改内容：
        1.增加UI相关功能    2025/03/25 ZH
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using Autohand;
using UnityEngine;
using UnityEngine.UI;

namespace Voltage
{
    public class GrabTraining : GO3DBase
    {
        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            InitUI();
            InitGrab();
        }

        private void InitUI()
        {
            view["HideTrigger"].GetOrAddComponent<TriggerEvent>().enterTriggerEvent.AddListener(() =>
            {
                view["HideTrigger"].SetActive(false);
                view["uiPanel/Step7"].SetActive(false);
            });

            view["MoveMark01"].GetOrAddComponent<TriggerEvent>().enterTriggerEvent.AddListener(() =>
            {
                view["MoveMark01"].SetActive(false);
                view["uiPanel/Step8"].SetActive(true);
            });

            view["uiPanel/Step8"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(() =>
            {
                view["uiPanel/Step8"].SetActive(false);
                view["uiPanel/Step9"].SetActive(true);
                PlayerManager.Instance.SetControllerModelState(EHandType.Both, false);
                PlayerManager.Instance.SetHandModelState(EHandType.Both, true);
                PlayerManager.Instance.SetHandProjectorState(true);
            });

            view["uiPanel/Step10/btnSelect"].GetComponent<Button>().onClick.AddListener(() =>
            {
                GOManager.Instance.RemoveGO(name);
                GOManager.Instance.ShowGO3D("MenuTraining");

                PlayerManager.Instance.HideAllControllerButton();
                PlayerManager.Instance.SetRayState(EHandType.Both, false);

                PlayerManager.Instance.SetControllerModelState(EHandType.Both, true);
                PlayerManager.Instance.SetHandModelState(EHandType.Both, false);
                PlayerManager.Instance.SetHandProjectorState(false);
            });
        }

        private void InitGrab()
        {
            view["Reel"].GetComponentInChildren<Grabbable>().onRelease.AddListener((hand, grabbable) =>
            {
                view["uiPanel/Step9"].SetActive(false);
                view["uiPanel/Step10"].SetActive(true);

                PlayerManager.Instance.SetRayState(EHandType.Both, true);
            });
        }
    }
}