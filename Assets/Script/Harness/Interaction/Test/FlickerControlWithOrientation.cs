/****************************************************
    功能：材质闪烁控制——根据计算方向的位置值大小
    作者：ZZQ
    创建日期：#2025/04/14#
    修改内容：1.0附加游戏对象显示控制的功能；1.1.优化代码结构；ZZQ #2024/04/17#
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using Unity.VisualScripting;

namespace Voltage
{
    public class FlickerControlWithOrientation : MonoBehaviour
    {
        //面板变量
        [Header("计算对象和方向")]
        /// <summary>
        /// 发光计算对象
        /// </summary>
        public Transform FlickerTarget = null;
        /// <summary>
        /// 显示计算对象
        /// </summary>
        public Transform ShowTarget = null;
        /// <summary>
        /// 计算方向（下拉框）
        /// </summary>
        [Dropdown("GetCalculationDirectionValues")]
        public Vector3 m_CalculationDirection;
        /// <summary>
        /// 发光组件父物体
        /// </summary>
        public Transform m_flickerParent = null;
        /// <summary>
        /// 显示对象父物体
        /// </summary>
        public Transform m_showParent = null;
        /// <summary>
        /// 计算时间间隔（秒）
        /// </summary>
        [Header("计算参数")][Range(0, 2f)]
        public float m_CalculateInterval = 2f;

        //运行变量
        /// <summary>
        /// 计算方向的下拉框选项和对应的Vector3值，默认值"-X"
        /// </summary>
        /// <returns></returns>
        private DropdownList<Vector3> GetCalculationDirectionValues()
        {
            return new DropdownList<Vector3>()
            {
                {"-X", Vector3.left},
                {"X", Vector3.right},
                {"-Y", Vector3.down},
                {"Y", Vector3.up},
                {"-Z", Vector3.back},
                {"Z", Vector3.forward}
            };
        }
        /// <summary>
        /// 所有发光组件的排序（位置值从小到大排列）列表
        /// </summary>
        private SortedList<float, MaterialFlicker> _flickerSortedList = new SortedList<float, MaterialFlicker>();
        /// <summary>
        /// 所有显示对象的排序（位置值从小到大排列）列表
        /// </summary>
        private SortedList<float, Transform> _transformSortedList = new SortedList<float, Transform>();
        private int _FlashedIndex = 0;//已闪烁对象的索引
        private int _ShownIndex = 0;//已显示对象的索引
        private Coroutine _flickerCoroutine = null;
        private bool _isFlickerCalculation = false;
        private float currentFlickerTargetPoint;
        private float currentShowTargetPoint;
        private float previousFlickerTargetPoint;

        private void Start()
        {
            if (FlickerTarget == null) { Debug.LogError("Target is null", this) ; return;}
            currentFlickerTargetPoint = Vector3.Dot(FlickerTarget.transform.position, m_CalculationDirection);
            previousFlickerTargetPoint = currentFlickerTargetPoint;

            InitTransforms();
            InitMaterialFlickers();
            StartFlickerCalculation();
        }
        private void OnDestroy()
        {
            StopFlickerCalculation();
        }
        private void InitMaterialFlickers()
        {
            MaterialFlicker[] _flickers = m_flickerParent.transform.GetComponentsInChildren<MaterialFlicker>(true);
            _flickerSortedList.Clear();
            //一次性添加所有发光器的位置值和引用，
            foreach (MaterialFlicker flicker in _flickers)
            {
                float flickerPositionValue = Vector3.Dot(flicker.transform.position, m_CalculationDirection);
                float _flickerPositionValue = GetFlickerPositionValue(flickerPositionValue);
                _flickerSortedList.Add(_flickerPositionValue, flicker);

            }
        }
        private void InitTransforms()
        {
            Transform[] _children = m_showParent.transform.GetComponentsInChildren<Transform>(true);
            _transformSortedList.Clear();
            foreach (Transform child in _children)
            {
                float childPositionValue = Vector3.Dot(child.position, m_CalculationDirection);
                float _childPositionValue = GetTransformPositionValue(childPositionValue);
                _transformSortedList.Add(_childPositionValue, child);

            }
        }
        /// <summary>
        /// 开始发光计算
        /// </summary>
        [Button("开始发光计算")]
        public void StartFlickerCalculation()
        {
            _isFlickerCalculation = true;
            _flickerCoroutine = StartCoroutine(FlickerCalculation());//使用协程实现、减少主线程的压力
        }
        /// <summary>
        /// 停止发光计算
        /// </summary>
        [Button("停止发光计算")]
        public void StopFlickerCalculation()
        {
            if (_flickerCoroutine != null)
            {
                _isFlickerCalculation = false;
                StopCoroutine(_flickerCoroutine);
                _flickerCoroutine = null;
            }
        }

        private IEnumerator FlickerCalculation()
        {
            yield return null;

            while (_isFlickerCalculation)
            {
                currentFlickerTargetPoint = Vector3.Dot(FlickerTarget.transform.position, m_CalculationDirection);
                currentShowTargetPoint = Vector3.Dot(ShowTarget.transform.position, m_CalculationDirection);
                if (currentFlickerTargetPoint != previousFlickerTargetPoint)
                {
                    //发光内容计算
                    for (int i = _FlashedIndex; i < _flickerSortedList.Keys.Count; i++)
                    {
                        if (currentFlickerTargetPoint > _flickerSortedList.Keys[i])
                        {
                            if (_flickerSortedList.Values[i].IsFlicker == false)
                            {
                                _flickerSortedList.Values[i].gameObject?.SetActive(true);
                                _flickerSortedList.Values[i].SetFlickerState(true);
                                _FlashedIndex = i;
                            }
                        }
                        else
                            break;//对于固定位置的材质发光器，已按照位置排序，因此只需要计算到最近的材质发光器即可
                    }
                    //显示内容计算
                    for (int i = _ShownIndex; i < _transformSortedList.Keys.Count; i++)
                    {
                        if (currentShowTargetPoint > _transformSortedList.Keys[i])
                        {
                            if (_transformSortedList.Values[i].gameObject.activeSelf == false)
                            {
                                _transformSortedList.Values[i].gameObject.SetActive(true);
                                _ShownIndex = i;
                            }
                        }
                        else
                            break;
                    }

                    previousFlickerTargetPoint = currentFlickerTargetPoint;
                }
                //计算间隔
                if (m_CalculateInterval!=0)
                    yield return new WaitForSeconds(m_CalculateInterval);
                else
                    yield return null;
            }
            yield return null;
        }

        #region Tools
        private float GetFlickerPositionValue(float positionValue)
        {
            if (!_flickerSortedList.ContainsKey(positionValue))
                return positionValue;
            else
            {
                float _flickerPositionValue = positionValue + 0.001f;
                if (_flickerSortedList.ContainsKey(_flickerPositionValue))
                    _flickerPositionValue = GetFlickerPositionValue(_flickerPositionValue);
                return _flickerPositionValue;
            }
        }
        private float GetTransformPositionValue(float positionValue)
        {
            if (!_transformSortedList.ContainsKey(positionValue))
                return positionValue;
            else
            {
                float _transformPositionValue = positionValue + 0.001f;
                if (_transformSortedList.ContainsKey(_transformPositionValue))
                    _transformPositionValue = GetTransformPositionValue(_transformPositionValue);
                return _transformPositionValue;
            }
        }
        #endregion
    }
}