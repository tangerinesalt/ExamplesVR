/****************************************************
    功能：菜单教学训练物体
    作者：ZH
    创建日期：#2025/03/25#
    修改内容：
        1.增加菜单训练相关内容   2025/03/25 ZH
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Voltage
{
    public class MenuTraining : UI3DBase
    {
        [Header("Input")]
        [SerializeField] private InputActionReference m_OpenLeftPanel;
        [SerializeField] private InputActionReference m_OpenRightPanel;

        private string stepRef = string.Empty;
        
        protected override void Awake()
        {
            base.Awake();
        }

        private void OnEnable()
        {
            if (m_OpenLeftPanel != null)
            {
                m_OpenLeftPanel.action.Enable();
                m_OpenLeftPanel.action.performed += ChangeLeftPanelStatus;
            }
            if (m_OpenRightPanel != null)
            {
                m_OpenRightPanel.action.Enable();
                m_OpenRightPanel.action.performed += ChangeRightPanelStatus;
            }
        }

        private void OnDisable()
        {
            if (m_OpenLeftPanel != null)
            {
                m_OpenLeftPanel.action.performed -= ChangeLeftPanelStatus;
            }
            if (m_OpenRightPanel != null)
            {
                m_OpenRightPanel.action.performed -= ChangeRightPanelStatus;
            }
        }

        private void ChangeRightPanelStatus(InputAction.CallbackContext context)
        {
            switch (stepRef)
            {
                case "Step13":
                    view["uiPanel/Step13"].SetActive(false);
                    stepRef = "Step13_1";
                    break;
                case "Step13_1":
                    view["uiPanel/Step14"].SetActive(true);
                    PlayerManager.Instance.HideAllControllerButton();
                    PlayerManager.Instance.SetControllerButtonHighlight(EControllerButtonType.PrimaryButton, true, EHandType.Left);
                    PlayerManager.Instance.SetControllerButtonHighlight(EControllerButtonType.SecondaryButton, true, EHandType.Left);
                    stepRef = "Step14";
                    PlayerManager.Instance.isCanOpenMainMenuRight = false;
                    PlayerManager.Instance.isCanOpenMainMenuLeft = true;
                    break;
            }
        }

        private void ChangeLeftPanelStatus(InputAction.CallbackContext context)
        {
            switch (stepRef)
            {
                case "Step14":
                    view["uiPanel/Step14"].SetActive(false);
                    stepRef = "Step14_1";
                    break;
                case "Step14_1":
                    view["uiPanel/Step15"].SetActive(true);
                    PlayerManager.Instance.HideAllControllerButton();
                    PlayerManager.Instance.isCanOpenMainMenuRight = false;
                    PlayerManager.Instance.isCanOpenMainMenuLeft = false;
                    break;
            }
        }

        private void Start()
        {
            #region 菜单教学

            view["uiPanel/Step12"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(() =>
            {
                view["uiPanel/Step12"].SetActive(false);
                view["uiPanel/Step13"].SetActive(true);
                stepRef = "Step13";
                PlayerManager.Instance.isCanOpenMainMenuLeft = false;
                PlayerManager.Instance.isCanOpenMainMenuRight = true;
                PlayerManager.Instance.SetControllerButtonHighlight(EControllerButtonType.PrimaryButton, true, EHandType.Right);
                PlayerManager.Instance.SetControllerButtonHighlight(EControllerButtonType.SecondaryButton, true, EHandType.Right);
            });
            view["uiPanel/Step15"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(() =>
            {
                PlayerManager.Instance.EnableMove = false;
                PlayerManager.Instance.EnableTurn = false;
                PlayerManager.Instance.SetHandModelState(EHandType.Both, false);
                PlayerManager.Instance.SetRayState(EHandType.Both, false);
                PlayerManager.Instance.EyeFadeDark();

                DelayFunc(0.5f, () =>
                {
                    GOManager.Instance.RemoveGO(name);
                    SceneServiceManager.Instance.LoadScene("04_Train");
                });
            });

            #endregion
        }
    }
}