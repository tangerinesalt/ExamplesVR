/****************************************************
    功能：玩家运动控制
    作者：
    创建日期：#2025/01/08#
    修改人：ZH
    修改日期：#2025/02/21#
    修改内容：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine.Events;
using System;

namespace Voltage
{
    public class OpenXRHandPlayerControllerLink_Voltage : MonoBehaviour
    {

        #region 变量

        [AutoHeader("Player Controller Link")]
        public bool ignoreMe;
        [SerializeField] public AutoHandPlayer player;
        [SerializeField] private Transform m_playerRoot;

        [AutoToggleHeader("Switch")]
        public bool EnableSwitch = true;
        [SerializeField] public bool EnableMove = true;
        [Tooltip("Enable rotation when the value is true, and enable Dual-hand movement when the value is false")]
        [SerializeField] public bool EnableTurn = true;
        [Tooltip("Minimum value for mobile input")]
        [SerializeField] public float _MoveDeadZone = 0.1f;
        [SerializeField] public float m_moveSpeedByKinematic = 3.0f;

        [AutoToggleHeader("Input")]
        public bool EnableInput = true;
        public EMoveMode m_moveMode = EMoveMode.MovebyPhysics;
        
        [SerializeField] public InputActionProperty moveAxisL;
        [DisableIf("EnableTurn")]
        [Tooltip("Dispensable, Enable when Turn Axis is not used, or when the reference is empty")]
        [SerializeField] public InputActionProperty moveAxisR;
        [EnableIf("EnableTurn")]
        [SerializeField] public InputActionProperty turnAxis;

        [HideInInspector]
        public bool TeleportRequireThumbClick = false;
        public bool InstantlyRotate
        {
            get { return player.rotationType == RotationType.snap; }
        }

        #endregion


        #region Unity生命周期

        private void OnEnable()
        {
            if (m_playerRoot == null)
            {
                m_playerRoot = player.transform.parent;
            }

            if (moveAxisL.action != null)
            {
                moveAxisL.action.Enable();
                moveAxisL.action.performed += MoveAction;
            }
            if (moveAxisR.action != null)
            {
                moveAxisR.action.Enable();
            }
            if (turnAxis.action != null)
            {
                turnAxis.action.Enable();
                turnAxis.action.performed += TurnAction;
            }
        }

        private void OnDisable()
        {
            if (moveAxisL.action != null) moveAxisL.action.performed -= MoveAction;
            if (turnAxis.action != null) turnAxis.action.performed -= TurnAction;
        }

        private void FixedUpdate()
        {
            Movement();
        }

        private void Update()
        {
            //再次更新运动，避免在FixedUpdate中无法获取到最新的输入值，以免player一直移动
            if (m_moveMode == EMoveMode.MovebyPhysics && EnableInput)
            {
                if (EnableMove) player.Move(moveAxisL.action.ReadValue<Vector2>());
                if (!EnableTurn && EnableMove) player.Move(moveAxisR.action.ReadValue<Vector2>());
                else if (EnableTurn) player.Turn(turnAxis.action.ReadValue<Vector2>().x);
            }
        }

        #endregion


        #region 事件

        private void MoveAction(InputAction.CallbackContext a)
        {
            // 事件处理代码
        }

        private void TurnAction(InputAction.CallbackContext a)
        {
            if (EnableTurn) StartCoroutine(PlayerManager.Instance.CheckRays());
        }

        #endregion


        #region 移动旋转

        /// <summary>
        /// 双手输入、单手输入逻辑；以及刚体运动和运动学刚体运动的选择
        /// </summary>
        private void Movement()
        {
            if (!EnableMove && !EnableTurn) return;

            Vector2 moveInputL = moveAxisL.action?.ReadValue<Vector2>() ?? Vector2.zero;
            Vector2 moveInputR = moveAxisR.action?.ReadValue<Vector2>() ?? Vector2.zero;
            Vector2 turnInput = turnAxis.action?.ReadValue<Vector2>() ?? Vector2.zero;

            MoveLogic(moveInputL, moveInputR);
            TurnLogic(turnInput);
        }

        /// <summary>
        /// 双手输入、单手输入逻辑
        /// </summary>
        /// <param name="moveL"></param>
        /// <param name="moveR"></param>
        private void MoveLogic(Vector2 moveL, Vector2 moveR)
        {
            if (!EnableMove) return;

            bool isLeftMove = moveL.sqrMagnitude > _MoveDeadZone * _MoveDeadZone;
            bool isRightMove = moveR.sqrMagnitude > _MoveDeadZone * _MoveDeadZone;

            // 检查 moveAxisR 是否为 null 且 EnableTurn 为 false
            if (moveAxisR.action == null && !EnableTurn)
            {
                if (isLeftMove)
                {
                    Move(moveL);
                    Debug.Log("moveAxisR is null and EnableTurn is false, only using moveL.");
                }
                return;
            }

            Vector2 input = Vector2.zero;
            if (EnableTurn && isLeftMove)
            {
                input = moveL;
            }
            else if (!EnableTurn)
            {
                if (isLeftMove && !isRightMove)
                {
                    input = moveL;
                }
                else if (isRightMove && !isLeftMove)
                {
                    input = moveR;
                }
                else if (isLeftMove && isRightMove)
                {
                    input.x = Mathf.Abs(moveL.x) > Mathf.Abs(moveR.x) ? moveL.x : moveR.x;
                    input.y = Mathf.Abs(moveL.y) > Mathf.Abs(moveR.y) ? moveL.y : moveR.y;
                }
            }

            if (input != Vector2.zero) Move(input);
        }

        private void Move(Vector2 axisInput)
        {
            if (m_moveMode == EMoveMode.MovebyPhysics)
            {
                player.useGrounding = true;
                player.body.isKinematic = false;
                player.Move(axisInput);
            }
            else if (m_moveMode == EMoveMode.MoveByKinematic)
            {
                player.body.isKinematic = true;
                player.useGrounding = false;
                MoveByKinematic(axisInput);
            }
        }

        public void MoveByKinematic(Vector2 axisInput)
        {
            float deltaDeg = Vector2.SignedAngle(Vector2.up, axisInput);
            Vector3 headDir3 = player.forwardFollow.forward;
            Vector2 headDir2 = new Vector2(headDir3.x, headDir3.z);
            float headDeg = Vector2.SignedAngle(Vector2.right, headDir2);

            float finalRad = (headDeg + deltaDeg) * Mathf.Deg2Rad;
            Vector3 moveDir = new Vector3(Mathf.Cos(finalRad), 0, Mathf.Sin(finalRad));

            float realSpeed = axisInput.magnitude * m_moveSpeedByKinematic;
            m_playerRoot.Translate(realSpeed * Time.fixedDeltaTime * moveDir, Space.World);
        }

        private void TurnLogic(Vector2 turnInput)
        {
            if (EnableTurn)
            {
                player.Turn(turnInput.x);
            }
        }

        #endregion

    }
}