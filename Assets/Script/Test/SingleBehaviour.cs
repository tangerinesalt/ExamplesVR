using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public class SingleBehaviour<T> : MonoBehaviour
    where T :MonoBehaviour
{
    private static T _instance = null;

    public static T Instance
    {
        get => _instance;
    }
    protected bool _overSingle = false;

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else
        {
            Type currentType = this.GetType();
            string className = currentType.FullName;
            Debug.LogWarning($"More than 1 {className}!");
            _overSingle = true;
            Destroy(this);
        }
    }


}
