using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Bhaptics.SDK2;
using Tangerine;

using UnityEngine.InputSystem;

public class XR_Grip : MonoBehaviour
{
    private XR_GripInteractiveObj m_InteractiveObj;
    private XR_GripInteractiveObj _ArchivedInteractiveObj;
    [SerializeField] private EHandType m_HandType = EHandType.Left;
    [SerializeField] public Transform m_GripPoint;
    [SerializeField] private InputActionReference m_InputAction = null;
    void Awake()
    {
        if (m_GripPoint == null)
        {
            m_GripPoint = transform;
        }
    }
    void Start()
    {
        if (m_InputAction != null)
        {
            m_InputAction.action.started += OnGripStart;
            m_InputAction.action.canceled += OnGripEnd;
        }
    }
    private void OnDestroy()
    {
        if (m_InputAction != null)
        {
            m_InputAction.action.started -= OnGripStart;
            m_InputAction.action.canceled -= OnGripEnd;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (m_InteractiveObj == null)
        {
            HapticManager.Instance.Haptic(m_HandType);
            switch (m_HandType)
            {
                case EHandType.Left:
                    BhapticsLibrary.Play(BhapticsEvent.TOUCH_CATCHABLE_LEFT);
                    break;
                case EHandType.Right:
                    BhapticsLibrary.Play(BhapticsEvent.TOUCH_CATCHABLE_RIGHT);
                    break;
            }
        }

        if (m_InteractiveObj == null)
        {
            if (_ArchivedInteractiveObj == null)
            {
                _ArchivedInteractiveObj = GetInteractiveObjForCollider(other);
                if (_ArchivedInteractiveObj == null)
                {
                    Debug.Log("No XR_GripInteractiveObj found in the collider");
                    return;
                }
            }
            m_InteractiveObj = _ArchivedInteractiveObj;
            // m_InteractiveObj = other.GetComponent<XR_GripInteractiveObj>();
            // if (m_InteractiveObj == null)
            // {
            //     m_InteractiveObj = other.GetComponentInParent<XR_GripInteractiveObj>();
            //     if (m_InteractiveObj == null)
            //         Debug.LogError("No XR_GripInteractiveObj found in the collider");
            // }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (m_InteractiveObj != null)
        {
            m_InteractiveObj = null;
        }
        if (_ArchivedInteractiveObj != null)
        {
            _ArchivedInteractiveObj = null;
        }
    }

    private void OnGripStart(InputAction.CallbackContext ctx)
    {
        //Debug.Log("Grip Start");
        if (m_InteractiveObj == null) 
        {
            if (_ArchivedInteractiveObj == null) return;
            m_InteractiveObj = _ArchivedInteractiveObj;
        }
        m_InteractiveObj.GripStart(this);

    }
    private void OnGripEnd(InputAction.CallbackContext ctx)
    {
        if (m_InteractiveObj == null) return;
        m_InteractiveObj.GripEnd(this);
        m_InteractiveObj = null;
    }
    public void GiveUpGrabbing(XR_GripInteractiveObj xr_GripInteractiveObj)
    {
        if (xr_GripInteractiveObj == m_InteractiveObj)
        {
            if (m_InteractiveObj == null) return;
            m_InteractiveObj = null;
        }
    }
    #region tools
    private XR_GripInteractiveObj GetInteractiveObjForCollider(Collider collider)
    {
        if (collider.GetComponent<XR_GripInteractiveObj>() != null)
        {
            return collider.GetComponent<XR_GripInteractiveObj>();
        }
        else if (collider.GetComponentInParent<XR_GripInteractiveObj>() != null)
        {
            
            return collider.GetComponentInParent<XR_GripInteractiveObj>();
        }
        else
        {
            return null;
        }
    }
    #endregion
}
