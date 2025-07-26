using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

namespace Voltage
{
    [System.Serializable]
    public class BoneControlPoint
    {
        [SerializeField] public Transform ControlPointTF;

        [SerializeField] public float strength = 10f;
        [Dropdown("m_stringDirectionValue")]
        [SerializeField] public String direction; private List<string> m_stringDirectionValue { get { return new List<string> { "X", "Y", "Z", "-X", "-Y", "-Z" }; } }

    }
    public class SkeletalAnimation : MonoBehaviour
    {
        /// <summary> 垂直方向偏移 </summary>
        [Range(-1, 1)]
        [SerializeField] public float m_verticalOffsetValue = 0;
        /// <summary> 水平方向偏移 </summary>
        [Range(-1, 1)]
        [SerializeField] public float m_horizontalOffsetValue = 0;

        [Header("垂直方向骨骼控制组")]
        [SerializeField] private BoneControlPoint[] m_verticalBCPGroup = null;

        [Header("水平方向骨骼控制组")]
        [SerializeField] private BoneControlPoint[] m_horizontalBCPGroup = null;
        private Dictionary<BoneControlPoint, Vector3> m_defaultVerticalAngleGroup = null;
        private Dictionary<BoneControlPoint, Vector3> m_defaultHorizontalAngleGroup = null;
        //内部变量——上一次的偏移值
        private float m_OldVerticalOffsetValue;
        private float m_OldHorizontalOffsetValue;
        private void Start()
        {
            GetDefaultAngle();
        }
        // Update is called once per frame
        void Update()
        {
            SetSkeletalGroupState(m_verticalOffsetValue, m_horizontalOffsetValue);
        }
        /// <summary>
        /// 根据同一参数设置骨骼组状态
        /// </summary>
        /// <param name="verticalInput">垂直偏移值</param>
        /// <param name="horizontalInput">水平偏移值</param>s
        public void SetSkeletalGroupState(float verticalInput, float horizontalInput)
        {
            if (horizontalInput == m_OldHorizontalOffsetValue && verticalInput == m_OldVerticalOffsetValue)
            {
                return;
            }

            Dictionary<Transform, Vector3> FinalAngleGroup = new Dictionary<Transform, Vector3>();
            
            //添加垂直组变换和影响后的角度值
            if (verticalInput != m_OldVerticalOffsetValue)
            {
                foreach (BoneControlPoint BCP in m_defaultVerticalAngleGroup.Keys)
                {
                    Vector3 FinalAngle = TransformRotate(BCP, m_defaultVerticalAngleGroup[BCP], verticalInput);
                    FinalAngleGroup.Add(BCP.ControlPointTF, FinalAngle);
                }
            }
            //添加水平组变换和影响后的角度值
            if (horizontalInput != m_OldHorizontalOffsetValue)
            {
                foreach (BoneControlPoint BCP in m_defaultHorizontalAngleGroup.Keys)
                {
                    //如果垂直组包含ControlPointTF，(已运算)则跳过该ControlPointTF的动画
                    if (FinalAngleGroup.ContainsKey(BCP.ControlPointTF))
                    {
                        FinalAngleGroup[BCP.ControlPointTF] = TransformRotate(BCP, FinalAngleGroup[BCP.ControlPointTF], horizontalInput);
                    }
                    else
                    {
                        Vector3 FinalAngle = TransformRotate(BCP, m_defaultHorizontalAngleGroup[BCP], horizontalInput);
                        FinalAngleGroup.Add(BCP.ControlPointTF, FinalAngle);
                    }
                }
            }
            //全部骨骼组统一执行旋转变换
            foreach (Transform TF in FinalAngleGroup.Keys)
            {
                TF.localEulerAngles = FinalAngleGroup[TF];
            }
            //记录当前偏移值
            m_OldVerticalOffsetValue = verticalInput;
            m_OldHorizontalOffsetValue = horizontalInput;

        }
        //获得骨骼控制节点的初始角度和节点变换，分别存入字典方便调用
        private void GetDefaultAngle()
        {
            m_defaultVerticalAngleGroup = new Dictionary<BoneControlPoint, Vector3>();
            m_defaultHorizontalAngleGroup = new Dictionary<BoneControlPoint, Vector3>();
            if (m_verticalBCPGroup == null || m_horizontalBCPGroup == null)
            {
                Debug.LogError("Vertical or Horizontal Bones are null");
            }
            for (int i = 0; i < m_verticalBCPGroup.Length; i++)
            {
                if (m_verticalBCPGroup[i].ControlPointTF != null)
                {
                    m_defaultVerticalAngleGroup.Add(m_verticalBCPGroup[i], m_verticalBCPGroup[i].ControlPointTF.localEulerAngles);
                }
            }
            for (int i = 0; i < m_horizontalBCPGroup.Length; i++)
            {
                if (m_horizontalBCPGroup[i].ControlPointTF != null)
                {
                    m_defaultHorizontalAngleGroup.Add(m_horizontalBCPGroup[i], m_horizontalBCPGroup[i].ControlPointTF.localEulerAngles);
                }
            }
        }
        /// <summary>
        /// 旋转骨骼
        /// </summary>
        /// <param name="TargetTF">目标骨骼</param>
        /// <param name="defualtAngle">初始默认角度</param>
        /// <param name="OffsetValue">偏移角度</param>
        private Vector3 TransformRotate(BoneControlPoint TargetTF, Vector3 defualtAngle, float OffsetValue)
        {

            Vector3 FinalAngle = defualtAngle;
            switch (TargetTF.direction)
            {
                case "X":
                    FinalAngle.x += OffsetValue * TargetTF.strength;
                    break;
                case "Y":
                    FinalAngle.y += OffsetValue * TargetTF.strength;
                    break;
                case "Z":
                    FinalAngle.z += OffsetValue * TargetTF.strength;
                    break;
                case "-X":
                    FinalAngle.x -= OffsetValue * TargetTF.strength;
                    break;
                case "-Y":
                    FinalAngle.y -= OffsetValue * TargetTF.strength;
                    break;
                case "-Z":
                    FinalAngle.z -= OffsetValue * TargetTF.strength;
                    break;
            }
            return FinalAngle;
        }
    }
}
