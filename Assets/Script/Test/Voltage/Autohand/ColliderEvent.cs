using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 碰撞触发器事件
/// </summary>
public class ColliderEvent : MonoBehaviour
{
    [Serializable]
    public class CollisionTriggerEvent : UnityEvent { }
    [SerializeField] private LayerMask m_TriggerLayer = 0;
    [SerializeField] private CollisionTriggerEvent m_OnTriggerEnter = null;
    [SerializeField] private CollisionTriggerEvent m_OnTriggerExit = null;
    [SerializeField] private CollisionTriggerEvent m_OncollideEnter = null;
    [SerializeField] private CollisionTriggerEvent m_OncollideExit = null;
    private bool m_AllowCollisionEvents = false;
    // Start is called before the first frame update
    void Start()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            m_AllowCollisionEvents = true;
        }
        else
        {
            Debug.LogWarning("No collider found on " + gameObject.name + "!", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_AllowCollisionEvents && (m_TriggerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            m_OnTriggerEnter?.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (m_AllowCollisionEvents && (m_TriggerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            m_OnTriggerExit?.Invoke();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (m_AllowCollisionEvents && (m_TriggerLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            m_OncollideEnter?.Invoke();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (m_AllowCollisionEvents && (m_TriggerLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            m_OncollideExit?.Invoke();
        }
    }
}
