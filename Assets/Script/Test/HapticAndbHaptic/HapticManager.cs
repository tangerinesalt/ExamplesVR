using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;
using UnityEngine.InputSystem.XR;
using Bhaptics.SDK2;
using Tangerine;

    public class HapticManager : MonoBehaviourSingleton<HapticManager>
    {
        protected override string _monoBehaviourName => "HapticManager";

        [SerializeField] private InputActionReference m_HapticActionLeft = null;
        [SerializeField] private InputActionReference m_HapticActionRight = null;

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
        
        public static void BHapticsJudgementLeft(bool isCorrect)
        {
            BHapticsJudgement(isCorrect, EHandType.Left);
        }
        
        public static void BHapticsJudgementRight(bool isCorrect)
        {
            BHapticsJudgement(isCorrect, EHandType.Right);
        }
        
        public static void BHapticsJudgementBoth(bool isCorrect)
        {
            BHapticsJudgement(isCorrect, EHandType.Both);
        }
    }
