using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.XR.CoreUtils;
using Unity.Mathematics;
using Tangerine;
using UnityEngine.UI;
using UnityEngine.Profiling;


/// <summary>
/// 修改自CharacterControllerDriver，根据VR相机的高度来设置VR模块主要CapsuleCollider的高度和位置
/// </summary>
/// 
namespace Tangerine
{
    public class RigidbodyColliderDrive : MonoBehaviour
    {
        [Header("Collider Settings")]
        [SerializeField] private CapsuleCollider m_capsuleCollider;
        [SerializeField] private XROrigin m_XROrigin;

        [SerializeField]
        [Tooltip("The minimum height of the CapsuleCollider that will be set by this behavior.")]
        private float m_MinHeight;

        [SerializeField]
        [Tooltip("The maximum height of the CapsuleCollider that will be set by this behavior.")]
        private float m_MaxHeight = float.PositiveInfinity;
        //复位功能
        [Header("Reset function")]
        [SerializeField] private bool m_EnableReset = false;
        [SerializeField] private Mask m_ResetMask = default;

        [SerializeField] private Rigidbody m_Rigidbody;
        private bool RecognizingHeadPiercings = false;
        private bool m_IsHeadPiercing = false;
        
        public event Action onReset;
        private VR_AuxiliaryPositioner m_auxPositioner = null;
        private VRMovementBase vrMovement = null;
        private RaycastHit _raycastHit;
        private bool m_UpdateLimiter=true;

        #region unity Cycle
        // 获取VR、碰撞体组件
        protected void Start()
        {
            bool canReset = true;
            m_UpdateLimiter=m_EnableReset;
            GetXROringinComponent();
            GetCapsuleColliderComponent();
            GetVRMovementComponent();
            GetRigidbodyComponent();
            GetAuxiliaryPositionerComponent();
            if (m_Rigidbody == null || m_XROrigin == null || m_capsuleCollider == null || m_auxPositioner == null || vrMovement == null)
            {
                canReset = false;
            }

            if (canReset) StartCoroutine(RecognizeHeadPiercing());
            else Debug.LogError("Reset function can't work");

            UpdateRigidBodyCollider();
        }
        //复位延时判断

        void Update()
        {
            UpdateRigidBodyCollider();//更新碰撞体高度
            UpdateReset();//复位功能

        }

        void OnDisable()
        {
            StopCoroutine(RecognizeHeadPiercing());
        }
        #endregion

        #region UpdateRigidBodyCollider
        private IEnumerator RecognizeHeadPiercing()
        {
            yield return new WaitForSeconds(1f);
            RecognizingHeadPiercings = true;
            yield return null;
        }

        //更新碰撞体高度
        protected virtual void UpdateRigidBodyCollider()
        {

            if (!(m_XROrigin == null) && !(m_capsuleCollider == null))
            {
                //更新计算
                float num = Mathf.Clamp(m_XROrigin.CameraInOriginSpaceHeight, m_MinHeight, m_MaxHeight);
                Vector3 cameraInOriginSpacePos = m_XROrigin.CameraInOriginSpacePos;
                cameraInOriginSpacePos.y = num / 2f + 0.001f;
                m_capsuleCollider.height = num;
                m_capsuleCollider.center = cameraInOriginSpacePos;

            }
        }
        #endregion

        #region Reset
        //复位功能
        private void UpdateReset()
        {
            UpdateResetStatus();
            Reset();
        }
        protected virtual void Reset()
        {
            if (m_EnableReset && m_XROrigin != null)
            {
                float groundClearance = GroundClearance();//离地高度

                if (RecognizingHeadPiercings && (m_XROrigin.CameraInOriginSpaceHeight - groundClearance) > 0.1f)// &&  m_XROrigin.transform.localPosition.y < -0.1f
                {

                    m_IsHeadPiercing = true;
                    Vector3 teleportionLocation = Vector3.zero;
                    teleportionLocation = GetTeleportationLocation();
                    if (m_IsHeadPiercing)
                    {
                        Teleportation(teleportionLocation);
                        //Debug.Log("XR Origin reset");
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
            if (m_auxPositioner == null)
            {
                GetAuxiliaryPositionerComponent();
                m_IsHeadPiercing = false;
                return Vector3.zero;
            }
            else
            {
                Vector3 teleportationLocation = m_auxPositioner.GetTeleportationLocation();
                teleportationLocation.y = m_XROrigin.transform.position.y;
                return teleportationLocation;
            }

        }

        //复位操作
        private void Teleportation(Vector3 _position)
        {
            if (vrMovement == null)
            {
                //组件获取,需要移植到start
                GetVRMovementComponent();
                return;
            }
            vrMovement.Teleportation(_position);
        }
        
        private void UpdateResetStatus()
        {
            if (m_EnableReset&&m_UpdateLimiter)
            {
                if (m_Rigidbody == null)
                {
                    GetRigidbodyComponent();
                }
                UpdateRigidbodyKinematic(false);
                ChangeMoveMethod(MoveMethod.MoveByRigidbody);
                m_UpdateLimiter = false;
            }
            else if(!m_EnableReset&&!m_UpdateLimiter)
            {
                if (m_Rigidbody == null)
                {
                    GetRigidbodyComponent();
                }
                UpdateRigidbodyKinematic(true);
                ChangeMoveMethod(MoveMethod.MoveByCommand);
                m_UpdateLimiter = true;
            }
        }
        private void UpdateRigidbodyKinematic(bool RigidbodyKinematicStatus)
        {
            m_Rigidbody.isKinematic = RigidbodyKinematicStatus;
        }
        #endregion

        #region Reset judgment
        private float GroundClearance()
        {
            //Vector3 cameraInOriginSpacePos = m_XROrigin.CameraInOriginSpacePos;
            Vector3 headPos = m_XROrigin.Camera.transform.position;
            Ray ray = new Ray(headPos, Vector3.down);
            Physics.Raycast(ray, out _raycastHit, 6f, LayerMask.GetMask("Environment"));

            Debug.DrawLine(headPos, _raycastHit.point, Color.red);

            float distance = math.distance(headPos, _raycastHit.point);
            return distance;


        }
        #endregion

        #region Attribute Settings
        public bool EnableReset
        {
            get => m_EnableReset;
            set => m_EnableReset = value;
        }

        protected CapsuleCollider capsuleCollider => m_capsuleCollider;
        protected XROrigin xrOrigin => m_XROrigin;
        public float minHeight
        {
            get => m_MinHeight;
            set => m_MinHeight = value;
        }

        public float maxHeight
        {
            get => m_MaxHeight;
            set => m_MaxHeight = value;
        }
        #endregion

        #region tools
        private void GetRigidbodyComponent()
        {
            if (m_Rigidbody == null)
            {
                m_Rigidbody = this.GetComponent<Rigidbody>();
                if (m_Rigidbody == null)
                {
                    Debug.LogError($"The {this.name}[{this.GetType()}] didn't find a Rigidbody component!", this);
                }
            }
        }
        private void GetAuxiliaryPositionerComponent()
        {
            if (m_auxPositioner == null)
            {
                m_auxPositioner = GetComponentsInChildren<VR_AuxiliaryPositioner>()[0];
                if (m_auxPositioner == null)
                {
                    Debug.LogError($"The {this.name}[{this.GetType()}] didn't find a VR_AuxiliaryPositioner component!", this);
                }
            }
        }
        private void GetVRMovementComponent()
        {
            if (vrMovement == null)
            {
                vrMovement = this.GetComponent<VRMovement_Tangerinesalt>();
                if (vrMovement == null)
                {
                    Debug.LogError($"RigidbodyColliderDrive({this.name}) No VRMovement Component");
                }
            }
        }

        private void GetCapsuleColliderComponent()
        {
            if (m_capsuleCollider == null)
            {
                m_capsuleCollider = this.GetComponent<CapsuleCollider>();
                if (m_capsuleCollider == null)
                {
                    Debug.LogError($"RigidBodyColliderDrive({this.name}) lose m_capsuleCollider");
                }
            }
        }

        private void GetXROringinComponent()
        {
            if (m_XROrigin == null)
            {
                m_XROrigin = this.transform.parent.GetComponent<XROrigin>();
                if (m_XROrigin == null)
                {
                    Debug.LogError($"RigidBodyColliderDrive({this.name}) lose m_XROrigin", this);
                }
            }
        }
        private void OnReset()
        {
            onReset?.Invoke();
        }
        private void ChangeMoveMethod(MoveMethod _moveMethod)
        {
            vrMovement.ChangeMoveMethod(_moveMethod);
        }
        #endregion

    }
}