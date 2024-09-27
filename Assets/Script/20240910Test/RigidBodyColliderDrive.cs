
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit;


/// <summary>
/// 修改自CharacterControllerDriver，根据VR相机的高度来设置VR模块主要CapsuleCollider的高度和位置
/// </summary>
public class RigidbodyColliderDrive : MonoBehaviour
{
    [SerializeField] private Rigidbody m_Rigidbody;
    [SerializeField]
    [Tooltip("The Locomotion Provider object to listen to.")]
    private LocomotionProvider m_LocomotionProvider;

    [SerializeField]
    [Tooltip("The minimum height of the CapsuleCollider that will be set by this behavior.")]
    private float m_MinHeight;

    [SerializeField]
    [Tooltip("The maximum height of the CapsuleCollider that will be set by this behavior.")]
    private float m_MaxHeight = float.PositiveInfinity;

    private XROrigin m_XROrigin;

    //private CharacterController m_CharacterController;

    [SerializeField]private CapsuleCollider m_capsuleCollider;

    public LocomotionProvider locomotionProvider
    {
        get
        {
            return m_LocomotionProvider;
        }
        set
        {
            Unsubscribe(m_LocomotionProvider);
            m_LocomotionProvider = value;
            Subscribe(m_LocomotionProvider);
            //SetupCharacterController();
            UpdateRigidBodyCollider();
        }
    }

    public float minHeight
    {
        get
        {
            return m_MinHeight;
        }
        set
        {
            m_MinHeight = value;
        }
    }

    public float maxHeight
    {
        get
        {
            return m_MaxHeight;
        }
        set
        {
            m_MaxHeight = value;
        }
    }

    protected XROrigin xrOrigin => m_XROrigin;

    //protected CharacterController characterController => m_CharacterController;
    protected CapsuleCollider capsuleCollider => m_capsuleCollider;


    // [Obsolete("xrRig has been deprecated. Use xrOrigin instead.")]
    // protected XRRig xrRig => xrOrigin as XRRig;

    protected void Awake()
    {
        if (!(m_LocomotionProvider == null))
        {
            return;
        }

        m_LocomotionProvider = GetComponent<ContinuousMoveProviderBase>();
        if (m_LocomotionProvider == null)
        {

            Debug.LogWarning("without the locomotion events of a Locomotion Provider. Set Locomotion Provider or ensure a Continuous Move Provider component is in your scene.", this);

        }
        try
        {
            m_Rigidbody = this.GetComponent<Rigidbody>();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            if (m_Rigidbody == null)
            {
                Debug.LogError("Please Add Rigidbody Component:Open \"m_EnableMoveWithRigidbod\",but None Rigidbody Component");
            }
        }
        
    }

    protected void OnEnable()
    {
        Subscribe(m_LocomotionProvider);
    }

    protected void OnDisable()
    {
        Unsubscribe(m_LocomotionProvider);
    }

    protected void Start()
    {
        m_XROrigin = m_LocomotionProvider.system.xrOrigin;
        if (m_XROrigin == null)
        {
            Debug.LogError("RigidBodyColliderDrive lose m_XROrigin", this);
        }
        
        m_capsuleCollider = this.GetComponent<CapsuleCollider>();
        if (m_capsuleCollider == null)
        {
            Debug.LogError("RigidBodyColliderDrive lose m_capsuleCollider");
            
        }
        
        UpdateRigidBodyCollider();
        
    }
    void Update()
    {
        UpdateRigidBodyCollider();
    }

    protected virtual void UpdateRigidBodyCollider()
    {
       
        if (!(m_XROrigin == null) && !(m_capsuleCollider == null))
        {
            float num = Mathf.Clamp(m_XROrigin.CameraInOriginSpaceHeight, m_MinHeight, m_MaxHeight);
            Vector3 cameraInOriginSpacePos = m_XROrigin.CameraInOriginSpacePos;
            cameraInOriginSpacePos.y = num / 2f + 0.01f;
            m_capsuleCollider.height = num;
            m_capsuleCollider.center = cameraInOriginSpacePos;
            Debug.Log("UpdateRigidBodyCollider");
        }
        
    }

    private void Subscribe(LocomotionProvider provider)
    {
        if (provider != null)
        {
            provider.beginLocomotion += OnBeginLocomotion;
            provider.endLocomotion += OnEndLocomotion;
        }
    }

    private void Unsubscribe(LocomotionProvider provider)
    {
        if (provider != null)
        {
            provider.beginLocomotion -= OnBeginLocomotion;
            provider.endLocomotion -= OnEndLocomotion;
        }
    }

    

    private void OnBeginLocomotion(LocomotionSystem system)
    {
        UpdateRigidBodyCollider();
    }

    private void OnEndLocomotion(LocomotionSystem system)
    {
        UpdateRigidBodyCollider();
    }
}
