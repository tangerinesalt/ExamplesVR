using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Mirror;
using Voltage;

namespace Voltage
{
    public class Catchable : NetworkBehaviour
    {
        [SerializeField] protected bool m_KeepRelativePos = false;
        [SyncVar(hook = nameof(Sync_m_IsCatchable))]
        [SerializeField] protected bool m_IsCatchable = true;
        [SerializeField] protected bool m_IsAllowSwitchHand = true;
        [SerializeField] protected Transform m_RootTrans = null;
        [SerializeField] protected List<Outline> m_OutlineList = null;
        [SerializeField] protected List<Collider> m_TriggerList = null;
        
        private NetworkTransformBase _rootNetTrans = null;

        protected Catcher _catcher = null;

        [SyncVar]
        protected EHandType _lastCatchHand = EHandType.None;
        private Dictionary<Catcher, int> _catcherDic = new Dictionary<Catcher, int>();

        protected Transform _followedTrans = null;
        protected Vector3 _selfLocalPos = Vector3.zero;
        protected Vector3 _selfLocalForward = Vector3.zero;
        protected Vector3 _selfLocalUpwards = Vector3.zero;
        
        protected bool _nextCatchLeft = false;
        protected bool _nextCatchRight = false;
        
        [SyncVar]
        protected uint _catcherPlayerId = 0;

        [SyncVar(hook = nameof(Sync__isCatched))]
        protected bool _isCatched = false;
        
        protected bool _ownerInited = false;
    
        public event Action OnCatch;
        public event Action OnCatchRelease;
        
        public event Action OnCatchTryC;
        public event Action OnCatchSucceedS;
        /// <summary>
        /// All client
        /// </summary>
        public event Action OnCatchSucceedRpc;
        /// <summary>
        /// Owner client only
        /// </summary>
        public event Action OnCatchSucceedC;
        
        public event Action OnReleaseTryC;
        public event Action OnReleaseSucceedS;
        /// <summary>
        /// All client
        /// </summary>
        public event Action OnReleaseSucceedRpc;
        /// <summary>
        /// Former owner client only
        /// </summary>
        public event Action OnReleaseSucceedC;
        
        public bool IsCatchable => m_IsCatchable;
        public NetworkTransformBase RootNetTrans
        {
            get => _rootNetTrans;
            private set => _rootNetTrans = value;
        }
        
        public bool IsCatched => _isCatched;
        public uint CatcherPlayerId => _catcherPlayerId;
        public bool IsCatchedLocal => _catcher != null;
        public EHandType LastCatchHand => _lastCatchHand;


        protected virtual void Awake()
        {
            if (null == m_RootTrans) m_RootTrans = transform;
            RootNetTrans = m_RootTrans.GetComponent<NetworkTransformBase>();
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            
            SetIsCatchable(m_IsCatchable);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            SetIsCatchable(m_IsCatchable);
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (isServer && !_ownerInited)
            {
                NetworkConnectionToClient hostConn2C = Utils.GetHostConn2C();
                if (hostConn2C != null)
                {
                    netIdentity.AssignClientAuthority(hostConn2C);
                    _ownerInited = true;
                }
            }
            
            if (_isCatched && isOwned && _catcher != null)
            {
                DoFollow();
            }
        }

        protected virtual void OnDisable()
        {
            if (isServer)
            {
                S_Release();
            }
            
            Unavailable();
            DisableOutline();
        }        

        void InitFollow()
        {
            _followedTrans = _catcher.CatchPoint;
            _selfLocalPos = _followedTrans.InverseTransformPoint(m_RootTrans.position);
            _selfLocalForward = _followedTrans.InverseTransformDirection(m_RootTrans.forward);
            _selfLocalUpwards = _followedTrans.InverseTransformDirection(m_RootTrans.up);
        }

        void DoFollow()
        {
            if (null == _followedTrans) return;
            
            if (m_KeepRelativePos)
            {
                m_RootTrans.position = _followedTrans.TransformPoint(_selfLocalPos);
            }
            else
            {
                m_RootTrans.position = _followedTrans.position;
            }
            
            m_RootTrans.rotation = Quaternion.LookRotation(
                _followedTrans.TransformDirection(_selfLocalForward),
                _followedTrans.TransformDirection(_selfLocalUpwards));
        }

        void StopFollow()
        {
            _followedTrans = null;
            _selfLocalPos = Vector3.zero;
            _selfLocalForward = Vector3.zero;
            _selfLocalUpwards = Vector3.zero;
        }
        
        #region Catch
        
        public Catchable Catch(Catcher catcher)
        {
            // Pre process
            Catcher oldCatcher = _catcher;
            _catcher = catcher;
            InitFollow();
            
            // Try catch
            C_OnCatchTry();
            if (!_isCatched)
            {
                // 本地认为此物当前未被抓取，遂尝试抓取
                // 可能存在争夺者
                Cmd_Catch(catcher.HandType);
            }
            else if (isOwned && m_IsAllowSwitchHand)
            {
                if (oldCatcher != null && oldCatcher != catcher)
                {
                    // 此物正被自己抓取，用另一只手再次抓取，直接换手即可
                    oldCatcher.DeCatch(this);
                }
                catcher.CatchSuccessfully(this);
                Cmd_LastCatcher(catcher.HandType);
            }
            
            return this;
        }

        [Command]
        private void Cmd_LastCatcher(EHandType eHandType)
        {
            _lastCatchHand = eHandType;
        }

        [Command(requiresAuthority = false)]
        private void Cmd_Catch(EHandType eHandType, NetworkConnectionToClient sender = null)
        {
            if (sender == null || sender.identity == null) return;
            if (!_isCatched)
            {
                if (sender != netIdentity.connectionToClient)
                {
                    netIdentity.RemoveClientAuthority();
                    netIdentity.AssignClientAuthority(sender);
                }
                _catcherPlayerId = sender.identity.netId;
                _isCatched = true;
                _lastCatchHand = eHandType;
                S_OnCatchSucceed();
                Rpc_OnCatchSucceed();
            }
        }

        [Client]
        protected virtual void C_OnCatchTry()
        {
            OnCatchTryC?.Invoke();
        }
        
        [Server]
        protected virtual void S_OnCatchSucceed()
        {
            OnCatchSucceedS?.Invoke();
        }
        
        /// <summary>
        /// All client
        /// </summary>
        [ClientRpc]
        protected virtual void Rpc_OnCatchSucceed()
        {
            OnCatchSucceedRpc?.Invoke();
        }
        
        /// <summary>
        /// Owner client only
        /// </summary>
        [Client]
        protected virtual void C_OnCatchSucceed()
        {
            OnCatchSucceedC?.Invoke();
        }
        
        #endregion
        
        #region Releasse
        
        public void Release()
        {
            StartCoroutine(DoRelease());
        }
        
        private IEnumerator DoRelease()
        {
            StopFollow();
            _catcher = null;
            C_OnReleaseTry();
            // 在请求 Release 前等一帧，极大地避免无权威地移动 RootTrans 。
            // 不过无权威地移动 RootTrans 貌似也不会产生什么问题，
            // 服务端会把操作挡掉然后报个警告。
            yield return null;
            Cmd_Release();
        }
        
        [Command(requiresAuthority = false)]
        private void Cmd_Release(NetworkConnectionToClient sender = null)
        {
            if (sender == connectionToClient)
            {
                S_Release();
            }
        }
        
        [Server]
        public void S_Release()
        {
            if (_isCatched)
            {
                NetworkConnectionToClient hostConn2C = Utils.GetHostConn2C();
                if (netIdentity.connectionToClient != hostConn2C)
                {
                    netIdentity.RemoveClientAuthority();
                    netIdentity.AssignClientAuthority(hostConn2C);
                }
                _isCatched = false;
                S_OnReleaseSucceed();
                Rpc_OnReleaseSucceed();
            }
        }
        
        [Client]
        protected virtual void C_OnReleaseTry()
        {
            OnReleaseTryC?.Invoke();
        }
        
        [Server]
        protected virtual void S_OnReleaseSucceed()
        {
            OnReleaseSucceedS?.Invoke();
        }
        
        /// <summary>
        /// All client
        /// </summary>
        [ClientRpc]
        protected virtual void Rpc_OnReleaseSucceed()
        {
            OnReleaseSucceedRpc?.Invoke();
        }
        
        /// <summary>
        /// Former owner client only
        /// </summary>
        [Client]
        protected virtual void C_OnReleaseSucceed()
        {
            OnReleaseSucceedC?.Invoke();
        }

        #endregion

        private void Sync__isCatched(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                // 本来用 isOwned 来做判断，但在 host 上它更新得比 SyncVar 晚
                if (Utils.GetPlayerId() == _catcherPlayerId)
                {
                    // 自己抓
                    _catcher.CatchSuccessfully(this);
                    C_OnCatchSucceed();
                }
                else
                {
                    // 别人抓
                    DisableTrigger();
                    Unavailable();
                }
                
                DisableOutline();
            }
            else
            {
                if (Utils.GetPlayerId() == _catcherPlayerId)
                {
                    // 自己释放
                    if (_catcher != null) _catcher.DeCatch(this);
                    C_OnReleaseSucceed();
                }
                else
                {
                    // 别人释放
                }
                
                EnableTrigger();
                NextCatch(EHandType.None);
            }
        }
                
        private void OnTriggerEnter(Collider other)
        {
            Catcher catcher = other.GetComponentInParent<Catcher>();
            if (null == catcher) return;

            if (_catcherDic.ContainsKey(catcher))
            {
                _catcherDic[catcher] += 1;
            }
            else
            {
                _catcherDic[catcher] = 1;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Catcher catcher = other.GetComponentInParent<Catcher>();
            if (null == catcher) return;

            if (_catcherDic.ContainsKey(catcher))
            {
                if (_catcherDic[catcher] > 1)
                {
                    _catcherDic[catcher] -= 1;
                }
                else
                {
                    _catcherDic.Remove(catcher);
                }
            }
        }
        
        protected void Unavailable()
        {
            foreach (var c in _catcherDic.Keys)
            {
                c.CatchableUnavailable(this);
            }
            _catcherDic.Clear();
            _nextCatchLeft = false;
            _nextCatchRight = false;
        }
        
        public void EnableOutline()
        {
            foreach (Outline outline in m_OutlineList)
            {
                outline.enabled = true;
            }
        }
        
        public void DisableOutline()
        {
            foreach (Outline outline in m_OutlineList)
            {
                outline.enabled = false;
            }
        }
        
        protected void EnableTrigger()
        {
            foreach (Collider trigger in m_TriggerList)
            {
                trigger.enabled = true;
            }
        }
        
        protected void DisableTrigger()
        {
            foreach (Collider trigger in m_TriggerList)
            {
                trigger.enabled = false;
            }
        }
        
        public void NextCatch(EHandType handType)
        {
            if (handType == EHandType.Left) _nextCatchLeft = true;
            else if (handType == EHandType.Right) _nextCatchRight = true;
            
            if (!_isCatched && (_nextCatchLeft || _nextCatchRight)) EnableOutline();
        }
        
        public void NextCatchCancelled(EHandType handType)
        {
            if (handType == EHandType.Left) _nextCatchLeft = false;
            else if (handType == EHandType.Right) _nextCatchRight = false;
            
            if (_isCatched || (!_nextCatchLeft && !_nextCatchRight)) DisableOutline();
        }
        
        [Server]
        public void S_SetActive(bool active)
        {
            gameObject.SetActive(active);
            Rpc_SetActive(active);
        }
        
        [ClientRpc]
        public void Rpc_SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        
        [Server]
        public void S_SetIsCatchable(bool isCatchable)
        {
            if (isCatchable == m_IsCatchable) return;
            
            StartCoroutine(S_CoSetIsCatchable(isCatchable));
        }
        
        [Server]
        private IEnumerator S_CoSetIsCatchable(bool isCatchable)
        {
            if (!isCatchable && _isCatched)
            {
                S_Release();
                yield return new WaitForSeconds(0.2f);
            }
            
            m_IsCatchable = isCatchable;
            if (isServerOnly) SetIsCatchable(isCatchable);         
        }
        
        private void Sync_m_IsCatchable(bool oldValue, bool newValue)
        {
            SetIsCatchable(newValue);
        }
        
        private void SetIsCatchable(bool isCatchable)
        {
            if (isCatchable)
            {
                EnableTrigger();
            }
            else
            {
                DisableTrigger();
                Unavailable();
                DisableOutline();
            }
        }
        
#if UNITY_EDITOR
        public Transform e_RootTrans
        {
            get => m_RootTrans;
            set => m_RootTrans = value;
        }
        
        public List<Outline> e_OutlineList
        {
            get => m_OutlineList;
            set => m_OutlineList = value;
        }
        
        public List<Collider> e_TriggerList
        {
            get => m_TriggerList;
            set => m_TriggerList = value;
        }
#endif
    }
}
