/****************************************************
    功能：单组区域触发事件类
    作者：ZH
    创建日期：#2025/04/15#
    修改内容：
        1.新增高模型线显示隐藏    2025/04/15 ZH
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace Voltage
{
    public class SingleGroupManager : MonoBehaviour
    {
        private List<SingleGroupData> allGroup = new List<SingleGroupData>();

        private void Awake()
        {
            GetAllGroup();
            GetNearGroup();
            GetOtherGroup();
        }

        private void GetAllGroup()
        {
            allGroup.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                SingleGroupData child = transform.GetChild(i).GetComponent<SingleGroupData>();
                allGroup.Add(child);
            }
        }

        private void GetNearGroup()
        {
            for (int i = 0; i < allGroup.Count; i++)
            {
                SingleGroupData singleGroup = allGroup[i];
                singleGroup.lastGroup = i > 0 ? allGroup[i - 1] : null;
                singleGroup.nextGroup = i < allGroup.Count - 1 ? allGroup[i + 1] : null;
            }
        }

        private void GetOtherGroup()
        {
            for (int i = 0; i < allGroup.Count; i++)
            {
                SingleGroupData group = allGroup[i];
                group.otherGroups.Clear();

                for (int j = 0; j < allGroup.Count; j++)
                {
                    if (i != j && group.lastGroup != allGroup[j] && group.nextGroup != allGroup[j])
                    {
                        group.otherGroups.Add(allGroup[j]);
                    }
                }
            }
        }
    }
}