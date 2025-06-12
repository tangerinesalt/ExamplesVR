/****************************************************
    功能：3D游戏对象基类
    作者：ZH
    创建日期：#2025/01/09#
    修改内容：
        1.增加了延迟调用方法    2025/01/10 ZH
        2.增加了非UI物体基类3D游戏对象基类，继承自GOBase    2025/02/10 ZH
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Voltage
{
    public class GOBase : MonoBehaviour
    {
        public Dictionary<string, GameObject> view = new Dictionary<string, GameObject>();
        public EGOType eGOType;

        protected virtual void Awake()
        {
            LoadAllObject(gameObject, "");
        }

        /// <summary>
        /// 递归加载子对象
        /// </summary>
        /// <param name="root"></param>
        /// <param name="path"></param>
        private void LoadAllObject(GameObject root, string path)
        {
            foreach (Transform tf in root.transform)
            {
                if (view.ContainsKey(path + tf.gameObject.name))
                {
                    Debug.LogWarning("Warning object is exist:" + path + tf.gameObject.name + "!");
                    continue;
                }
                view.Add(path + tf.gameObject.name, tf.gameObject);
                LoadAllObject(tf.gameObject, path + tf.gameObject.name + "/");
            }
        }

        /// <summary>
        /// 延迟调用指定的方法
        /// </summary>
        /// <param name="delayTime"></param>
        /// <param name="unityAction"></param>
        protected void DelayFunc(float delayTime, UnityAction unityAction)
        {
            StartCoroutine(DelayAction(delayTime, unityAction));
        }

        /// <summary>
        /// 协程实现延迟调用
        /// </summary>
        /// <param name="delayTime"></param>
        /// <param name="unityAction"></param>
        /// <returns></returns>
        private IEnumerator DelayAction(float delayTime, UnityAction unityAction)
        {
            yield return new WaitForSeconds(delayTime);
            unityAction?.Invoke();
        }
    }

    /// <summary>
    /// UIBase2D类继承自GOBase，用于处理2DUI
    /// </summary>
    public class UI2DBase : GOBase
    {
        protected override void Awake()
        {
            base.Awake();
            eGOType = EGOType.UI2D;
        }
    }

    /// <summary>
    /// UIBase3D类继承自GOBase，用于处理3DUI
    /// </summary>
    public class UI3DBase : GOBase
    {
        protected override void Awake()
        {
            base.Awake();
            eGOType = EGOType.UI3D;
        }
    }

    /// <summary>
    /// GO3D类继承自GOBase，用于处理3D游戏对象
    /// </summary>
    public class GO3DBase : GOBase
    {
        protected override void Awake()
        {
            base.Awake();
            eGOType = EGOType.GO3D;
        }
    }
}