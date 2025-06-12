/****************************************************
	功能：游戏物体管理器
    作者：ZH
    创建日期：#2025/01/09#
    修改人：ZH
    修改日期：#2025/02/28#
    修改内容：
        1.增加3D物体管理
        2.增加错误日志提示
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Voltage
{
    public class GOManager : MonoSingleton<GOManager>
    {
        private GameObject canvas2DRoot;
        private GameObject canvas3DRoot;

        private string pathUI2D = "GO/UI2D/";
        private string pathUI3D = "GO/UI3D/";
        private string pathGO3D = "GO/GO3D/";

        private Dictionary<string, GOBase> dicGOs = new Dictionary<string, GOBase>();

        public void Init()
        {
            if (GlobalDataManager.Instance.isInit2DCanvas)
            {
                InitializeCanvas(ref canvas2DRoot, "Canvas2D");
            }

            if (GlobalDataManager.Instance.isInit3DCanvas)
            {
                InitializeCanvas(ref canvas3DRoot, "Canvas3D");
            }
        }

        private void InitializeCanvas(ref GameObject canvasRoot, string canvasName)
        {
            canvasRoot = new GameObject(canvasName);
            DontDestroyOnLoad(canvasRoot);
            canvasRoot.transform.localPosition = Vector3.zero;
            canvasRoot.transform.localEulerAngles = Vector3.zero;
        }

        /// <summary>
        /// 显示游戏物体
        /// </summary>
        /// <param name="name"></param>
        /// <param name="eGOType"></param>
        /// <returns></returns>
        public GOBase ShowGO(string name, EGOType eGOType = EGOType.GO3D)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("游戏物体名称不能为空！");
                return null;
            }

            Transform parentTF = null;
            string path = GetPathForEGOType(name, eGOType, ref parentTF);
            if (path == null) return null;
            
            if (dicGOs.TryGetValue(name, out GOBase goBase))
            {
                goBase.gameObject.SetActive(true);
                return goBase;
            }

            goBase = InstantiateGO(path, name, parentTF);
            if (goBase == null)
            {
                Debug.LogError($"无法实例化游戏物体: {name}");
            }

            return goBase;
        }

        private string GetPathForEGOType(string name, EGOType eGOType, ref Transform parentTF)
        {
            switch (eGOType)
            {
                case EGOType.UI2D:
                    if (!canvas2DRoot)
                    {
                        Debug.LogError("2D Canvas未初始化！");
                        return null;
                    }
                    parentTF = canvas2DRoot.transform;
                    return pathUI2D + name;

                case EGOType.UI3D:
                    if (!canvas3DRoot)
                    {
                        Debug.LogError("3D Canvas未初始化！");
                        return null;
                    }
                    parentTF = canvas3DRoot.transform;
                    return pathUI3D + name;

                case EGOType.GO3D:
                    parentTF = null;
                    return pathGO3D + name;

                default:
                    Debug.LogError($"未知的游戏物体类型: {eGOType}");
                    return null;
            }
        }

        private GOBase InstantiateGO(string path, string name, Transform parentTF)
        {
            GameObject go = ResManager.Instance.LoadPrefab(path);
            go.name = name;
            go.transform.SetParent(parentTF, false);
            Type type = Type.GetType("Voltage." + name);
            GOBase goBase = (GOBase)go.GetOrAddComponent(type);
            dicGOs.Add(name, goBase);

            return goBase;
        }

        /// <summary>
        /// 显示2DUI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GOBase ShowUI2D(string name) => ShowGO(name, EGOType.UI2D);

        /// <summary>
        /// 显示3DUI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GOBase ShowUI3D(string name) => ShowGO(name, EGOType.UI3D);

        /// <summary>
        /// 显示3D游戏物体
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GOBase ShowGO3D(string name) => ShowGO(name, EGOType.GO3D);

        /// <summary>
        /// 隐藏游戏物体
        /// </summary>
        /// <param name="name"></param>
        public void HideGO(string name)
        {
            if (dicGOs.TryGetValue(name, out GOBase goBase))
            {
                goBase.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError($"dicGOs：{name} 不存在，请检查！");
            }
        }

        /// <summary>
        /// 隐藏所有物体
        /// </summary>
        /// <param name="eGOType"></param>
        public void HideAll(EGOType eGOType = EGOType.All)
        {
            foreach (var item in dicGOs)
            {
                if (eGOType == EGOType.All || item.Value.eGOType == eGOType)
                {
                    item.Value.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 移除游戏物体
        /// </summary>
        /// <param name="name"></param>
        public void RemoveGO(string name)
        {
            if (dicGOs.TryGetValue(name, out GOBase goBase))
            {
                Destroy(goBase.gameObject);
                dicGOs.Remove(name);
            }
            else
            {
                Debug.LogError($"dicGOs：{name} 不存在，请检查！");
            }
        }

        /// <summary>
        /// 移除所有物体
        /// </summary>
        public void RemoveAll(EGOType eGOType = EGOType.All)
        {
            var keysToRemove = new List<string>();

            foreach (var item in dicGOs)
            {
                if (eGOType == EGOType.All || item.Value.eGOType == eGOType)
                {
                    Destroy(item.Value.gameObject);
                    keysToRemove.Add(item.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                dicGOs.Remove(key);
            }
        }

        /// <summary>
        /// 添加Button监听器
        /// </summary>
        /// <param name="goBase"></param>
        /// <param name="name"></param>
        /// <param name="onclick"></param>
        public void AddButtonListener(GOBase goBase, string name, UnityAction onclick)
        {
            if (goBase.eGOType != EGOType.UI2D && goBase.eGOType != EGOType.UI3D)
            {
                Debug.LogError($"UIManager AddButtonListener: {name} 不是2D或3D UI类型，无法添加Button监听器！");
                return;
            }

            if (!goBase.view.TryGetValue(name, out GameObject viewGO))
            {
                Debug.LogError($"UIManager AddButtonListener: {name} 在视图中未找到！");
                return;
            }

            Button bt = viewGO.GetComponent<Button>();
            if (bt == null)
            {
                Debug.LogError($"UIManager AddButtonListener: {name} 不是Button组件！");
                return;
            }

            bt.onClick.AddListener(onclick);
        }

        /// <summary>
        /// 添加Slider监听器
        /// </summary>
        /// <param name="goBase"></param>
        /// <param name="name"></param>
        /// <param name="onValueChanged"></param>
        public void AddSliderListener(GOBase goBase, string name, UnityAction<float> onValueChanged)
        {
            if (goBase.eGOType != EGOType.UI2D && goBase.eGOType != EGOType.UI3D)
            {
                Debug.LogError($"UIManager AddSliderListener: {name} 不是2D或3D UI类型，无法添加Slider监听器！");
                return;
            }

            if (!goBase.view.TryGetValue(name, out GameObject viewGO))
            {
                Debug.LogError($"UIManager AddSliderListener: {name} 在视图中未找到！");
                return;
            }

            Slider slider = viewGO.GetComponent<Slider>();
            if (slider == null)
            {
                Debug.LogError($"UIManager AddSliderListener: {name} 不是Slider组件！");
                return;
            }

            slider.onValueChanged.AddListener(onValueChanged);
        }
    }
}