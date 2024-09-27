using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.XR.Interaction.Toolkit;

public class CharacterControllerDriver_Voltage : CharacterControllerDriver
{
    // [SerializeField]
    // private Collider m_CameraCollisionTrigger;
    void Update()
    {
        UpdateCharacterController();
    }
    
}
