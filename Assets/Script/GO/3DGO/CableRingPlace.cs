/****************************************************
    功能：Whip(线圈)线缆放置
    作者：ZZQ
    创建日期：#2025/03/27#
    修改内容：1.代码优化 ZZQ #2025/04/01#
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using Autohand;
using UnityEngine;

namespace Voltage
{
    public class CableRingPlace : GO3DBase
    {
        protected override void Awake()
        {
            base.Awake();
        }
        void Start()
        {
            InitObject();
            InitInteraction();
            InitFlicker();
        }
        private void InitObject()
        {
            view["String01/Target"].SetActive(false);
            view["String02/Target"].SetActive(false);
            view["String01/Canvas/OperationTie"].SetActive(false);
            view["String02/Canvas/OperationTie"].SetActive(false);
        }
        private void InitInteraction()
        {
            HarnessObiOperate obi = (HarnessObiOperate)GOManager.Instance.ShowGO3D("HarnessObiOperate");

            view["String01/MoveMark"].GetComponent<TriggerEvent>().enterTriggerEvent.AddListener(
                () =>
                {
                    PlayerManager.Instance.UpdateCurrentTask("placewhip1");
                    view["String01/MoveMark"].SetActive(false);
                    view["String01/Target"].SetActive(true);
                    view["String01/Canvas/OperationTie"].SetActive(true);
                    //关闭寻路
                    view["Nav/Box01"].SetActive(false);
                    PathFinding.Instance.ToggleNavigation(false);
                    
                });
            view["String02/MoveMark"].GetComponent<TriggerEvent>().enterTriggerEvent.AddListener(
                () =>
                {
                    PlayerManager.Instance.UpdateCurrentTask("placewhip1");
                    view["String02/MoveMark"].SetActive(false);
                    view["String02/Target"].SetActive(true);
                    view["String02/Canvas/OperationTie"].SetActive(true);
                    //关闭寻路
                    view["Nav/Box02"].SetActive(false);
                    PathFinding.Instance.ToggleNavigation(false);
                });
            view["String01/CableRing/CableRing_Target"].GetComponent<Plug_Grabbable>().m_afterConnection.AddListener(
                () =>
                {
                    view["String01/CableRing/CableRing_Target"].GetComponent<MaterialFlicker>().SetRendererActive(false);
                    Debug.Log("放置线圈_String01完成");
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    view["String01/Canvas/OperationTie"].SetActive(false);
                    obi.StartWhipPlace01();
                }
            );
            view["String02/CableRing/CableRing_Target"].GetComponent<Plug_Grabbable>().m_afterConnection.AddListener(
                () =>
                {
                    view["String02/CableRing/CableRing_Target"].GetComponent<MaterialFlicker>().SetRendererActive(false);
                    Debug.Log("放置线圈_String02完成");
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    view["String02/Canvas/OperationTie"].SetActive(false);
                    obi.StartWhipPlace02();
                }
            );
        }
        private void InitFlicker()
        {
            view["String01/CableRing/CableRing_Target"].GetComponent<MaterialFlicker>().SetFlickerState(true);

            StepOnGrab("String01/CableRing/CableRing_Target");
            StepOnGrab("String02/CableRing/CableRing_Target");
            StepOnRelease("String01/CableRing/CableRing_Target");
            StepOnRelease("String02/CableRing/CableRing_Target");
        }
        public void StartCableRingPlace01()
        {
            Debug.Log("开始放置第一组线圈");
            view["String01"].SetActive(true);
            view["String02"].SetActive(false);
            view["String01/CableRing/CableRing_Target"].GetComponent<MaterialFlicker>().SetFlickerState(true);
            //寻路
            view["Nav/Box01"].SetActive(true);
            PathFinding.Instance.SetNavigationTarget(view["Nav/Box01"].transform);

        }
        /// <summary>
        /// 开始第二组线圈放置
        /// </summary>
        public void StartCableRingPlace02()
        {
            Debug.Log("开始放置第二组线圈");
            view["String01"].SetActive(false);
            view["String02"].SetActive(true);
            view["String02/CableRing/CableRing_Target"].GetComponent<MaterialFlicker>().SetFlickerState(true);
            //寻路
            view["Nav/Box02"].SetActive(true);
            PathFinding.Instance.SetNavigationTarget(view["Nav/Box02"].transform);
        }
        /// <summary>
        /// 抓取时禁用渲染
        /// </summary>
        /// <param name="objName">物体在预制体中的路径</param>
        private void StepOnGrab(string objName)
        {
            view[objName].GetComponent<Grabbable>().onGrab.AddListener(
                (Hand hand, Grabbable grabbable) =>
                {
                    view[objName].GetComponent<MaterialFlicker>().SetRendererActive(false);
                }
            );
        }
        /// <summary>
        /// 释放时启用发光
        /// </summary>
        /// <param name="objName">物体在预制体中的路径</param>
        private void StepOnRelease(string objName)
        {
            view[objName].GetComponent<Grabbable>().onRelease.AddListener(
                (Hand hand, Grabbable grabbable) =>
                {
                    view[objName].GetComponent<MaterialFlicker>().SetFlickerState(true);
                }
            );
        }
    }
}