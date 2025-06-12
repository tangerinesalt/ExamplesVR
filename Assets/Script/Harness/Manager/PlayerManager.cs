/****************************************************
    功能：玩家交互管理
    作者：ZH
    创建日期：#2025/01/08#
    修改内容：
        1.增加主菜单控制功能    2025/03/05 ZH
        2.增加手柄按键高亮显隐功能    2025/03/12 ZH
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;
using UnityEngine.InputSystem.XR;
using Bhaptics.SDK2;
using Unity.VisualScripting;

namespace Voltage
{
    public class PlayerManager : MonoSingleton<PlayerManager>
    {
        [Header("玩家控制器")]
        [SerializeField] private OpenXRHandPlayerControllerLink_Voltage m_playerController;

        [Header("视野蒙板")]
        [SerializeField] private GameObject eyeFade;

        [Header("左手射线")]
        [SerializeField] private GameObject leftRay;
        [Header("右手射线")]
        [SerializeField] private GameObject rightRay;

        [Header("左手模型")]
        [SerializeField] private GameObject leftHandModel;
        [Header("右手模型")]
        [SerializeField] private GameObject rightHandModel;

        [Header("左手手柄")]
        [SerializeField] private GameObject leftControllerModel;
        [Header("右手手柄")]
        [SerializeField] private GameObject rightControllerModel;

        [Header("左手")]
        [SerializeField] private GameObject leftControllerStick;
        [SerializeField] private GameObject leftControllerTrigger;
        [SerializeField] private GameObject leftControllerGrip;
        [SerializeField] private GameObject leftControllerPrimaryButton;
        [SerializeField] private GameObject leftControllerSecondaryButton;

        [Header("右手")]
        [SerializeField] private GameObject rightControllerStick;
        [SerializeField] private GameObject rightControllerTrigger;
        [SerializeField] private GameObject rightControllerGrip;
        [SerializeField] private GameObject rightControllerPrimaryButton;
        [SerializeField] private GameObject rightControllerSecondaryButton;

        [Header("左手高亮投射器")]
        [SerializeField] private GameObject m_LeftHandProjector;
        [Header("右手高亮投射器")]
        [SerializeField] private GameObject m_RightHandProjector;

        [Header("左手震动")]
        [SerializeField] private InputActionReference m_HapticActionLeft;
        [Header("右手震动")]
        [SerializeField] private InputActionReference m_HapticActionRight;

        /// <summary>
        /// 是否可以打开主菜单
        /// </summary>
        public bool isCanOpenMainMenu = false;
        public bool isCanOpenMainMenuLeft = false;
        public bool isCanOpenMainMenuRight = false;

        /// <summary>
        /// 主菜单UI
        /// </summary>
        [HideInInspector]
        public PanelControl mainMenu;

        /// <summary>
        /// 测试用，用完修改为属性
        /// </summary>
        [SerializeField]
        private string m_currentTaskName = "CustomTask";
        public string CurrentTaskName
        {
            get { return m_currentTaskName; }
            set { m_currentTaskName = value; }
        }

        public void UpdateCurrentTask(string taskName)
        {
            mainMenu.UpdateCurrentTask(taskName);
        }

        public bool isFinish = false;

        public string CustomTaskContent { get; set; }

        /// <summary>
        /// 移动状态
        /// </summary>
        public bool EnableMove
        {
            get { return m_playerController.EnableMove; }
            set { m_playerController.EnableMove = value; }
        }

        /// <summary>
        /// 旋转状态（右手）
        /// </summary>
        public bool EnableTurn
        {
            get { return m_playerController.EnableTurn; }
            set { m_playerController.EnableTurn = value; }
        }

        /// <summary>
        /// 移动速度
        /// </summary>
        public float MoveSpeed
        {
            get
            {
                return m_playerController.player.maxMoveSpeed;
            }
            set
            {
                float speed = value;
                m_playerController.player.maxMoveSpeed = speed;
                m_playerController.m_moveSpeedByKinematic = speed;
            }
        }

        /// <summary>
        /// 旋转速度
        /// </summary>
        public float TurnSpeed
        {
            get { return m_playerController.player.snapTurnAngle; }
            set { m_playerController.player.snapTurnAngle = value; }
        }

        /// <summary>
        /// 运动模式
        /// </summary>
        public EMoveMode moveMode
        {
            get
            {
                return m_playerController.m_moveMode;
            }
            set
            {
                m_playerController.m_moveMode = value;
                if (value == EMoveMode.MoveByKinematic)
                {
                    m_playerController.player.useGrounding = false;
                    m_playerController.player.body.isKinematic = true;
                }
                else
                {
                    m_playerController.player.useGrounding = true;
                    m_playerController.player.body.isKinematic = false;
                }
            }
        }

        /// <summary>
        /// 线缆安装状态
        /// </summary>
        public EInstallState state = EInstallState.Uninstall;

        private void Start()
        {
            mainMenu = ResManager.Instance.LoadPrefab("GO/UI3D/PanelControl").GetComponent<PanelControl>();
            DontDestroyOnLoad(mainMenu.gameObject);
        }

        /// <summary>
        /// 视野淡入变亮
        /// </summary>
        public void EyeFadeBright()
        {
            if (eyeFade != null && eyeFade.GetComponent<Renderer>() != null)
            {
                eyeFade.GetComponent<Renderer>().material.DOFade(0, 0.3f);
            }
        }

        /// <summary>
        /// 视野淡出变暗
        /// </summary>
        public void EyeFadeDark()
        {
            if (eyeFade != null && eyeFade.GetComponent<Renderer>() != null)
            {
                eyeFade.GetComponent<Renderer>().material.DOFade(1, 0.3f);
            }
        }

        /// <summary>
        /// 设置射线的开启关闭
        /// </summary>
        /// <param name="handType"></param>
        /// <param name="state"></param>
        public void SetRayState(EHandType handType, bool state = true)
        {
            switch (handType)
            {
                case EHandType.Left:
                    leftRay.SetActive(state);
                    break;
                case EHandType.Right:
                    rightRay.SetActive(state);
                    break;
                case EHandType.Both:
                    leftRay.SetActive(state);
                    rightRay.SetActive(state);
                    break;
            }
        }

        /// <summary>
        /// 获取射线的状态
        /// </summary>
        /// <param name="handType"></param>
        /// <returns></returns>
        public bool GetRayState(EHandType handType)
        {
            bool state = false;
            switch (handType)
            {
                case EHandType.Left:
                    state = leftRay.activeSelf;
                    break;
                case EHandType.Right:
                    state = rightRay.activeSelf;
                    break;
            }
            return state;
        }

        /// <summary>
        /// 转向检查射线
        /// </summary>
        /// <returns></returns>
        public IEnumerator CheckRays()
        {
            bool isLeftRayShow = GetRayState(EHandType.Left);
            bool isRightRayShow = GetRayState(EHandType.Right);

            if (isLeftRayShow) SetRayState(EHandType.Left, false);
            if (isRightRayShow) SetRayState(EHandType.Right, false);

            yield return new WaitForSeconds(0.001f);

            if (isLeftRayShow) SetRayState(EHandType.Left);
            if (isRightRayShow) SetRayState(EHandType.Right);
        }

        /// <summary>
        /// 设置位置
        /// </summary>
        /// <param name="pos"></param>
        public void SetPosition(Vector3 pos)
        {
            m_playerController.player.SetPosition(pos);
        }

        /// <summary>
        /// 获取玩家位置
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPlayerPosition()
        {
            return m_playerController.player.transform.position;
        }

        /// <summary>
        /// 设置旋转
        /// </summary>
        /// <param name="rotation"></param>
        public void SetRotation(Vector3 rotation)
        {
            Quaternion qu = Quaternion.Euler(rotation);
            m_playerController.player.SetRotation(qu);
        }

        /// <summary>
        /// 设置手部模型的显示隐藏
        /// </summary>
        /// <param name="handType"></param>
        /// <param name="state"></param>
        public void SetHandModelState(EHandType handType, bool state = true)
        {
            switch (handType)
            {
                case EHandType.Left:
                    leftHandModel.SetActive(state);
                    break;
                case EHandType.Right:
                    rightHandModel.SetActive(state);
                    break;
                case EHandType.Both:
                    leftHandModel.SetActive(state);
                    rightHandModel.SetActive(state);
                    break;
            }
        }

        /// <summary>
        /// 设置手柄的显示隐藏
        /// </summary>
        /// <param name="handType"></param>
        /// <param name="state"></param>
        public void SetControllerModelState(EHandType handType, bool state = true)
        {
            switch (handType)
            {
                case EHandType.Left:
                    leftControllerModel.SetActive(state);
                    break;
                case EHandType.Right:
                    rightControllerModel.SetActive(state);
                    break;
                case EHandType.Both:
                    leftControllerModel.SetActive(state);
                    rightControllerModel.SetActive(state);
                    break;
            }
        }

        /// <summary>
        /// 设置手柄按钮的高亮显示
        /// </summary>
        /// <param name="buttonType"></param>
        /// <param name="state"></param>
        /// <param name="handType"></param>
        public void SetControllerButtonHighlight(EControllerButtonType buttonType, bool state = true, EHandType handType = EHandType.Both)
        {
            if (handType == EHandType.Left || handType == EHandType.Both)
            {
                SetButtonState(buttonType, state, true);
            }

            if (handType == EHandType.Right || handType == EHandType.Both)
            {
                SetButtonState(buttonType, state, false);
            }
        }

        /// <summary>
        /// 设置按钮的显示隐藏
        /// </summary>
        /// <param name="buttonType"></param>
        /// <param name="state"></param>
        /// <param name="isLeftHand"></param>
        private void SetButtonState(EControllerButtonType buttonType, bool state, bool isLeftHand)
        {
            GameObject buttonObject = null;

            // 根据按钮类型和左右手选择对应的 GameObject
            switch (buttonType)
            {
                case EControllerButtonType.Stick:
                    buttonObject = isLeftHand ? leftControllerStick : rightControllerStick;
                    break;
                case EControllerButtonType.Trigger:
                    buttonObject = isLeftHand ? leftControllerTrigger : rightControllerTrigger;
                    break;
                case EControllerButtonType.Grip:
                    buttonObject = isLeftHand ? leftControllerGrip : rightControllerGrip;
                    break;
                case EControllerButtonType.PrimaryButton:
                    buttonObject = isLeftHand ? leftControllerPrimaryButton : rightControllerPrimaryButton;
                    break;
                case EControllerButtonType.SecondaryButton:
                    buttonObject = isLeftHand ? leftControllerSecondaryButton : rightControllerSecondaryButton;
                    break;
            }

            // 设置按钮的显隐状态
            if (buttonObject != null)
            {
                buttonObject.SetActive(state);
            }
        }

        /// <summary>
        /// 隐藏所有手柄按钮高亮
        /// </summary>
        public void HideAllControllerButton()
        {
            foreach (EControllerButtonType buttonType in Enum.GetValues(typeof(EControllerButtonType)))
            {
                SetControllerButtonHighlight(buttonType, false, EHandType.Both);
            }
        }

        private float m_defaultReachDistance = 0.15f;
        public float DefaultReachDistance{get { return m_defaultReachDistance; }}
        /// <summary>
        /// 设置手部抓取的距离
        /// </summary>
        /// <param name="distance"></param>
        public void SetReachDistance(float distance)
        {
            m_playerController.player.handLeft.reachDistance = distance;
            m_playerController.player.handRight.reachDistance = distance;
            m_defaultReachDistance = distance;
        }
        
        /// <summary>
        /// 设置手部抓取的精度
        /// </summary>
        /// <param name="precision"></param>
        public void SetCollisionPrecision(Vector2 precision)
        {
            m_playerController.player.handLeft.collisionPrecision = precision;
            m_playerController.player.handRight.collisionPrecision = precision;
        }

        /// <summary>
        /// 设置手部投射器的显示隐藏
        /// </summary>
        /// <param name="state"></param>
        public void SetHandProjectorState(bool state = true)
        {
            m_LeftHandProjector.SetActive(state);
            m_RightHandProjector.SetActive(state);
        }

        /// <summary>
        /// 播放正确错误音效
        /// </summary>
        /// <param name="isRight"></param>
        public void PlaySound(bool isRight = true)
        {
            if (isRight)
            {
                AudioManager.Instance.PlaySound("right");
            }
            else
            {
                AudioManager.Instance.PlaySound("wrong");
            }
        }


        #region 震动

        /// <summary>
        /// 使手柄振动
        /// </summary>
        /// <param name="handType">可选：左手柄、右手柄、左右手柄一起</param>
        /// <param name="duration">振动持续时间</param>
        /// <param name="amplitude">振动强度[0-1]</param>
        /// <param name="frequency">振动频率</param>
        public void Haptic(EHandType handType, float duration = 0.2f, float amplitude = 1.0f, float frequency = 0)
        {
            switch (handType)
            {
                case EHandType.Left:
                    OpenXRInput.SendHapticImpulse(m_HapticActionLeft, amplitude, frequency, duration, XRController.leftHand);
                    break;
                case EHandType.Right:
                    OpenXRInput.SendHapticImpulse(m_HapticActionRight, amplitude, frequency, duration, XRController.rightHand);
                    break;
                case EHandType.Both:
                    OpenXRInput.SendHapticImpulse(m_HapticActionLeft, amplitude, frequency, duration, XRController.leftHand);
                    OpenXRInput.SendHapticImpulse(m_HapticActionRight, amplitude, frequency, duration, XRController.rightHand);
                    break;
            }
        }

        /// <summary>
        /// 停止手柄振动
        /// </summary>
        /// <param name="handType">可选：左手柄、右手柄、左右手柄一起</param>
        public void StopHaptic(EHandType handType = EHandType.Both)
        {
            switch (handType)
            {
                case EHandType.Left:
                    OpenXRInput.StopHaptics(m_HapticActionLeft, XRController.leftHand);
                    break;
                case EHandType.Right:
                    OpenXRInput.StopHaptics(m_HapticActionRight, XRController.rightHand);
                    break;
                case EHandType.Both:
                    OpenXRInput.StopHaptics(m_HapticActionLeft, XRController.leftHand);
                    OpenXRInput.StopHaptics(m_HapticActionRight, XRController.rightHand);
                    break;
            }
        }

        /// <summary>
        /// 做出正确或错误操作之后的 bHaptics 振动反馈。此类振动反馈以操作手为起点，因此有左右区分。
        /// </summary>
        /// <param name="isCorrect">操作是正确的还是错误的</param>
        /// <param name="handType">可选：左侧、右侧、左右两侧一起</param>
        public static void BHapticsJudgement(bool isCorrect, EHandType handType = EHandType.Both)
        {
            switch (handType)
            {
                case EHandType.Left:
                    BhapticsLibrary.Play(isCorrect ? BhapticsEvent.CORRECT_LEFT : BhapticsEvent.WRONG_LEFT);
                    break;
                case EHandType.Right:
                    BhapticsLibrary.Play(isCorrect ? BhapticsEvent.CORRECT_RIGHT : BhapticsEvent.WRONG_RIGHT);
                    break;
                case EHandType.Both:
                    BhapticsLibrary.Play(isCorrect ? BhapticsEvent.CORRECT_BOTH : BhapticsEvent.WRONG_BOTH);
                    break;
            }
        }

        #endregion


        #region 主菜单

        /// <summary>
        /// 更新主菜单状态
        /// </summary>
        /// <param name="status"></param>
        public void UpdatePanelState(DisplayStatus status = DisplayStatus.CloseAll)
        {
            if(mainMenu == null) return;

            if (isCanOpenMainMenu)
            {
                mainMenu.SetDisplayStatus(DisplayStatus.CloseAll);
            }
            else
            {
                mainMenu.SetDisplayStatus(status);
            }
        }

        /// <summary>
        /// 设置主菜单的偏移
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="offset"></param>
        public void SetMainMenuOffset(float distance, float offset)
        {
            mainMenu.m_panelDistance = distance;
            mainMenu.m_Offset = offset;
        }

        #endregion

    }
}