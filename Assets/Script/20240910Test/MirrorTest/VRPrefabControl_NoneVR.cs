using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using kcp2k;

public class VRPrefabControl_NoneVR : NetworkBehaviour
{
    [SerializeField] private Transform NoneVRPlayerTransform;
    [SerializeField] private Transform m_RootTransform;
    public override void OnStartServer()
    {
        base.OnStartServer();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        transform.SetPositionAndRotation(MainManager_NoneVR.Instance.PlayerTransform.position, MainManager_NoneVR.Instance.PlayerTransform.rotation);
        m_RootTransform.SetPositionAndRotation(MainManager_NoneVR.Instance.PlayerTransform.position, MainManager_NoneVR.Instance.PlayerTransform.rotation);
        
    }

}
