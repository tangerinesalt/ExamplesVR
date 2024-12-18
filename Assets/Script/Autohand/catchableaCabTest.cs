using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using Voltage;
using Autohand;

namespace Voltage
{
    public class catchableaCabTest : Catchable
    {
        [Space]
        [SerializeField] private Grabbable_Voltage m_grabbable=null;
        [SerializeField] private bool m_disablePhysicsExceptGrabbing = true;

        [Space(6)]
        [SerializeField] private Transform m_CabRoot = null;
        [SerializeField] private Animator m_CabAni = null;
        [SerializeField] private Transform m_HandleOriginPos = null;
        [SerializeField] private float m_OpenSensitivity = 10;
        [SerializeField] private float m_ShearSensitivity = 10;
        [SerializeField] private Transform m_ResetPos = null;
        [SerializeField] private MeshRenderer m_Renderer = null;
        
        [Space(6)]
        [SerializeField] private List<GameObject> m_SocketList = null;
        [SerializeField] private float m_ShowSocketThreshold = 0.6f;
        [SerializeField] private UnityEvent m_OnFirstOpen = null;
        [SerializeField, Tooltip("- Server Event -")] private UnityEvent m_OnPlugAll = null;
        
        [Space(6)]
        [SerializeField] private Rect m_FinishRange;
        [SerializeField, Tooltip("- Server Event -")] private UnityEvent m_OnFinish = null;
        
        private readonly SyncHashSet<int> _notPluggedSocketIndexSyncSet = new SyncHashSet<int>();
        
        private Vector3 _handleStartPos = Vector3.zero;
        private float _openValue = 0;
        private float _shearValue = 0;
        private Dictionary<int, GameObject> _notPluggedSocketDic = new Dictionary<int, GameObject>();
        private bool _firstOpened = false;
        private bool _pluggedAll = false;
        private CollisionDetectionMode RigidbodyDetectionMode;
        
        protected override void Start()
        {
            m_RootTrans.position = m_ResetPos.position;
            m_RootTrans.rotation = m_ResetPos.rotation;
            if (m_grabbable!= null)
            {
                m_grabbable.onGrab.AddListener(OnGrab);
                m_grabbable.onRelease .AddListener(OnRelease);
            }
            base.Start();
            
            for (int i = 0; i < m_SocketList.Count; ++i)
            {
                _notPluggedSocketDic.Add(i, m_SocketList[i]);
            }
            
            _handleStartPos = m_CabRoot.InverseTransformPoint(m_HandleOriginPos.position);
        }
        
        public override void OnStartServer()
        {
            base.OnStartServer();
            
            for (int i = 0; i < m_SocketList.Count; ++i)
            {
                _notPluggedSocketIndexSyncSet.Add(i);
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            _notPluggedSocketIndexSyncSet.Callback += OnNotPluggedSocketIndexSyncSetUpdate;
        }

        protected override void Update()
        {
            base.Update();
           
            
            if (isServer)
            {
                if (!_pluggedAll)
                {
                    if (_notPluggedSocketDic.Count == 0)
                    {
                        m_OnPlugAll.Invoke();
                        _pluggedAll = true;
                    }
                }
                
                if (m_grabbable.isgrabbed)
                {
                    Vector3 handlePos = m_CabRoot.InverseTransformPoint(m_RootTrans.position);
                    Vector3 handleDelta = handlePos - _handleStartPos;
                    float targetOpenValue = Mathf.Clamp(handleDelta.z * m_OpenSensitivity * -1, -1, 1);
                    float targetShearValue = Mathf.Clamp(handleDelta.y * m_ShearSensitivity, -1, 1);
                    
                    // Cab 挂载物的放置
                    if (!_pluggedAll)
                    {
                        if (_openValue < m_ShowSocketThreshold && targetOpenValue >= m_ShowSocketThreshold)
                        {
                            // 判定 Cab 张得足够开，可以往里面放东西
                            SetSocketActive(true);
                            if (!_firstOpened)
                            {
                                m_OnFirstOpen.Invoke();
                                _firstOpened = true;
                            }
                        }
                        else if (_openValue >= m_ShowSocketThreshold && targetOpenValue < m_ShowSocketThreshold)
                        {
                            // 判定 Cab 张得不够开，不能往里面放东西
                            SetSocketActive(false);
                        }
                    }
                    
                    m_CabAni.SetFloat("Open", targetOpenValue);
                    m_CabAni.SetFloat("Shear", targetShearValue);
                    _openValue = targetOpenValue;
                    _shearValue = targetShearValue;
                    
                    // 闭合 Cab
                    if (_pluggedAll)
                    {
                        if (m_FinishRange.Contains(new Vector2(_openValue, _shearValue)))
                        {
                            m_OnFinish.Invoke();
                            //HapticNetManager.Instance.BHapticsJudgement(CatcherPlayerId, true, LastCatchHand);
                            m_CabAni.SetFloat("Open", 0);
                            m_CabAni.SetFloat("Shear", 0);
                            S_SetActive(false);
                        }
                    }
                }
            }
        }
        
        private void OnEnable()
        {
            if (isServer && RootNetTrans != null) RootNetTrans.S_Teleport(m_ResetPos.position, m_ResetPos.rotation);
            if (m_grabbable.body != null&& m_disablePhysicsExceptGrabbing)
            {
                RigidbodyDetectionMode = m_grabbable.body.collisionDetectionMode;
                DisablePhysicsExceptGrabbing();
            }
            
        }

        [ClientRpc]
        protected override void Rpc_OnCatchSucceed()
        {
            base.Rpc_OnCatchSucceed();
            
            m_Renderer.enabled = false;
        }
        private void OnRelease(Hand hand, Grabbable grab)
        {
            if (m_grabbable.body != null&& m_disablePhysicsExceptGrabbing)
            DisablePhysicsExceptGrabbing();

            RootNetTrans.S_Teleport(m_ResetPos.position, m_ResetPos.rotation);
            m_Renderer.enabled = true;
        }
        private void OnGrab(Hand hand, Grabbable grab)
        {
            if (m_grabbable.body != null&& m_disablePhysicsExceptGrabbing)
            EnabledPhysicsInGrabbing();  

            m_Renderer.enabled = false;
        }

        [Server]
        protected override void S_OnReleaseSucceed()
        {
            base.S_OnReleaseSucceed();
            
            if (RootNetTrans != null) RootNetTrans.S_Teleport(m_ResetPos.position, m_ResetPos.rotation);
        }

        [ClientRpc]
        protected override void Rpc_OnReleaseSucceed()
        {
            base.Rpc_OnReleaseSucceed();
            
            m_Renderer.enabled = true;
        }

        [Server]
        public void SocketPlugged(GameObject socketGO)
        {
            foreach (var kvp in _notPluggedSocketDic)
            {
                if (kvp.Value == socketGO)
                {
                    if (_notPluggedSocketIndexSyncSet.Contains(kvp.Key))
                    {
                        _notPluggedSocketIndexSyncSet.Remove(kvp.Key);
                        break;
                    }
                }
            }
        }
        
        [ClientRpc]
        private void SetSocketActive(bool active)
        {
            foreach (var kvp in _notPluggedSocketDic)
            {
                kvp.Value.SetActive(active);
            }
        }
        private void DisablePhysicsExceptGrabbing()
        {
            if (m_grabbable.body != null)
            {
                RigidbodyDetectionMode = m_grabbable.body.collisionDetectionMode;
                m_grabbable.body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                m_grabbable.body.isKinematic = true;
            }
        }
        private void EnabledPhysicsInGrabbing()
        {
            if (m_grabbable.body != null)m_grabbable.body.isKinematic = false;
            m_grabbable.body.collisionDetectionMode = RigidbodyDetectionMode;
        }
        private void OnNotPluggedSocketIndexSyncSetUpdate(SyncSet<int>.Operation op, int value)
        {
            if (op == SyncSet<int>.Operation.OP_REMOVE)
            {
                if (_notPluggedSocketDic.ContainsKey(value))
                {
                    _notPluggedSocketDic[value].SetActive(false);
                    _notPluggedSocketDic.Remove(value);
                }
            }
        }
    }
}
