/****************************************************
    功能：自动命名工具（用后即删）
    作者：ZZQ
    创建日期：#2025/03/28#
    修改内容：#2025/03/28#
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Voltage
{
    public class AutomaticNaming : MonoBehaviour
    {
        public string NameStart;
        public Transform parent;
        private string[] oldNames;
        [Button("Generate Name")]
        public void GenerateName()
        {
            oldNames = new string[parent.childCount];
            foreach (Transform child in parent)
            {
                oldNames[child.GetSiblingIndex()] = child.name;
                child.name = NameStart + "" + (child.GetSiblingIndex()+1);
            }
        }
        [Button("Recover Names")]
        public void RecoverNames()
        {
            foreach (Transform child in parent)
            {
                child.name = oldNames[child.GetSiblingIndex()];
            }
        }
    }
}