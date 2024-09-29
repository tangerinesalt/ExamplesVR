using System.Collections;
using System.Collections.Generic;
using Tangerine;
using UnityEngine;

public class VRMovement_Tangerinesalt : VRMovementBase
{
    private void Start()
    {
        //m_moveMethod = MoveMethod.MoveByCustom;

    }
    public override void ChooseMoveMethod(MoveMethod _moveMethod, Vector3 MoveDir)
    {
        base.ChooseMoveMethod(_moveMethod, MoveDir);
        if (_moveMethod == MoveMethod.MoveByCustom)
        {
            Debug.Log($"Executed: MoveByCustom() for {this.name}");
            //MoveByCustom(MoveDir);
        }
    }
    

    public override void MoveByCommand(Vector3 MoveDir)
    {
        base.MoveByCommand(MoveDir);
    }

    public override void MoveByRigidbody(Vector3 MoveDir)
    {
        base.MoveByRigidbody(MoveDir);
    }

    public override void MoveByCharacterControllerd()
    {
        base.MoveByCharacterControllerd();
    }

}

