using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR.Input;
using Voltage;
using Bhaptics.SDK2;

namespace Voltage
{
    public enum ECatchAction
    {
        Catch,
        Release,
    }
    
    public class Catcher : MonoBehaviour
    {
        public class CatchableInfo
        {
            public int id;
            public Catchable catchable;
            public int count;
        }
        
        [SerializeField] private EHandType m_HandType = EHandType.Left;
        [SerializeField] private Transform m_CatchPoint = null;
        [SerializeField] private InputActionReference m_CatchAction = null;
        [SerializeField] private Animator m_HandAnimator = null;

        private int _curCatchableID = 0;
        private SortedList<int, CatchableInfo> _catchableSList = new SortedList<int, CatchableInfo>();
        private Dictionary<Catchable, CatchableInfo> _catchableDic = new Dictionary<Catchable, CatchableInfo>();

        private bool _isCatching = false;
        public bool IsCatching => _isCatching;
        private Catchable _curCatching = null;
        public Catchable GetCurCatching => _curCatching;
        private Catchable _toCatch = null;
        
        public EHandType HandType => m_HandType;
        public Transform CatchPoint => m_CatchPoint;
        

        // Start is called before the first frame update
        void Start()
        {
            if (null == m_CatchPoint) m_CatchPoint = transform;

            if (null != m_CatchAction && null != m_CatchAction.action)
            {
                m_CatchAction.action.started += OnActionStarted;
                m_CatchAction.action.performed += OnActionPerformed;
                m_CatchAction.action.canceled += OnActionCanceled;
            }
        }

        private void OnDestroy(){
            if (null != m_CatchAction && null != m_CatchAction.action)
            {
                m_CatchAction.action.started -= OnActionStarted;
                m_CatchAction.action.performed -= OnActionPerformed;
                m_CatchAction.action.canceled -= OnActionCanceled;
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void OnActionStarted(InputAction.CallbackContext ctx)
        {
            //MainManager.Instance.CatchAction(m_HandType, ECatchAction.Catch);
            m_HandAnimator.SetBool("isCatching", true);
            _isCatching = true;
            
            if (_curCatching == null && _toCatch != null)
                _toCatch.Catch(this);
        }
        
        public void CatchSuccessfully(Catchable catchable)
        {
            if (_isCatching)
            {
                if (_curCatching != null && _curCatching != catchable)
                {
                    _curCatching.Release();
                }
                _curCatching = catchable;
            }
            else
            {
                catchable.Release();
            }
        }

        void OnActionPerformed(InputAction.CallbackContext ctx) {}

        void OnActionCanceled(InputAction.CallbackContext ctx)
        {
            // MainManager.Instance.CatchAction(m_HandType, ECatchAction.Release);
            // if(NetAppManager.Instance.SimulationType == ESimulationType.OperationGuide)
            // {
            //     if(MainManager.Instance.HandShowing)
            //     MainManager.Instance.SetControllerModelVisible(true,m_HandType);
            // }
            m_HandAnimator.SetBool("isCatching", false);
            _isCatching = false;
            
            if (_curCatching != null) _curCatching.Release();
            _curCatching = null;
        }

        public void DeCatch(Catchable catchable)
        {
            if (_curCatching == catchable) _curCatching = null;
        }


        private void OnTriggerEnter(Collider other)
        {
            Catchable catchable = other.GetComponentInParent<Catchable>();
            if (null == catchable) return;

            if (_catchableDic.ContainsKey(catchable))
            {
                _catchableDic[catchable].count += 1;
                return;
            }
            
            if (_curCatching == null)
            {
                //HapticManager.Instance.Haptic(m_HandType);
                switch (m_HandType)
                {
                    case EHandType.Left:
                        BhapticsLibrary.Play(BhapticsEvent.TOUCH_CATCHABLE_LEFT);
                        break;
                    case EHandType.Right:
                        BhapticsLibrary.Play(BhapticsEvent.TOUCH_CATCHABLE_RIGHT);
                        break;
                }
            }

            if (_toCatch != null) _toCatch.NextCatchCancelled(m_HandType);
            catchable.NextCatch(m_HandType);

            CatchableInfo catchableInfo = new CatchableInfo
            {
                id = _curCatchableID,
                catchable = catchable,
                count = 1,
            };

            _catchableSList.Add(catchableInfo.id, catchableInfo);
            _catchableDic[catchable] = catchableInfo;
            _curCatchableID++;
            _toCatch = catchable;
        }

        private void OnTriggerExit(Collider other)
        {
            Catchable catchable = other.GetComponentInParent<Catchable>();
            if (null == catchable) return;

            RemoveCatchable(catchable);
        }

        private void RemoveCatchable(Catchable catchable, bool removeCompletely = false)
        {
            if (catchable == null || !_catchableDic.ContainsKey(catchable)) return;
            if (!removeCompletely && _catchableDic[catchable].count > 1)
            {
                _catchableDic[catchable].count -= 1;
                return;
            }
            
            _catchableSList.Remove(_catchableDic[catchable].id);
            _catchableDic.Remove(catchable);
            int catchableCount = _catchableSList.Count;
            if (_toCatch == catchable)
            {
                catchable.NextCatchCancelled(m_HandType);
                if (catchableCount == 0)
                {
                    _toCatch = null;
                }
                else
                {
                    _toCatch = _catchableSList.Values[catchableCount - 1].catchable;
                    _toCatch.NextCatch(m_HandType);
                }
            }
        }

        public void CatchableUnavailable(Catchable catchable)
        {
            RemoveCatchable(catchable, true);
            DeCatch(catchable);
        }

        public bool HaveCatchable(){
            return _catchableDic.Count != 0;
        }
    }
}
