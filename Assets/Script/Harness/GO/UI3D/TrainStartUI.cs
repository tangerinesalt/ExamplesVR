/****************************************************
    功能：训练初始面板
    作者：ZH
    创建日期：#2025/01/17#
    修改内容：
        1.介绍UI切换功能    2025/03/18 ZH
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voltage
{
    public class TrainStartUI : UI3DBase
    {
        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            #region 介绍

            view["uiPanel/Step1"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(() =>
            {
                view["uiPanel/Step1"].SetActive(false);
                view["uiPanel/Step2"].SetActive(true);
            });
            view["uiPanel/Step2"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(() =>
            {
                view["uiPanel/Step2"].SetActive(false);
                view["uiPanel/Step3"].SetActive(true);
            });
            view["uiPanel/Step3"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(() =>
            {
                view["uiPanel/Step3"].SetActive(false);
                view["uiPanel/Step4"].SetActive(true);
            });
            view["uiPanel/Step4"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(() =>
            {
                view["uiPanel/Step4"].SetActive(false);
                view["uiPanel/Step5"].SetActive(true);
            });
            view["uiPanel/Step5"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(() =>
            {
                view["uiPanel/Step5"].SetActive(false);
                view["uiPanel/Step6"].SetActive(true);
            });
            view["uiPanel/Step6"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(() =>
            {
                view["uiPanel/Step6"].SetActive(false);
                view["uiPanel/Step7"].SetActive(true);
            });

            view["uiPanel/Step7"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(() =>
            {
                GOManager.Instance.RemoveGO(name);
                GOManager.Instance.ShowGO3D("ReelBox");

                PlayerManager.Instance.SetHandProjectorState(true);
                PlayerManager.Instance.SetReachDistance(1f);
            });

            #endregion
        }
    }
}