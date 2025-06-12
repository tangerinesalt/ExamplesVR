/*******************************************************************************
Copyright © 2015-2022 PICO Technology Co., Ltd.All rights reserved.  
NOTICE：All information contained herein is, and remains the property of 
PICO Technology Co., Ltd. The intellectual and technical concepts 
contained herein are proprietary to PICO Technology Co., Ltd. and may be 
covered by patents, patents in process, and are protected by trade secret or 
copyright law. Dissemination of this information or reproduction of this 
material is strictly forbidden unless prior written permission is obtained from
PICO Technology Co., Ltd. 
修改人：ZZQ
修改日期：#2025/02/20#
修改内容：移除动画效果，使用代码控制PICO手柄的动画
*******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace Voltage
{
    public class ControllerAnimator : MonoBehaviour
    {
        public Transform primary2DAxisTran;
        public Transform gripTran;
        public Transform triggerTran;
        public Transform primaryButtonTran;
        public Transform secondaryButtonTran;
        public Controller controller;
        // private InputDevice currentController;
        private Vector2 axis2D = Vector2.zero;
        private float primaryButton;
        private float secondaryButton;
        private float primaryButtonRecord = 0;
        private float secondaryButtonRecord = 0;
        private float menuButton;
        private float grip;
        private float trigger;
        private Vector3 originalGrip;
        private Vector3 originalTrigger;
        private Vector3 originalJoystick;
        private Vector3 originalprimaryButtonValue;
        private Vector3 originalsecondaryButtonValue;
        private float ButtomChangeValue;

        [Space(20)]
        [SerializeField] private InputActionReference m_InputAxis2D = null;
        [SerializeField] private InputActionReference m_InputGrip = null;
        [SerializeField] private InputActionReference m_InputTrigger = null;
        [SerializeField] private InputActionReference m_InputPrimaryButton = null;
        [SerializeField] private InputActionReference m_InputSecondaryButton = null;
        [SerializeField] private InputActionReference m_InputMenuButton = null;



        public const string primary = "IsPrimaryDown";
        public const string secondary = "IsSecondaryDown";
        public const string media = "IsMediaDown";
        public const string menu = "IsMenuDown";

        void Start()
        {
            originalGrip = gripTran.localEulerAngles;
            originalJoystick = primary2DAxisTran.localEulerAngles;
            originalTrigger = triggerTran.localEulerAngles;
            originalprimaryButtonValue = primaryButtonTran.localPosition;
            originalsecondaryButtonValue = secondaryButtonTran.localPosition;

            if (controller == Controller.LeftController)
                ButtomChangeValue = -0.0018f;
            else
                ButtomChangeValue = 0.0018f;
        }

        void Update()
        {
            axis2D = m_InputAxis2D.action.ReadValue<Vector2>();

            float x = Mathf.Clamp(axis2D.x * 10f, -10f, 10f);
            float z = Mathf.Clamp(axis2D.y * 10f, -10f, 10f);
            if (primary2DAxisTran != null)
            {
                if (controller == Controller.LeftController)
                {
                    primary2DAxisTran.localEulerAngles = new Vector3(-z, 0, x) + originalJoystick;
                }
                else
                {
                    primary2DAxisTran.localEulerAngles = new Vector3(-z, 0, -x) + originalJoystick;
                }
            }

            trigger = m_InputTrigger.action.ReadValue<float>();
            trigger *= -15;
            if (triggerTran != null)
            {
                triggerTran.localEulerAngles = new Vector3(trigger, 0f, 0f) + originalTrigger;
            }
            grip = m_InputGrip.action.ReadValue<float>();
            grip *= 12;
            if (gripTran != null)
            {
                gripTran.localEulerAngles = new Vector3(0f, grip, 0f) + originalGrip;
            }

            primaryButton = m_InputPrimaryButton.action.ReadValue<float>();
            secondaryButton = m_InputSecondaryButton.action.ReadValue<float>();
            menuButton = m_InputMenuButton.action.ReadValue<float>();
            
            if (primaryButton != primaryButtonRecord || secondaryButton != secondaryButtonRecord)
            {
                switch (primaryButton)
                {
                    case 1:
                        primaryButtonTran.localPosition = new Vector3(originalprimaryButtonValue.x, ButtomChangeValue, originalprimaryButtonValue.z);
                        break;
                    default:
                        primaryButtonTran.localPosition = originalprimaryButtonValue;
                        break;
                }

                switch (secondaryButton)
                {
                    case 1:
                        secondaryButtonTran.localPosition = new Vector3(originalsecondaryButtonValue.x, ButtomChangeValue, originalsecondaryButtonValue.z);
                        break;
                    default:
                        secondaryButtonTran.localPosition = originalsecondaryButtonValue;
                        break;
                }
            }
            primaryButtonRecord = primaryButton;
            secondaryButtonRecord = secondaryButton;
        }
    }
    public enum Controller
    {
        LeftController,
        RightController
    }
}