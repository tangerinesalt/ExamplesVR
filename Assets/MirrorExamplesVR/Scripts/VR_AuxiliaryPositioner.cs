using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_AuxiliaryPositioner : MonoBehaviour
{
    private Vector3 _TeleportationLocation=Vector3.zero;
    private bool OpenLocationRecord=true;
    
    void Start()
    {
        _TeleportationLocation=this.transform.position;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (OpenLocationRecord)
        {
            _TeleportationLocation=this.transform.position;
            _TeleportationLocation.y=0;
            Debug.Log("Teleportation Location: "+_TeleportationLocation.x+","+_TeleportationLocation.y+","+_TeleportationLocation.z);
            //Debug.Log($"{other.gameObject.name} entered VR_AuxiliaryPositioner");
            OpenLocationRecord=false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!OpenLocationRecord)
        {
            OpenLocationRecord=true;
        }
        
    }
    public Vector3 GetTeleportationLocation()
    {
        return _TeleportationLocation;
    }
}
