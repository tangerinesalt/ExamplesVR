using System;
using System.Collections;
using System.Collections.Generic;
using Tangerine;
using UnityEngine;

public class VRMovement_Tangerinesalt : VRMovementBase
{
    [Header("Sync")]
    [SerializeField] private Transform m_SyncTransformForModel = null;//需要优化,使用单例获取
    private bool isOpenSync = false;

    protected override void Start()
    {
        base.Start();
       if (m_SyncTransformForModel == null)
        {FindSyncObject();}
    }
    protected override void Update()
    {
        base.Update();
        if (m_SyncTransformForModel == null)
        {FindSyncObject();}
        SyncPosition();//VR组件与模型的位置同步
    }
    private void FindSyncObject()
    {
        if (m_SyncTransformForModel == null)
        {
            m_SyncTransformForModel= GameObject.Find("Y Bot_model").transform;
            if(m_SyncTransformForModel!= null)
           { isOpenSync = false;
            Debug.Log("nont find syncObject");}
        }
         if(m_SyncTransformForModel != null) isOpenSync =true;
    }

    private void SyncPosition()
    {
        if (!isOpenSync)
        {
            Debug.Log("The object being synchronized is empty");
        }
        else
        {
            m_SyncTransformForModel.position = m_RootTrans.position;
        }
    }


}

