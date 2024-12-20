using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using NaughtyAttributes;
using Mirror.Examples.Basic;
using Mirror;

namespace Voltage
{
    /// <summary>
    /// 自定义玩家输入控制脚本,允许双手运动,或左手运动右手旋转
    /// 提供了移动和转向的控制接口
    /// </summary>
    public class OpenXRHandPlayerControllerLink_Voltage : MonoBehaviour
    {
        public AutoHandPlayer player;

        [Header("Switch")]
        public bool EnableMove = true;
        [Tooltip("Minimum value for mobile input")]
        public float _MoveDeadZone = 0.1f;
        [Tooltip("Enable rotation when the value is false, and enable Dual-hand movement when the value is true")]
        public bool EnableTurnOrDualHandMovement = true;
        [Header("Input")]
        public InputActionProperty moveAxisL;
        [DisableIf("EnableTurnOrDualHandMovement")]
        [Tooltip("Dispensable, Enable when Turn Axis is not used, or when the reference is empty")]
        public InputActionProperty moveAxisR;
        [EnableIf("EnableTurnOrDualHandMovement")]
        public InputActionProperty turnAxis;

        private void OnEnable()
        {
            if (moveAxisL.action != null) moveAxisL.action.Enable();
            if (moveAxisL.action != null) moveAxisL.action.performed += MoveAction;
            if (moveAxisR.action != null) moveAxisR.action.Enable();
            if (turnAxis.action != null) turnAxis.action.Enable();
            if (turnAxis.action != null) turnAxis.action.performed += TurnAction;
        }

        private void OnDisable()
        {
            if (moveAxisL.action != null) moveAxisL.action.performed -= MoveAction;
            if (turnAxis.action != null) turnAxis.action.performed -= TurnAction;
        }

        private void FixedUpdate()
        {
            Movement();
            // player.Move(moveAxisL.action.ReadValue<Vector2>());
            // player.Turn(turnAxis.action.ReadValue<Vector2>().x);
        }

        private void Update()
        {
            //再次更新运动，避免在FixedUpdate中无法获取到最新的输入值，以免player一直移动
            if(EnableMove) player.Move(moveAxisL.action.ReadValue<Vector2>());
            if (!EnableTurnOrDualHandMovement&&EnableMove)  player.Move(moveAxisR.action.ReadValue<Vector2>());
            else if (EnableTurnOrDualHandMovement)          player.Turn(turnAxis.action.ReadValue<Vector2>().x);
            // player.Move(moveAxisL.action.ReadValue<Vector2>());
            // player.Turn(turnAxis.action.ReadValue<Vector2>().x);
        }

        void MoveAction(InputAction.CallbackContext a)
        {
            // var axis = a.ReadValue<Vector2>();
            // player.Move(axis);

            // Vector2 MoveInputL = moveAxisL.action.ReadValue<Vector2>();
            // Vector2 MoveInputR = moveAxisR.action.ReadValue<Vector2>();
            // MoveLogic(MoveInputL, MoveInputR);
            // Debug.Log("MoveAction for OpenXRHandPlayerControllerLink_tangerine");
        }

        void TurnAction(InputAction.CallbackContext a)
        {
            // var axis = a.ReadValue<Vector2>();
            // player.Turn(axis.x);

            // Vector2 TurnInput = turnAxis.action.ReadValue<Vector2>();
            // TurnLogic(TurnInput);
        }
        void Movement()
        {
            Vector2 MoveInputL = moveAxisL.action.ReadValue<Vector2>();
            Vector2 MoveInputR = moveAxisR.action.ReadValue<Vector2>();
            Vector2 TurnInput = turnAxis.action.ReadValue<Vector2>();

            //bool isLeftTurn=TurnInput.magnitude>_MoveDeadZone;
            MoveLogic(MoveInputL, MoveInputR);
            TurnLogic(TurnInput);
        }
        void MoveLogic(Vector2 MoveInputL, Vector2 MoveInputR)
        {
            if (EnableMove)
            {
                bool isLeftMove = MoveInputL.magnitude > _MoveDeadZone;
                bool isRightMove = MoveInputR.magnitude > _MoveDeadZone;

                if (EnableTurnOrDualHandMovement)
                {
                    //单手移动逻辑
                    if (isLeftMove)
                        player.Move(MoveInputL);
                }
                else
                {
                    if (moveAxisR.action == null)
                    {
                        if (isLeftMove) player.Move(MoveInputL);
                        Debug.Log("moveAxisR is null");
                        return;
                    }

                    //双手移动逻辑
                    if (isLeftMove && !isRightMove)
                    {
                        player.Move(MoveInputL);
                    }
                    else if (isRightMove && !isLeftMove)
                    {
                        player.Move(MoveInputR);
                    }
                    else if (isLeftMove && isRightMove)
                    {
                        //判断左右手的移动方向
                        bool isLeftHorizontal = math.abs(MoveInputL.x) > math.abs(MoveInputR.x);
                        bool isLeftVertical = math.abs(MoveInputL.y) > math.abs(MoveInputR.y);
                        if (isLeftHorizontal && isLeftVertical)
                        {
                            player.Move(MoveInputL);
                        }
                        else if (isLeftHorizontal && !isLeftVertical)
                        {
                            player.Move(new Vector2(MoveInputL.x, MoveInputR.y));
                        }
                        else if (!isLeftHorizontal && isLeftVertical)
                        {
                            player.Move(new Vector2(MoveInputR.x, MoveInputL.y));
                        }
                        else if (!isLeftHorizontal && !isLeftVertical)
                        {
                            player.Move(MoveInputR);
                        }
                    }
                    else
                    {
                        //返回不移动的结果
                    }
                }
            }
        }
        void TurnLogic(Vector2 TurnInput)
        {
            if (EnableTurnOrDualHandMovement)
            {
                player.Turn(TurnInput.x);
            }
        }
        //补充方法
        public void SetPosition(Transform target,bool isKinematic=false)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = isKinematic;
            
            player.SetPosition(target.position,Quaternion.LookRotation(target.forward));
        }
        public void SetPosition(Transform target)
        {
            player.SetPosition(target.position,Quaternion.LookRotation(target.forward));
            //SetPositioncalculation(target.position);
            gameObject.GetComponent<Rigidbody>().isKinematic=false;
        }
        public void SetPositioncalculation( Vector3 position)
        {
             Vector3 deltaPos = position - transform.position;
            transform.position += deltaPos; 
           gameObject.GetComponent<Rigidbody>().position = transform.position;
        }
        public void SetPosition(Vector3 target)
        {
            player.SetPosition(target);
            gameObject.GetComponent<Rigidbody>().isKinematic=false;
        }
        public void SetKinematic(bool isKinematic)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = isKinematic;
        }
        
        public bool TeleportRequireThumbClick=false;
        public bool InstantlyRotate { get { return player.rotationType == RotationType.snap; } } //player.rotationType == RotationType.snap;
    }
    #region Network Messages
    public struct RigidbodyMoveMessage : NetworkMessage
    {
        public NetworkIdentity Identity; 
        public uint avatarID;
        public int moveState;
    }
    #endregion
}
