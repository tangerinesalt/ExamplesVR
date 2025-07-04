/****************************************************
    功能：游戏入口基类
    作者：ZH
    创建日期：#2025/01/08#
    修改人：ZH
    修改日期：#2025/01/15#
    修改内容：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voltage
{
    [DisallowMultipleComponent]
    public class GameEntry<T> : MonoSingleton<T> 
        where T : Component
    {
        /// <summary>
        /// 根节点
        /// </summary>
        protected  GameObject root;

        protected virtual void Awake()
        {
            root = GameObject.Find("Root");
            if (root == null)
            {
                UtilsVoltage.DebugLog(Color.red, "Root节点不存在！");
            }
            else
            {
                UtilsVoltage.DebugLog(Color.green, "初始化根节点！");
                DontDestroyOnLoad(root);

                GOManager.Instance.Init();
                AudioManager.Instance.Init();
                SceneServiceManager.Instance.Init();
            }

            //root.AddComponent<ShowFPS>();
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            
        }

        protected virtual void OnApplicationQuit()
        {
            UtilsVoltage.DebugLog(Color.green, "游戏结束！");
            UtilsVoltage.Unload_Collect();
        }
    }
}