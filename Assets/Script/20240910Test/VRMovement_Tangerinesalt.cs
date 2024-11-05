using System;
using System.Collections;
using System.Collections.Generic;
using Tangerine;
using UnityEngine;

public class VRMovement_Tangerinesalt : VRMovementBase
{
    [Header("Sync")]
    [SerializeField] private bool m_isOpenSync = false;
    [SerializeField] private Transform m_SyncTransformForModel = null;//需要优化,使用单例获取


    protected override void Start()
    {
        base.Start();
        if (m_isOpenSync)
        {
            if (m_SyncTransformForModel == null)
            { FindSyncObject(); }
        }
    }
    protected override void Update()
    {
        base.Update();
        SyncPosition();//VR组件与模型的位置同步
    }
    private void FindSyncObject()
    {
        m_SyncTransformForModel = GameObject.Find("Y Bot_model").transform;
        if (m_SyncTransformForModel == null)
        {
            m_isOpenSync = false;
            Debug.Log("nont find syncObject");
        }

        if (m_SyncTransformForModel != null) m_isOpenSync = true;
    }

    private void SyncPosition()
    {
        if(m_isOpenSync)
        {
            if (m_SyncTransformForModel == null)
            { FindSyncObject(); }
            m_SyncTransformForModel.position = m_RootTrans.position;
        }
    }


}

