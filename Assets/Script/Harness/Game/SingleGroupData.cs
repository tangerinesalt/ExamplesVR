/****************************************************
    功能：单组模型数据类
    作者：ZH
    创建日期：#2025/04/17#
    修改内容：
        1. 新增模型类型和显隐方法    2025/04/17 ZH
        2. 代码结构优化            2025/04/18 ZH
*****************************************************/

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Voltage
{
    public class SingleGroupData : MonoBehaviour
    {
        [Serializable]
        private enum TriggerType
        {
            Mode3In,
            Mode3Out,
            Mode2In,
            Mode2Out
        }

        [Header("模型列表")]
        [SerializeField] private List<GameObject> mode3 = new List<GameObject>();
        [SerializeField] private List<GameObject> mode2 = new List<GameObject>();
        [SerializeField] private List<GameObject> nearModel3 = new List<GameObject>();
        [SerializeField] private List<GameObject> nearModel2 = new List<GameObject>();
        [SerializeField] private List<GameObject> farModel3 = new List<GameObject>();
        [SerializeField] private List<GameObject> farModel2 = new List<GameObject>();
        [SerializeField] private List<GameObject> modeCable = new List<GameObject>();
        [SerializeField] private List<GameObject> modeCableOut = new List<GameObject>();

        [Header("相邻组引用")]
        [HideInInspector]
        public SingleGroupData lastGroup;
        [HideInInspector]
        public SingleGroupData nextGroup;

        [Header("其他组引用")]
        [HideInInspector]
        public List<SingleGroupData> otherGroups = new List<SingleGroupData>();

        [Header("触发器引用")]
        [SerializeField] private TriggerEvent _3InTrigger;
        [SerializeField] private TriggerEvent _3OutTrigger;
        [SerializeField] private TriggerEvent _2InTrigger;
        [SerializeField] private TriggerEvent _2OutTrigger;

        private void Start()
        {
            BindTriggerEvents();
        }

        private void OnDestroy()
        {
            UnbindTriggerEvents();
        }

        private void BindTriggerEvents()
        {
            _3InTrigger.enterTriggerEvent.AddListener(() => HandleTrigger(TriggerType.Mode3In));
            _3OutTrigger.enterTriggerEvent.AddListener(() => HandleTrigger(TriggerType.Mode3Out));
            _2InTrigger.enterTriggerEvent.AddListener(() => HandleTrigger(TriggerType.Mode2In));
            _2OutTrigger.enterTriggerEvent.AddListener(() => HandleTrigger(TriggerType.Mode2Out));
        }

        private void UnbindTriggerEvents()
        {
            _3InTrigger.enterTriggerEvent.RemoveAllListeners();
            _3OutTrigger.enterTriggerEvent.RemoveAllListeners();
            _2InTrigger.enterTriggerEvent.RemoveAllListeners();
            _2OutTrigger.enterTriggerEvent.RemoveAllListeners();
        }

        private void HandleTrigger(TriggerType triggerType)
        {
            // 确定模式类型和触发方向
            bool is3D = triggerType.ToString().Contains("3");
            bool isEnter = triggerType.ToString().Contains("In");

            // 处理其他组的可见性
            UpdateOtherGroupsVisibility(is3D, isEnter);

            // 处理相邻组的可见性
            UpdateAdjacentGroupsVisibility(is3D, isEnter);

            // 更新当前组可见性
            UpdateCurrentGroupVisibility(is3D);
        }

        private void UpdateOtherGroupsVisibility(bool is3D, bool isEnter)
        {
            foreach (var group in otherGroups)
            {
                var farModels = is3D ? group.farModel3 : group.farModel2;
                var nearModels = is3D ? group.nearModel3 : group.nearModel2;

                SetVisibility(farModels, false);  // 所有情况都隐藏远模型
                SetVisibility(nearModels, !isEnter); // 进入时隐藏近模型，退出时显示

                if (PlayerManager.Instance.state == EInstallState.Uninstall)
                {
                    SetVisibility(group.modeCable, false);
                    SetVisibility(group.modeCableOut, false);
                }
                else if (PlayerManager.Instance.state == EInstallState.Install)
                {
                    SetVisibility(group.modeCable, false);
                    SetVisibility(group.modeCableOut, !isEnter);
                }
                
            }
        }

        private void UpdateAdjacentGroupsVisibility(bool is3D, bool isEnter)
        {
            void UpdateGroup(SingleGroupData group)
            {
                if (!group) return;

                SetVisibility(group.mode2, !is3D);
                SetVisibility(group.mode3, is3D);

                if (PlayerManager.Instance.state == EInstallState.Uninstall)
                {
                    SetVisibility(group.modeCable, false);
                    SetVisibility(group.modeCableOut, false);
                }
                else if (PlayerManager.Instance.state == EInstallState.Install)
                {
                    SetVisibility(group.modeCable, true);

                    if (is3D)
                    {
                        SetVisibility(group.modeCableOut, !isEnter);
                    }
                    else
                    {
                        SetVisibility(group.modeCableOut, false);
                    }
                }
            }

            UpdateGroup(lastGroup);
            UpdateGroup(nextGroup);
        }

        private void UpdateCurrentGroupVisibility(bool is3D)
        {
            SetVisibility(mode2, true);
            SetVisibility(mode3, true);

            if (PlayerManager.Instance.state == EInstallState.Uninstall)
            {
                SetVisibility(modeCable, false);
                SetVisibility(modeCableOut, false);
            }
            else if (PlayerManager.Instance.state == EInstallState.Install)
            {
                SetVisibility(modeCable, true);

                if (is3D)
                {
                    SetVisibility(modeCableOut, true);
                }
                else
                {
                    SetVisibility(modeCableOut, false);
                }
            }
        }

        private void SetVisibility(List<GameObject> targets, bool visible)
        {
            if (targets == null) return;

            foreach (var go in targets)
            {
                if (go) go.SetActive(visible);
            }
        }

    }
}