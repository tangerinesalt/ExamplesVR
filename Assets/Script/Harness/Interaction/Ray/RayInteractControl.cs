/****************************************************
    功能：射线交互时（射线投射到UI上）改变抓取距离，防止误操作
    作者：ZZQ
    创建日期：#2025/02/20#
    修改人：ZZQ
    修改日期：#2025/03/12#
    修改内容：解决使用PlayerManager修改识别距离的冲突问题
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using NaughtyAttributes;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

namespace Voltage
{
    public class RayInteractControl : MonoBehaviour
    {
        [Header("messages")]
        [SerializeField, TextArea] private string m_Message = null;

        [Header("References")]
        [SerializeField] private XRRayInteractor m_RayInteractor = null;
        [SerializeField] private Hand m_Hand = null;

        [Header("Settings")]
        [Range(0.001f, 0.01f)]
        [SerializeField] private float m_ReachDistanceOnOverUI = 0.001f;

        [Header("Events")]
        [SerializeField] private UnityEvent m_OnOverUI = null;

        private bool m_isOverUI = false;
        private bool _firstOverUI = false;
        private float m_originalReachDistance;
        private float m_RecoveredReachDistance;

        void Awake()
        {
            m_isOverUI = false;
            _firstOverUI = false;

            if (m_Hand != null) m_originalReachDistance = m_Hand.reachDistance;
            m_RecoveredReachDistance = m_originalReachDistance;
        }
        void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            Application.quitting += OnApplicationQuit;
        }
        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Application.quitting -= OnApplicationQuit;
        }
        void Update()
        {
            if (m_RayInteractor == null || m_Hand == null || !m_RayInteractor.gameObject.activeInHierarchy)
            {
                m_Message = "please check RayInteractor or Hand component, and RayInteractor is not active";
                return;
            }

            if (m_RayInteractor.IsOverUIGameObject() && !m_isOverUI)
            {
                m_Hand.reachDistance = m_ReachDistanceOnOverUI;

                if (m_Hand != null)
                {
                    m_Message = "Ray Over UI";
                    m_Message += "\nHandReachDistance:" + m_Hand.reachDistance + $" ({m_Hand.name})";
                }
                m_OnOverUI?.Invoke();

                m_isOverUI = true;
                if (!_firstOverUI) _firstOverUI = true;
            }
            else if (!m_RayInteractor.IsOverUIGameObject() && _firstOverUI)
            {
                m_Hand.reachDistance = PlayerManager.Instance.DefaultReachDistance;

                if (m_Hand != null)
                {
                    m_Message = "Ray Out UI";
                    m_Message += "\nHandReachDistance:" + m_Hand.reachDistance + $" ({m_Hand.name})";
                }
                m_isOverUI = false;
            }
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            m_Hand.reachDistance = m_RecoveredReachDistance;
        }
        void OnApplicationQuit()
        {
            m_Hand.reachDistance = m_originalReachDistance;
        }
    }
}