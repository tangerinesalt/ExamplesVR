using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Autohand;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

namespace Voltage
{
    /// <summary>  挂钩状态  </summary>
    public enum ETiesState
    {
        OpenAll,
        CloseAll
    }
    public class TiesControl : MonoBehaviour
    {
        [Header("Ties")]
        [SerializeField] private Transform Ties;

        [Header("控制对象和定位")]
        [SerializeField] private Transform ControlObjTF = null;
        [SerializeField] private Transform anchorPoint;
        [SerializeField, BoxGroup("垂直方向输入参数")] private Vector3 verticalDirection = Vector3.back;
        [SerializeField, BoxGroup("垂直方向输入参数")] private float verticalSensitivity = 8f;
        [SerializeField, BoxGroup("水平方向输入参数")] private Vector3 horizontalDirection = Vector3.right;
        [SerializeField, BoxGroup("水平方向输入参数")] private float horizontalSensitivity = 8f;

        /// <summary> 挂钩是否允许完全打开 </summary>
        public bool isAllowOpenAll = false;
        /// <summary> 挂钩是否允许完全关闭 </summary>
        public bool isAllowCloseAll = false;
        public bool checkOnRelease = true;
        [HideInInspector] public UnityEvent onOpenAll = new UnityEvent();
        [HideInInspector] public UnityEvent onCloseAll = new UnityEvent();


        /// <summary> 挂钩控制的计算点位 </summary>
        private Vector3 OldControlPos;
        /// <summary> 挂钩骨骼组当前垂直值 </summary>
        private float CurVerticalValue = 0f;
        /// <summary> 挂钩骨骼组当前水平值 </summary>
        private float CurHorizontalValue = 0f;
        /// <summary> 计算挂钩骨骼组动画的协程 </summary>
        private Coroutine m_AnimatorCoroutine;

        #region Unity生命周期
        void Awake()
        {
            if ( Ties == null)
            {
                Debug.LogError("检查挂钩是否为空");
            }
            if (ControlObjTF == null)
            {
                Debug.LogError("检查挂钩控制物是否为空");
            }
            if (anchorPoint == null)
            {
                Debug.LogError("检查挂钩控制物的定位点是否为空");
            }
            ResetControlObjTF();
        }
        void Start()
        {
            //初始化挂钩骨骼组状态
            CurVerticalValue = 0f;
            CurHorizontalValue = 0f;
            //初始化挂钩控制状态
            AllowOpenAll(false);
            AllowCloseAll(false);
            CloseControl();

            ControlObjTF.GetComponent<Grabbable>().onGrab.AddListener(OnGrab);
            ControlObjTF.GetComponent<Grabbable>().onRelease.AddListener(OnRelease);
        }
        void OnDestroy()
        {
            ControlObjTF.GetComponent<Grabbable>().onGrab.RemoveListener(OnGrab);
            ControlObjTF.GetComponent<Grabbable>().onRelease.RemoveListener(OnRelease);
        }
        #endregion
        #region 挂钩相关功能
        [Button("打开控制")]
        public void OpenControl()
        {
            SetControlObjState(true);
        }
        [Button("关闭控制")]
        public void CloseControl()
        {
            SetControlObjState(false);
        }

        /// <summary> 设置挂钩是否可以被控制 </summary>
        public void SetControlObjState(bool isOpen)
        {
            ControlObjTF.gameObject.SetActive(isOpen);
        }
        /// <summary> 设置挂钩是否允许完全打开 </summary>
        public bool AllowOpenAll(bool allowopen)
        {
            isAllowOpenAll = allowopen;
            return isAllowOpenAll;
        }
        /// <summary> 设置挂钩是否允许完全关闭 </summary>
        public bool AllowCloseAll(bool allowclose)
        {
            isAllowCloseAll = allowclose;
            return isAllowCloseAll;
        }
        private void OnOpen()
        {
            CloseControl();
            AllowOpenAll(false);
            SetTiesState(ETiesState.OpenAll);
            onOpenAll?.Invoke();
        }
        private void OnClose()
        {
            CloseControl();
            AllowCloseAll(false);
            SetTiesState(ETiesState.CloseAll);
            onCloseAll?.Invoke();
        }

        #endregion
        #region Grabbable控制
        void OnGrab(Hand hand, Grabbable grabbable)
        {
            //显示内容刷新
            grabbable.GetComponent<Renderer>().enabled = false;
            PlayerManager.Instance.SetHandModelState(hand.left ? EHandType.Left : EHandType.Right, false);

            //开始计算骨骼动画
            m_AnimatorCoroutine = StartCoroutine(SkeletalAnimator(CurVerticalValue, CurHorizontalValue));
        }
        void OnRelease(Hand hand, Grabbable grabbable)
        {
            //显示内容刷新
            grabbable.GetComponent<Renderer>().enabled = true;
            PlayerManager.Instance.SetHandModelState(EHandType.Both, true);

            //停止骨骼动画
            if (m_AnimatorCoroutine != null)
            {
                StopCoroutine(m_AnimatorCoroutine);
                m_AnimatorCoroutine = null;
            }
            //刷新控制对象位置
            ResetControlObjTF();

            //释放时检测挂钩是否完全打开或者完全关闭
            if (checkOnRelease)
            {
                //检测挂钩是否完全打开
                if (isAllowOpenAll)
                {
                    if (CurVerticalValue > 0.9f)
                    {
                        if (grabbable.heldBy.Count > 0)
                            grabbable.ForceHandsRelease();
                        OnOpen();
                    }
                }
                //检测挂钩是否完全关闭
                if (isAllowCloseAll)
                {
                    if (math.abs(CurVerticalValue) < 0.1f)
                    {
                        if (grabbable.heldBy.Count > 0)
                            grabbable.ForceHandsRelease();
                        OnClose();
                    }
                }
            }
        }
        /// <summary>
        /// 计算控制对象的垂直和水平值并设置骨骼组状态
        /// </summary>
        /// <param name="_curVerticalValue"></param>
        /// <param name="_curHorizontalValue"></param>
        /// <returns></returns>
        IEnumerator SkeletalAnimator(float _curVerticalValue, float _curHorizontalValue)
        {
            yield return null;
            // 检查 Ties 和 ControlObj 是否为空
            if (Ties == null || ControlObjTF == null)
            {
                Debug.LogError("Ties 或 ControlObj 为空，无法执行 RetrunValue 协程");
                yield break;
            }
            // 缓存组件
            Grabbable grabbable = ControlObjTF.GetComponent<Grabbable>();
            SkeletalAnimation TieAnimation = Ties.GetComponent<SkeletalAnimation>();
            // 检查组件是否存在
            if (grabbable == null || TieAnimation == null)
            {
                Debug.LogError("Grabbable 或 CabAnimation 组件不存在，无法执行 RetrunValue 协程");
                yield break;
            }

            Debug.Log("初始——垂直值：" + _curVerticalValue + "水平值：" + _curHorizontalValue);

            while (grabbable.heldBy.Count > 0)
            {
                Vector3 CurControlPos = ControlObjTF.localPosition;
                if (CurControlPos != OldControlPos)
                {
                    Vector3 DeltaPos = CurControlPos - OldControlPos;
                    CurVerticalValue = Mathf.Clamp(Vector3.Dot(DeltaPos, verticalDirection) * verticalSensitivity + _curVerticalValue, -1, 1);
                    CurHorizontalValue = Mathf.Clamp(Vector3.Dot(DeltaPos, horizontalDirection) * horizontalSensitivity + _curHorizontalValue, -1, 1);
                    //更新骨骼组状态
                    SetSkeletalGroupState(CurVerticalValue, CurHorizontalValue);
                }
                yield return new WaitForEndOfFrame();
                
                //动画运行中检测挂钩是否完全打开或完全关闭
                if (!checkOnRelease)
                {
                    //检测挂钩是否完全打开
                    if (isAllowOpenAll)
                    {
                        if (CurVerticalValue > 0.9f)
                        {
                            if (grabbable.heldBy.Count > 0)
                                grabbable.ForceHandsRelease();
                            OnOpen();
                        }
                    }
                    //检测挂钩是否完全关闭
                    if (isAllowCloseAll)
                    {
                        if (math.abs(CurVerticalValue) < 0.1f)
                        {
                            if (grabbable.heldBy.Count > 0)
                                grabbable.ForceHandsRelease();
                            OnClose();
                        }
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
        #endregion
        #region 骨骼动画控制相关
        /// <summary>
        /// 设置骨骼组状态
        /// </summary>
        /// <param name="state"></param>
        public void SetTiesState(ETiesState state)
        {
            switch (state)
            {
                case ETiesState.OpenAll:
                    SetSkeletalGroupState(1, 0);
                    break;
                case ETiesState.CloseAll:
                    SetSkeletalGroupState(0, 0);
                    break;
            }
            ResetControlObjTF();
        }
        /// <summary>
        /// 更新骨骼组状态
        /// </summary>
        /// <param name="_verticalValue">骨骼组垂直值</param>
        /// <param name="_horizontalValue">骨骼组水平值</param>
        private void SetSkeletalGroupState(float _verticalValue, float _horizontalValue)
        {
            //目前通过动画组件的update方式更新骨骼组状态
            Ties.GetComponent<SkeletalAnimation>().m_verticalOffsetValue = _verticalValue;
            Ties.GetComponent<SkeletalAnimation>().m_horizontalOffsetValue = _horizontalValue;
        }
        /// <summary> 重置挂钩控制物的空间位置和计算点位 </summary>
        private void ResetControlObjTF()
        {
            ControlObjTF.position = anchorPoint.position;
            ControlObjTF.rotation = anchorPoint.rotation;

            //刷新位移计算初始位置
            OldControlPos = ControlObjTF.localPosition;
        }
        #endregion
    }
}

