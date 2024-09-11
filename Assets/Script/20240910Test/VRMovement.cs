using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using System;
using Unity.Mathematics;
using Tangerine;

namespace Tangerine
{
    public class VRMovement : MonoBehaviour
    {
        [Header("Rotate")]
        [SerializeField] private bool m_EnableRotate = true;
        [SerializeField] private float m_RotateSpeed = 10f;
        [Header("Move")]
        [SerializeField] private bool m_EnableMove = true;
        //[SerializeField] private bool m_EnableMoveOnlyLeft = true;
        [SerializeField] private float m_MoveSpeed = 3f;
        [SerializeField] private float m_MoveDeadZone = 0.1f;
        [Header("Input")]
        [SerializeField] private InputActionReference m_InputAxis2DLeft = null;
        [SerializeField] private InputActionReference m_InputAxis2DRight = null;
        [SerializeField] private Transform m_RootTrans = null;
        [SerializeField] private Transform m_HeadTrans = null;

        private MoveDirect m_direct = MoveDirect.None;

        private float m_realSpeed = 0;
        #region  Unity Cycle
        void Start()
        {
            if (m_RootTrans == null) m_RootTrans = this.transform;
        }


        // Update is called once per frame
        void Update()
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
            Debug.Log("axisDirR.magnitude:"+axisDirR.magnitude);

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

        private void moveByAxisDir(Vector2 axisDir)
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
            m_RootTrans.Translate(m_realSpeed * Time.deltaTime * MoveDir, Space.World);

        }
        #endregion

        #region Rotate
        private void updateRotate()
        {
            Vector2 axisDirR = m_InputAxis2DRight.action.ReadValue<Vector2>();
            //Debug.Log("axisDirR.X:"+axisDirR.x);
            rotateByAxisDir(axisDirR.x * m_RotateSpeed * Time.deltaTime);
        }
        private void rotateByAxisDir(float axisDir_Horizontal)
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
        #endregion
    }
}
