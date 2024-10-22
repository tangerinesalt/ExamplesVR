using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tangerine
{
    public enum MoveDirect
    {
        None = 0,
        Forward = 1,
        ForwardLeft = 2,
        Left = 3,
        BackLeft = 4,
        Back = 5,
        BackRight = 6,
        Right = 7,
        ForwardRight = 8,
    }
    public enum MoveMethod
    {
        NoneMove = 0,
        MoveByCommand = 1,
        MoveByRigidbody = 2,
        MoveByCharacterController = 3,

        MoveByCustom=4
    }
    
}
