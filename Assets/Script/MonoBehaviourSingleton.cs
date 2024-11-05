using System;
using UnityEngine;

    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
    {
        private static T _instance = null;
        
        public static T Instance
        {
            get => _instance;
        }

        protected virtual string _monoBehaviourName => "SingletonBase";
        
        protected bool _overSingle = false;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Debug.LogWarning($"More than 1 {_monoBehaviourName}!");
                _overSingle = true;
                Destroy(this);
            }
        }
    }

