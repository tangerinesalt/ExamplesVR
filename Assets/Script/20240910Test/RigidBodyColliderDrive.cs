
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Mathematics;
using Tangerine;


/// <summary>
/// 修改自CharacterControllerDriver，根据VR相机的高度来设置VR模块主要CapsuleCollider的高度和位置
/// </summary>
public class RigidbodyColliderDrive : MonoBehaviour
{
    [SerializeField] private XROrigin m_XROrigin;
    [SerializeField] private Rigidbody m_Rigidbody;

    [SerializeField]
    [Tooltip("The minimum height of the CapsuleCollider that will be set by this behavior.")]
    private float m_MinHeight;

    [SerializeField]
    [Tooltip("The maximum height of the CapsuleCollider that will be set by this behavior.")]
    private float m_MaxHeight = float.PositiveInfinity;
    [SerializeField] private CapsuleCollider m_capsuleCollider;
    //复位功能
    private bool RecognizingHeadPiercings = false;
    private bool m_IsHeadPiercing = false;
    private VR_AuxiliaryPositioner auxPositioner = null;
    private VRMovementBase vrMovement = null;


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

    protected CapsuleCollider capsuleCollider => m_capsuleCollider;



    protected void Awake()
    {

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

    // 获取VR、碰撞体组件
    protected void Start()
    {


        if (m_XROrigin == null)
        {
            try
            {
                m_XROrigin = this.transform.parent.GetComponent<XROrigin>();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            if (m_XROrigin == null)
            {
                Debug.LogError($"RigidBodyColliderDrive({this.name}) lose m_XROrigin", this);
            }
        }

        m_capsuleCollider = this.GetComponent<CapsuleCollider>();
        if (m_capsuleCollider == null)
        {
            Debug.LogError($"RigidBodyColliderDrive({this.name}) lose m_capsuleCollider");

        }

        StartCoroutine(RecognizeHeadPiercing());
        UpdateRigidBodyCollider();
    }

    //复位延时判断
    private IEnumerator RecognizeHeadPiercing()
    {
        yield return new WaitForSeconds(1f);
        RecognizingHeadPiercings = true;
        yield return null;
    }
    void Update()
    {
        UpdateRigidBodyCollider();//更新碰撞体高度
        Reset();//复位功能
    }
    void OnDisable()
    {
        StopCoroutine(RecognizeHeadPiercing());
    }

    //更新碰撞体高度
    protected virtual void UpdateRigidBodyCollider()
    {

        if (!(m_XROrigin == null) && !(m_capsuleCollider == null))
        {
            //更新计算
            float num = Mathf.Clamp(m_XROrigin.CameraInOriginSpaceHeight, m_MinHeight, m_MaxHeight);
            Vector3 cameraInOriginSpacePos = m_XROrigin.CameraInOriginSpacePos;
            cameraInOriginSpacePos.y = num / 2f + 0.01f;
            m_capsuleCollider.height = num;
            m_capsuleCollider.center = cameraInOriginSpacePos;

        }
    }

    //复位功能
    protected virtual void Reset()
    {
         if (!(m_XROrigin == null) && !(m_capsuleCollider == null))
         {
             float cameraHeightInVirtualSpace = m_XROrigin.Camera.transform.position.y;
            float cameraHeightInOriginSpace = m_XROrigin.CameraInOriginSpaceHeight;
            //Debug.Log("The Height displacement:"+math.abs(cameraHeightInOriginSpace-cameraHeightInVirtualSpace).ToString("F1"));
            if (RecognizingHeadPiercings && math.abs(cameraHeightInOriginSpace - cameraHeightInVirtualSpace) > 0.1f)
            {
                m_IsHeadPiercing = true;
                Vector3 teleportionLocation = Vector3.zero;
                teleportionLocation = GetTeleportationLocation();
                if (m_IsHeadPiercing)
                {
                    Teleportation(teleportionLocation);
                }
            }
            else
            {
                m_IsHeadPiercing = false;
            }
         }
    }
    //获取复位位置
    public Vector3 GetTeleportationLocation()
    {
        if (auxPositioner == null)
        {
            //组件获取,需要移植到start
            auxPositioner = GetComponentsInChildren<VR_AuxiliaryPositioner>()[0];
            if (auxPositioner == null)
            {
                Debug.LogError($"RigidbodyColliderDrive({this.name}) No VR_AuxiliaryPositioner Component");
            }
            m_IsHeadPiercing = false;
            return Vector3.zero;
        }
        else
        {
            Vector3 teleportationLocation = auxPositioner.GetTeleportationLocation();
            return teleportationLocation;
        }

    }

    //复位操作
    private void Teleportation(Vector3 _position)
    {
        if (vrMovement == null)
        {
             //组件获取,需要移植到start
            vrMovement = GetComponent<VRMovementBase>();

            if (vrMovement == null)
            {
                Debug.LogError($"RigidbodyColliderDrive({this.name}) No VRMovementBase Component");
                return;
            }
        }
        vrMovement.Teleportation(_position);
    }


}
