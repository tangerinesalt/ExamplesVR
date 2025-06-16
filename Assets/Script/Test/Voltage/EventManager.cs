
using UnityEngine;
using System;
using Unity.XR.CoreUtils;
using Unity.Mathematics;

public class EventManager 
{
    private static EventManager m_eventManager;
    private EventManager(){}
    public static EventManager Instance
    {
        get
        {
            if (m_eventManager == null)
            {
                m_eventManager = new EventManager();
            }
            return m_eventManager;
        }
    }

    public event Action onChangeSence;
    public void OnChangeSence()
    {
        onChangeSence?.Invoke();
    }

    
}

