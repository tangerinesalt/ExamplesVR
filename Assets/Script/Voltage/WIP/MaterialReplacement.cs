/*
功能：替换子对象的材质球
*/
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;


namespace WIP
{
    public class MaterialReplacement : MonoBehaviour
    {
        [Header("是否使用字符串匹配"), Space(10)]
        public bool usName = false;
        public string materialName;
        [Header("是否包含名称匹配"), Space(10)]
        public bool includeName = false;
        [Header("材质球预制体"), Space(10)]
        public Material materialPrefab;
        [ReadOnly]
        public Renderer[] renderers;
        private int num = 0;

        [Button("获取渲染器")]
        void GetRenderers()
        {
            renderers = GetComponentsInChildren<Renderer>();
            if (renderers == null || renderers.Length == 0)
            {
                Debug.LogWarning("没有找到渲染器");
                return;
            }
            Debug.Log("找到渲染器数量：" + renderers.Length);
        }
        [Button("替换材质球")]
        void ReplaceMaterial()
        {
            num = 0;
            if (materialPrefab == null)
            {
                Debug.LogWarning("没有指定材质球");
                return;
            }
            if (renderers == null || renderers.Length == 0)
            {
                Debug.LogWarning("没有指定渲染器");
                return;
            }
            if (usName && string.IsNullOrEmpty(materialName))
            {
                Debug.LogWarning("没有指定材质球名称");
                return;
            }
            string _materialName;
            if (usName)
                _materialName = materialName;
            else
                _materialName = materialPrefab.name;

            foreach (Renderer renderer in renderers)
            {
                if (renderer != null)
                {
                    Material[] materials = renderer.sharedMaterials;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        //验证名称是否匹配
                        bool isMatch = false;
                        if (includeName)
                        {
                            if (materials[i].name.Contains(_materialName))
                                isMatch = true;
                            else isMatch = false;
                        }
                        else
                        {
                            if (materials[i].name == _materialName)
                                isMatch = true;
                            else isMatch = false;
                        }

                        if (isMatch)
                        {
                            // Debug.Log("找到材质球，进行替换");
                            num++;
                            materials[i] = materialPrefab;
                        }
                    }
                    renderer.sharedMaterials = materials;
                }
            }
            Debug.Log("替换材质球完成，数量：" + num, materialPrefab);
        }
    }
}
