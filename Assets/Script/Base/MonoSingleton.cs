/****************************************************
    功能：继承Mono的单例
    作者：ZH
    创建日期：#2025/01/07#
    修改人：ZH
    修改日期：#2025/01/14#
    修改内容：
*****************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Voltage
{
    public class MonoSingleton<T> : MonoBehaviour
        where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(T)) as T;
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        _instance = (T)obj.AddComponent(typeof(T));
                        obj.name = typeof(T).Name;
                    }
                }
                return _instance;
            }
        }
    }
}