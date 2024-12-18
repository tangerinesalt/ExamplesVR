using System.Collections;
using System.Collections.Generic;
using Autohand;
using UnityEngine;
using Mirror;
using Voltage;

namespace Voltage
{
    /// <summary>
    /// 对于Grabbable_Voltage组件，在客户端禁用所有客户端的碰撞体collider，禁用子物体collider，禁用刚体rigidbody，并将其设置为 kinematic
    /// </summary>
    [RequireComponent(typeof(Grabbable_Voltage))]
    public class GrabbableRigAndColliderControl : NetworkBehaviour
    {
        [SerializeField] private bool m_enableKinematic=true;    
        [SerializeField] private bool m_disableSelfColliderOndisable = true;
        [SerializeField] private bool m_disableAllChildCollidersOndisable = false;
        [SerializeField] private GameObject[] m_disableObjectsCollidersOndisable;
        private Grabbable_Voltage m_grabbable;
        private void Start()
        {
            m_grabbable = GetComponent<Grabbable_Voltage>();
            if (m_grabbable != null)
            {
                m_grabbable.onDisable += Cmd_DisableGrabbableObject;
                m_grabbable.onEnable += Cmd_EnableGrabbableObject;
            }
        }

        [Command(requiresAuthority = false)]
        public void Cmd_DisableGrabbableObject()
        {
            S_DisableGrabbableObject();
        }
        [Command(requiresAuthority = false)]
        public void Cmd_EnableGrabbableObject()
        {
            S_EnableGrabbableObject();
        }

        [Server]
        public void S_DisableGrabbableObject()
        {
            if (isServer)
            {
                RPC_DisableGrabbableObject();
            }
        }
        [Server]
        public void S_EnableGrabbableObject()
        {
            if (isServer)
            {
                RPC_EnableGrabbableObject();
            }
        }
        //禁用抓取对象Grabbable组件，禁用碰撞体collider，禁用子物体collider，禁用目标及其子物体的collider，禁用刚体rigidbody，并将其设置为 kinematic
        [ClientRpc]
        private void RPC_DisableGrabbableObject()
        {
            Rigidbody _rigidbody = gameObject.GetComponent<Rigidbody>();

            if (m_grabbable != null)
            {
                m_grabbable.makeChildrenGrabbable = false;
            }
            if (_rigidbody != null&& m_enableKinematic)
            {
                _rigidbody.isKinematic = true;
            }

            if (m_disableSelfColliderOndisable)
            {
                Collider _collider = gameObject.GetComponent<Collider>();
                if (_collider != null) _collider.enabled = false;
            }
            if (m_disableAllChildCollidersOndisable)
            {
                Collider[] _childColliders = gameObject.GetComponentsInChildren<Collider>(true);
                if (_childColliders != null)
                {
                    for (int i = 0; i < _childColliders.Length; i++)
                    {
                        _childColliders[i].enabled = false;
                    }
                }
            }
            if (m_disableObjectsCollidersOndisable!= null)
            {
                foreach (GameObject _obj in m_disableObjectsCollidersOndisable)
                {
                    Collider collider = _obj.GetComponent<Collider>();
                    if (collider != null) collider.enabled = false;

                    Collider[] _childColliders = _obj.GetComponentsInChildren<Collider>(true);
                    if (_childColliders != null)
                    {
                        foreach (Collider _collider in _childColliders)
                        { 
                            _collider.enabled = false; 
                        }
                    }
                }
            }

        }
        [ClientRpc]
        private void RPC_EnableGrabbableObject()
        {
            Rigidbody _rigidbody = gameObject.GetComponent<Rigidbody>();

            if (m_grabbable != null)
            {
                m_grabbable.makeChildrenGrabbable = true;
            }
            if (_rigidbody != null && m_enableKinematic)
            {
                _rigidbody.isKinematic = false;
            }

            if (m_disableSelfColliderOndisable)
            {
                Collider _collider = gameObject.GetComponent<Collider>();
                if (_collider != null) _collider.enabled = true;
            }
            if (m_disableAllChildCollidersOndisable)
            {
                Collider[] _childColliders = gameObject.GetComponentsInChildren<Collider>(true);
                if (_childColliders != null)
                {
                    for (int i = 0; i < _childColliders.Length; i++)
                    {
                        _childColliders[i].enabled = true;
                    }
                }
            }
        }

    }
}