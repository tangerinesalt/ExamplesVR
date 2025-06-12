/****************************************************
    功能：物体单向移动控制
    作者：ZZQ
    创建日期：#2025/04/16#
    修改内容：
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using NaughtyAttributes;
using Unity.Mathematics;

namespace Voltage
{
    public enum MoveDirection
    {
        X,
        Y,
        Z
    }
    public class ReleaseCableAnimation : MonoBehaviour
    {
        /// <summary>
        /// 运动对象
        /// </summary>
        [Header("运动对象")]
        public Transform target;
        [Header("车轮旋转")]
        public Transform[] _TireParents;
        public float rotationSpeed = 180f;
        public bool isClockwise = false;
        //运行变量
        private Tween _MoveTween;
        private List<Transform> _Tires = new List<Transform>();
        private List<Tween> _TireRotations = new List<Tween>();
        void Awake()
        {
            if (target == null) target = transform;
            foreach (Transform parent in _TireParents)
            {
                foreach (Transform child in parent)
                {
                    if (child.name.Equals("Tire"))
                    {
                        _Tires.Add(child);
                    }
                }
            }
        }
        void OnDestroy()
        {
            if (_MoveTween != null) _MoveTween.Kill();
            StopWheelRotation();
        }
        /// <summary>
        /// 移动到指定坐标
        /// </summary>
        /// <param name="direction">方向</param>
        /// <param name="coordinates">坐标值</param>
        /// <param name="duration">持续时间</param>
        public Tween Move(MoveDirection direction, float coordinates, float duration, Ease ease = Ease.InOutQuad)
        {
            switch (direction)
            {
                case MoveDirection.X:
                    _MoveTween = target.transform.DOMoveX(coordinates, duration).SetEase(ease);
                    break;
                case MoveDirection.Y:
                    _MoveTween = target.transform.DOMoveY(coordinates, duration).SetEase(ease);
                    break;
                case MoveDirection.Z:
                    _MoveTween = target.transform.DOMoveZ(coordinates, duration).SetEase(ease);
                    break;
            }
            _MoveTween.onPlay += () => { StartWheelRotation(); };
            _MoveTween.onComplete += () => { StopWheelRotation(); };
            return _MoveTween;
        }
        public Tween[] StartWheelRotation()
        {
            float direction = isClockwise ? 1f : -1f;

            foreach (Transform tire in _Tires)
            {
                _TireRotations.Add(tire.transform.DOLocalRotate(
                    new Vector3(0, 0, 360 * direction), //角度
                    360f / rotationSpeed, //持续时间
                    RotateMode.FastBeyond360 //旋转模式——超过360度
                )
                .SetLoops(-1, LoopType.Restart) //循环次数，-1表示无限循环
                .SetEase(Ease.Linear)); //缓动类型_匀速
            }
            return _TireRotations.ToArray();
        }
        public void StopWheelRotation()
        {
            if (_TireRotations.Count == 0) return;

            foreach (Tween t in _TireRotations)
            {
                t.Kill();
            }
            _TireRotations.Clear();
        }
    }
}