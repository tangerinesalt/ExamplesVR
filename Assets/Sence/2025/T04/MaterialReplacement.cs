using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;


namespace WIP
{
    public class MaterialReplacement : MonoBehaviour
    {
        public string materialName = "Default-Material (Instance)";
        public Material materialPrefab;
        [ReadOnly]
        public Renderer[] renderers;

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
            foreach (Renderer renderer in renderers)
            {
                if (renderer != null)
                {
                    Material[] materials = renderer.sharedMaterials;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        if (materials[i].name == materialName)
                        {
                            Debug.Log("找到材质球，进行替换");
                            
                            materials[i] = materialPrefab;
                            // materials[i] = Instantiate(materialPrefab);
                        }
                    }
                    renderer.sharedMaterials = materials;
                }
            }
        }
    }
}
