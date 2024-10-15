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
        if (m_SyncTransformForModel != null)
        {
            isOpenSync = true;
        }
    }
    protected override void Update()
    {
        base.Update();
        SyncPosition();//VR组件与模型的位置同步
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

    public override void ChooseMoveMethod(MoveMethod _moveMethod, Vector3 MoveDir)
    {
        base.ChooseMoveMethod(_moveMethod, MoveDir);
    }
    public override void MoveByCommand(Vector3 MoveDir)
    {
        base.MoveByCommand(MoveDir);
    }

    public override void MoveByRigidbody(Vector3 MoveDir)
    {
        base.MoveByRigidbody(MoveDir);
    }
    public override void MoveByCharacterControllerd(Vector3 MoveDir)
    {
        base.MoveByCharacterControllerd(MoveDir);
    }

}

