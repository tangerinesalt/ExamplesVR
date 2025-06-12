/****************************************************
    功能：教学初始面板
    作者：ZH
    创建日期：#2025/01/16#
    修改内容：
        1.增加手柄按键控制流程    2025/03/05 ZH
        2.增加抓取练习流程    2025/03/11 ZH
        3.分离各步骤逻辑到各训练物体   2025/03/25 ZH
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Voltage
{
    public class TeachStartUI : UI3DBase
    {
        private void Start()
        {
            PlayerManager.Instance.moveMode = EMoveMode.MovebyPhysics;
            #region 教学准备

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
                GOManager.Instance.RemoveGO(name);
                GOManager.Instance.ShowGO3D("MoveTraining");
            });

            #endregion
        }
    }
}