/****************************************************
    功能：主菜单界面控制脚本
    作者：ZZQ
    创建日期：#2025/03/03#
    修改内容：
        1.继承3DUI基类，增加面板显隐控制参数，方法代码优化合并    2025/03/05 ZH
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Obi;

namespace Voltage
{
    public class PanelControl : UI3DBase
    {
        [Header("Input")]
        [SerializeField] private InputActionReference m_OpenLeftPanel;
        [SerializeField] private InputActionReference m_OpenRightPanel;

        [Header("Panel Settings")]
        [SerializeField] public float m_panelDistance = 0.6f;
        [SerializeField] public float m_Offset = 0f;

        [Header("Panel child")]
        [SerializeField] private Transform m_LeftPanelTF;
        [SerializeField] private Transform m_RightPanelTF;

        private bool LeftPanelDisplayStatus = false;
        private bool RightPanelDisplayStatus = false;

        protected override void Awake()
        {
            base.Awake();
            SetDisplayStatus(DisplayStatus.CloseAll);

            kx = (mapB.x - mapA.x) / (realB.x - realA.x);
            ky = (mapB.y - mapA.y) / (realB.z - realA.z);
        }

        private void OnEnable()
        {
            if (m_OpenLeftPanel != null)
            {
                m_OpenLeftPanel.action.Enable();
                m_OpenLeftPanel.action.performed += LeftAction;
            }
            if (m_OpenRightPanel != null)
            {
                m_OpenRightPanel.action.Enable();
                m_OpenRightPanel.action.performed += RightAction;
            }
        }

        private void OnDisable()
        {
            if (m_OpenLeftPanel != null)
            {
                m_OpenLeftPanel.action.performed -= LeftAction;
            }
            if (m_OpenRightPanel != null)
            {
                m_OpenRightPanel.action.performed -= RightAction;
            }
        }

        public void LeftAction(InputAction.CallbackContext context)
        {
            if (PlayerManager.Instance.isCanOpenMainMenu && PlayerManager.Instance.isCanOpenMainMenuLeft)
            {
                LeftPanelDisplayStatus = !LeftPanelDisplayStatus;
                m_LeftPanelTF.gameObject.SetActive(LeftPanelDisplayStatus);
                if (LeftPanelDisplayStatus) UpdatePanelTF();
            }
        }

        public void RightAction(InputAction.CallbackContext context)
        {
            if (PlayerManager.Instance.isCanOpenMainMenu && PlayerManager.Instance.isCanOpenMainMenuRight)
            {
                RightPanelDisplayStatus = !RightPanelDisplayStatus;
                m_RightPanelTF.gameObject.SetActive(RightPanelDisplayStatus);
                if (RightPanelDisplayStatus) UpdatePanelTF();
            }
        }

        /// <summary>
        /// 更新面板位置
        /// </summary>
        private void UpdatePanelTF()
        {
            if (Camera.main == null) return;

            Vector3 headForward = Camera.main.transform.forward;
            Vector3 displayPosition = Camera.main.transform.position + Camera.main.transform.forward * m_panelDistance;

            if (!Mathf.Approximately(m_Offset, 0f))
            {
                Vector3 mainMenuUp = Vector3.ProjectOnPlane(Vector3.up, headForward).normalized;
                displayPosition += mainMenuUp * m_Offset;
            }

            transform.position = displayPosition;
            transform.rotation = Quaternion.LookRotation(headForward);
        }

        /// <summary>
        /// 设置显示状态
        /// </summary>
        /// <param name="status"></param>
        public void SetDisplayStatus(DisplayStatus status)
        {
            LeftPanelDisplayStatus = status == DisplayStatus.OnlyLeft || status == DisplayStatus.OpenAll;
            RightPanelDisplayStatus = status == DisplayStatus.OnlyRight || status == DisplayStatus.OpenAll;
            m_LeftPanelTF.gameObject.SetActive(LeftPanelDisplayStatus);
            m_RightPanelTF.gameObject.SetActive(RightPanelDisplayStatus);
        }

        private Vector3 realA = new Vector3(2.11f, 0, -1.35f);
        private Vector3 realB = new Vector3(393.91f, 0, -247.57f);

        private Vector2 mapA = new Vector2(-210.68f, 238.6f);
        private Vector2 mapB = new Vector2(244.06f, -222.71f);

        private float kx;
        private float ky;

        public RectTransform playerMap;

        public void Update()
        {
            UpdateMapPlayerPos();
        }

        private void UpdateMapPlayerPos()
        {
            Vector3 playerPos = PlayerManager.Instance.GetPlayerPosition();
            float x = (playerPos.x - realA.x) * kx + mapA.x;
            float y = (playerPos.z - realA.z) * ky + mapA.y;

            playerMap.localPosition = new Vector3(x, y, 0);
        }

        public Transform taskRoot;

        public void UpdateCurrentTask(string taskName)
        {
            HideAllTask();

            Transform task = taskRoot.Find(taskName);
            if (task != null)
            {
                task.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Task not found: " + taskName);
            }
        }

        private void HideAllTask()
        {
            foreach (Transform child in taskRoot)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}