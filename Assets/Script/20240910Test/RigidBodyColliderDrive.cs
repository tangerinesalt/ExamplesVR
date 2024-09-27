
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit;



public class RigidBodyColliderDrive : MonoBehaviour
{
    [SerializeField] private Rigidbody m_Rigidbody;
    [SerializeField]
    [Tooltip("The Locomotion Provider object to listen to.")]
    private LocomotionProvider m_LocomotionProvider;

    [SerializeField]
    [Tooltip("The minimum height of the character's capsule that will be set by this behavior.")]
    private float m_MinHeight;

    [SerializeField]
    [Tooltip("The maximum height of the character's capsule that will be set by this behavior.")]
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


    [Obsolete("xrRig has been deprecated. Use xrOrigin instead.")]
    protected XRRig xrRig => xrOrigin as XRRig;

    protected void Awake()
    {
        if (!(m_LocomotionProvider == null))
        {
            return;
        }

        m_LocomotionProvider = GetComponent<ContinuousMoveProviderBase>();
        if (m_LocomotionProvider == null)
        {

            Debug.LogWarning("Unable to drive properties of the Character Controller without the locomotion events of a Locomotion Provider. Set Locomotion Provider or ensure a Continuous Move Provider component is in your scene.", this);

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
                //Debug.Log("Open \"m_EnableMoveWithRigidbod\",but None Rigidbody Component");
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
        //m_CharacterController = ((m_XROrigin != null) ? m_XROrigin.Origin.GetComponent<CharacterController>() : null);
        m_capsuleCollider = this.GetComponent<CapsuleCollider>();
        if (m_capsuleCollider == null)
        {
            Debug.LogError("RigidBodyColliderDrive lose m_capsuleCollider");
            
        }
        //SetupCharacterController();
        UpdateRigidBodyCollider();
        
    }
    void Update()
    {
        UpdateRigidBodyCollider();
    }

    protected virtual void UpdateRigidBodyCollider()
    {
        // if (!(m_XROrigin == null) && !(m_CharacterController == null))
        // {
        //     float num = Mathf.Clamp(m_XROrigin.CameraInOriginSpaceHeight, m_MinHeight, m_MaxHeight);
        //     Vector3 cameraInOriginSpacePos = m_XROrigin.CameraInOriginSpacePos;
        //     cameraInOriginSpacePos.y = num / 2f + m_CharacterController.skinWidth;
        //     m_CharacterController.height = num;
        //     m_CharacterController.center = cameraInOriginSpacePos;
        // }
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

    // private void SetupCharacterController()
    // {
    //     if (!(m_LocomotionProvider == null) && !(m_LocomotionProvider.system == null))
    //     {
    //         m_XROrigin = m_LocomotionProvider.system.xrOrigin;
    //         m_CharacterController = ((m_XROrigin != null) ? m_XROrigin.Origin.GetComponent<CharacterController>() : null);
    //         if (m_CharacterController == null && m_XROrigin != null)
    //         {
    //             Debug.LogError($"Could not get CharacterController on {m_XROrigin.Origin}, unable to drive properties." + $" Ensure there is a CharacterController on the \"Rig\" GameObject of {m_XROrigin}.", this);
    //         }
    //     }
    // }

    private void OnBeginLocomotion(LocomotionSystem system)
    {
        UpdateRigidBodyCollider();
    }

    private void OnEndLocomotion(LocomotionSystem system)
    {
        UpdateRigidBodyCollider();
    }
}
