using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using System;
using Unity.Mathematics;
using Tangerine;
using RootMotion.Demos;

namespace Tangerine
{
    /// <summary>
    /// 可以选择三种移动方式,并提供自定义移动方式MoveMethod.custom;
    /// 提供unity生命周期继承方式
    /// </summary>
    public class VRMovementBase : MonoBehaviour
    {
        [Header("Rotate")]
        [SerializeField] protected bool m_EnableRotate = false;
        [SerializeField] protected float m_RotateSpeed = 10f;

        [Header("Move")]
        [SerializeField] protected bool m_EnableMove = true;
        [SerializeField] protected MoveMethod m_MoveMethod = MoveMethod.MoveByCommand;

        protected bool m_EnableMoveWithRigidbody = true;
        [SerializeField] protected float m_MoveSpeed = 3f;
        [SerializeField] protected float m_MoveDeadZone = 0.1f;
        protected bool m_isMove = false;


        [Header("Input")]
        [SerializeField] protected InputActionReference m_InputAxis2DLeft = null;
        [SerializeField] protected InputActionReference m_InputAxis2DRight = null;
        [SerializeField] protected Transform m_RootTrans = null;
        [SerializeField] protected Transform m_HeadTrans = null;
        [SerializeField] protected Rigidbody m_Rigidbody = null;


        private MoveDirect m_direct = MoveDirect.None;

        private float m_realSpeed = 0;
        #region  Unity Cycle
        protected virtual void Start()
        {
            if (m_RootTrans == null) m_RootTrans = this.transform;
        }

        
        protected virtual void Update()
        {
            OpenActivity();
        }
        
        public void ChangeMoveStatus(bool isMove)
        {
            m_EnableMove= isMove;
        }
        #endregion

        #region Movement
        protected virtual void OpenActivity()
        {
            if (m_EnableMove)
            {
                updateMove();
            }
            if (m_EnableRotate)
            {
                updateRotate();
            }
        }
        #endregion

        #region Move
        private void updateMove()
        {
            Vector2 axisDirL = m_InputAxis2DLeft.action.ReadValue<Vector2>();
            //Debug.Log("axisDirL.x:"+axisDirL.x);
            Vector2 axisDirR = m_InputAxis2DRight.action.ReadValue<Vector2>();
            bool isLeftMove = axisDirL.magnitude > m_MoveDeadZone;
            bool isRightMove = axisDirR.magnitude > m_MoveDeadZone;
            //Debug.Log("axisDirR.magnitude:"+axisDirR.magnitude);

            if (m_EnableMove && m_EnableRotate)
            {
                if (isLeftMove)
                {
                    moveByAxisDir(axisDirL);
                }
                else
                {
                    m_direct = 0;
                }
            }
            if (m_EnableMove && !m_EnableRotate)
            {
                if (isLeftMove && !isRightMove)
                {
                    moveByAxisDir(axisDirL);
                }
                else if (!isLeftMove && isRightMove)
                {
                    moveByAxisDir(axisDirR);
                }
                else if (isLeftMove && isRightMove)
                {
                    bool isLeftHorizontal = math.abs(axisDirL.x) > math.abs(axisDirR.x);
                    bool isLeftVertical = math.abs(axisDirL.y) > math.abs(axisDirR.y);

                    if (isLeftHorizontal && isLeftVertical)
                    {
                        moveByAxisDir(axisDirL);
                    }
                    else if (isLeftHorizontal && !isLeftVertical)
                    {
                        moveByAxisDir(new Vector2(axisDirL.x, axisDirR.y));
                    }
                    else if (!isLeftHorizontal && isLeftVertical)
                    {
                        moveByAxisDir(new Vector2(axisDirR.x, axisDirL.y));
                    }
                    else
                    {
                        moveByAxisDir(axisDirR);
                    }
                }
                else
                {
                    m_direct = 0;
                }

            }

        }
         #endregion

        #region  Choose MoveMethod
        protected void moveByAxisDir(Vector2 axisDir)
        {
            float deltaDeg = Vector2.SignedAngle(Vector2.up, axisDir);

            Vector3 headDir3 = m_HeadTrans.forward;
            Vector2 headDir2 = new Vector2(headDir3.x, headDir3.z);
            float headDeg = Vector2.SignedAngle(Vector2.right, headDir2);

            float finalRad = (headDeg + deltaDeg) * Mathf.Deg2Rad;
            Vector3 MoveDir = new Vector3(Mathf.Cos(finalRad), 0, Mathf.Sin(finalRad));

            //Vector3 hDirection = Vector3.ProjectOnPlane(headDir3,Vector3.up);
            float MoveAngle = Vector3.SignedAngle(MoveDir, headDir3, Vector3.up);
            m_direct = GetDirect(MoveAngle);
            m_realSpeed = axisDir.magnitude * m_MoveSpeed;

            ChooseMoveMethod(m_MoveMethod, MoveDir);
            //DefaultMoveMethod(MoveDir);
        }
        public virtual void ChooseMoveMethod(MoveMethod _moveMethod, Vector3 MoveDir)
        {
            if (_moveMethod == MoveMethod.MoveByCommand)
            {
                MoveByCommand(MoveDir);
            }
            else if (_moveMethod == MoveMethod.MoveByRigidbody)
            {
                MoveByRigidbody(MoveDir);
            }
            else if (_moveMethod == MoveMethod.MoveByCharacterController)
            {
                MoveByCharacterControllerd(MoveDir);
            }
            else if (_moveMethod == MoveMethod.NoneMove)
            {
                m_EnableMove = false;
            }


        }
        private void DefaultMoveMethod(Vector3 MoveDir)
        {
            bool isRigidbodyMove = false;
            if (!m_EnableMoveWithRigidbody)
            {
                isRigidbodyMove = false;
                if (!isRigidbodyMove && m_Rigidbody != null)
                {
                    if (!m_Rigidbody.isKinematic)
                    {
                        m_Rigidbody.isKinematic = true;
                    }
                    isRigidbodyMove = true;
                }

                m_RootTrans.Translate(m_realSpeed * Time.deltaTime * MoveDir, Space.World);
            }
            else
            {
                isRigidbodyMove = true;
                if (isRigidbodyMove && m_Rigidbody.isKinematic)
                {
                    m_Rigidbody.isKinematic = false;
                    isRigidbodyMove = false;
                }
                Vector3 playerHorizontalVelocity = m_Rigidbody.velocity;
                playerHorizontalVelocity.y = 0f;
                m_Rigidbody.AddForce((m_realSpeed * MoveDir) - playerHorizontalVelocity, ForceMode.VelocityChange);
            }

        }
        public virtual void MoveByCommand(Vector3 MoveDir)
        {
            m_EnableMove = true;
            m_EnableMoveWithRigidbody = false;
            
            m_RootTrans.Translate(m_realSpeed * Time.deltaTime * MoveDir, Space.World);
        }
        public virtual void MoveByRigidbody(Vector3 MoveDir)
        {
            m_EnableMove = true;
            m_EnableMoveWithRigidbody = true;
            if (m_Rigidbody == null)
            {
                GetRigidbodyComponent();
                if (m_Rigidbody == null)
                {
                    m_MoveMethod = MoveMethod.MoveByCommand;
                    Debug.Log("change MoveMethod to MoveByCommand");
                    return;
                }
            }
            if(m_Rigidbody != null && m_Rigidbody.isKinematic)
            {
                //m_Rigidbody.isKinematic = false;
                m_MoveMethod = MoveMethod.MoveByCommand;
                Debug.Log("change MoveMethod to MoveByCommand");
                return;
            }

            Vector3 playerHorizontalVelocity = m_Rigidbody.velocity;
            playerHorizontalVelocity.y = 0f;
            m_Rigidbody.AddForce((m_realSpeed * MoveDir) - playerHorizontalVelocity, ForceMode.VelocityChange);
        }
        public virtual void MoveByCharacterControllerd(Vector3 MoveDir)
        {
            bool errorExamples = false;
            if (!errorExamples)
            {
                Debug.LogError($"have a characterControllerd(VRMovement:VRMovementBase) Error for{this.name}");
                m_MoveMethod = MoveMethod.MoveByCommand;
                Debug.Log("change MoveMethod to MoveByCommand");
                return;
            }
            if (errorExamples)
            {
                CharacterController characterController = this.GetComponent<CharacterController>();
                if (characterController == null)
                {
                    Debug.LogError($"Please Add CharacterController Component for{this.name}");
                    m_MoveMethod = MoveMethod.MoveByCommand;
                    Debug.Log("change MoveMethod to MoveByCommand");
                    return;
                }
                else
                {
                    m_EnableMove = true;
                    m_EnableMoveWithRigidbody = false;
                    
                        if (m_Rigidbody != null && !m_Rigidbody.isKinematic)
                        {
                            m_Rigidbody.isKinematic = true;
                        }
                    
                    characterController.Move(m_realSpeed * Time.deltaTime * MoveDir);
                }
            }

        }

        #endregion

        #region Rotate

        protected void updateRotate()
        {
            Vector2 axisDirR = m_InputAxis2DRight.action.ReadValue<Vector2>();
            //Debug.Log ("axisDirR.X:"+axisDirR.x);
            rotateByAxisDir(axisDirR.x * m_RotateSpeed * Time.deltaTime);
        }
        protected void rotateByAxisDir(float axisDir_Horizontal)
        {
            Vector3 oldRootPos = m_RootTrans.position;
            Vector3 oldHeadPos = m_HeadTrans.position;

            Vector2 v2HeadToRoot = new Vector2(oldRootPos.x - oldHeadPos.x, oldRootPos.z - oldHeadPos.z);
            float targetRad = (Vector2.SignedAngle(Vector2.right, v2HeadToRoot) - axisDir_Horizontal) * Mathf.Deg2Rad;
            Vector2 v2HeadToRootNewDir = new Vector2(Mathf.Cos(targetRad), Mathf.Sin(targetRad));
            float v2HeadToRootMag = v2HeadToRoot.magnitude;
            m_RootTrans.position = new Vector3(
                oldHeadPos.x + v2HeadToRootNewDir.x * v2HeadToRootMag,
                oldRootPos.y,
                oldHeadPos.z + v2HeadToRootNewDir.y * v2HeadToRootMag);

            m_RootTrans.Rotate(Vector3.up, axisDir_Horizontal);
            transform.Rotate(0, axisDir_Horizontal, 0);
        }
        #endregion
        #region Teleportation
        public void Teleportation(Vector3 _position)
        {
            m_RootTrans.position = _position;
        }

        #endregion

        #region Other
        public MoveDirect GetDirect(float angle)
        {
            if (angle > -22.5f && angle <= 22.5f)
            {
                return MoveDirect.Forward;
            }
            if (angle > 22.5f && angle <= 67.5f)
            {
                return MoveDirect.ForwardLeft;
            }
            if (angle > 67.5f && angle <= 112.5f)
            {
                return MoveDirect.Left;
            }
            if (angle > 112.5f && angle <= 157.5f)
            {
                return MoveDirect.BackLeft;
            }
            if (angle > 157.5f && angle <= 180f)
            {
                return MoveDirect.Back;
            }
            if (angle >= -180f && angle <= -157.5f)
            {
                return MoveDirect.Back;
            }
            if (angle > -157.5f && angle <= -112.5f)
            {
                return MoveDirect.BackRight;
            }
            if (angle > -112.5f && angle <= -67.5f)
            {
                return MoveDirect.Right;
            }
            if (angle > -67.5f && angle <= -22.5f)
            {
                return MoveDirect.ForwardRight;
            }
            return 0;
        }
        private void GetRigidbodyComponent()
        {
            if (m_Rigidbody == null)
            {
                m_Rigidbody = this.GetComponent<Rigidbody>();
                if (m_Rigidbody == null)
                {
                    Debug.LogError("Please Add Rigidbody Component for" + this.name);
                }
            }
        }
        
        public void ChangeMoveMethod(MoveMethod _moveMethod)
        {
            if (m_MoveMethod != _moveMethod)
            {
                m_MoveMethod = _moveMethod;

                switch (m_MoveMethod)
                {
                    case MoveMethod.MoveByCommand:
                        Debug.Log("change MoveMethod to MoveByCommand");
                        break;
                    case MoveMethod.MoveByRigidbody:
                        if (m_Rigidbody == null)
                            GetRigidbodyComponent();
                        if (m_Rigidbody != null)
                        {
                            if (m_Rigidbody.isKinematic)
                                Debug.LogError($"({this.name})Rigidbody isKinematic is true,but it should be false");
                            else 
                                Debug.Log("change MoveMethod to MoveByRigidbody");
                        }
                        break;
                    case MoveMethod.MoveByCharacterController:
                        Debug.Log("change MoveMethod to MoveByCharacterController");
                        break;
                    case MoveMethod.NoneMove:
                        Debug.Log("change MoveMethod to NoneMove");
                        break;
                    case MoveMethod.MoveByCustom:
                        Debug.Log("change MoveMethod to custom");
                        break;
                    default:
                        Debug.Log("change MoveMethod to default");
                        break;
                }
            }
        }
        #endregion
    }
}
