using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voltage;

namespace Voltage
{
    public enum EHandType
    {
        Left,
        Right,
        Both,
        Any,
        None,
    }

    public enum EAxis
    {
        None,
        X,
        Y,
        Z,
        XNegative,
        YNegative,
        ZNegative,
    }

    public enum EUpdateType
    {
        Update,
        LateUpdate,
        UpdateAndLateUpdate,
    }
    
    public enum ESideType
    {
        None,
        Any,
        Left,
        Right,
        Up,
        Down,
        LeftUp,
        LeftDown,
        RightUp,
        RightDown
    }

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
    

    public enum ESimulationType
    {
        None,
        SinglePerson,
        MultiPerson,
        OperationGuide,
    }

    public enum Controller
    {
        LeftController,
        RightController
    }

    public enum GuideState
    {
        None,
        Trigger,
        Move,
        Rotate,
        LeftUI,
        RightUI,
        Catch,
        TelePort
    }

    public enum EHardType
    {
        Easy = 1,
        Normal,
        Hard,
        Expert,
        Master,
        God,
    }
}
