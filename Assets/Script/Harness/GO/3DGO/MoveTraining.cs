/****************************************************
	功能：移动训练物体
    作者：ZZQ
    创建日期：#2025/02/21#
    修改内容：
        1.震动音效触发    2025/03/07 ZH
        2.UI相关添加    2025/03/25 ZH
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voltage
{
    public class MoveTraining : GO3DBase
    {
        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            InitializeMoveMarkers();
            InitUI();
        }

        /// <summary>
        /// 初始化移动标记
        /// </summary>
        private void InitializeMoveMarkers()
        {
            foreach (Transform child in view["MoveMark"].transform)
            {
                child.gameObject.GetOrAddComponent<TriggerEvent>().enterTriggerEvent.AddListener(() =>
                {
                    HandleMarkerArrival(child.gameObject.name);
                });
            }
            HideAllMoveMarkers();
            view["MoveMark/MoveMark01"].SetActive(true);
        }

        /// <summary>
        /// 处理标记到达事件
        /// </summary>
        /// <param name="markerName"></param>
        private void HandleMarkerArrival(string markerName)
        {
            HideAllMoveMarkers();

            PlayerManager.Instance.Haptic(EHandType.Both);
            PlayerManager.Instance.PlaySound();

            switch (markerName)
            {
                case "MoveMark01":

                    view["MoveMark/MoveMark02"].SetActive(true);

                    break;
                case "MoveMark02":

                    view["uiPanel/Step4"].SetActive(true);
                    view["MoveMark/MoveMark03"].SetActive(true);

                    break;
                case "MoveMark03":

                    view["uiPanel/Step4"].SetActive(false);
                    view["MoveMark/MoveMark04"].SetActive(true);

                    break;
                case "MoveMark04":

                    view["MoveMark/MoveMark04"].SetActive(false);
                    view["uiPanel/Step5"].SetActive(true);
                    PlayerManager.Instance.SetControllerButtonHighlight(EControllerButtonType.Stick);

                    break;
                case "MoveMark05":

                    view["MoveMark/MoveMark06"].SetActive(true);
                    PathFinding.Instance.SetNavigationTarget(view["MoveMark/MoveMark06/pos"].transform);

                    break;
                case "MoveMark06":

                    view["MoveMark/MoveMark07"].SetActive(true);
                    PathFinding.Instance.SetNavigationTarget(view["MoveMark/MoveMark07/pos"].transform);

                    break;
                case "MoveMark07":

                    view["MoveMark/MoveMark08"].SetActive(true);
                    PathFinding.Instance.SetNavigationTarget(view["MoveMark/MoveMark08/pos"].transform);

                    break;
                case "MoveMark08":

                    PathFinding.Instance.ToggleNavigation(false);
                    PlayerManager.Instance.HideAllControllerButton();
                    view["uiPanel/Step6"].SetActive(true);

                    break;
                default:
                    Debug.LogWarning($"Unknown marker name: {markerName}");
                    break;
            }
        }

        /// <summary>
        /// 隐藏所有移动标记
        /// </summary>
        private void HideAllMoveMarkers()
        {
            foreach (Transform child in view["MoveMark"].transform)
            {
                if (child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        private void InitUI()
        {
            view["uiPanel/Step5"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(() =>
            {
                view["uiPanel/Step5"].SetActive(false);

                PlayerManager.Instance.EnableMove = true;
                // PlayerManager.Instance.EnableTurn = true;

                view["MoveMark/MoveMark05"].SetActive(true);
                PathFinding.Instance.SetNavigationTarget(view["MoveMark/MoveMark05/pos"].transform);
            });

            view["uiPanel/Step6"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(() =>
            {
                view["uiPanel/Step6"].SetActive(false);
                GOManager.Instance.RemoveGO(name);
                GOManager.Instance.ShowGO3D("ObjectGrabTraining");
                PlayerManager.Instance.SetControllerButtonHighlight(EControllerButtonType.Trigger);
            });
        }
    }
}