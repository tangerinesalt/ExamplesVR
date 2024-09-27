using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollisionTriggerSensing : MonoBehaviour
{
    [SerializeField] private bool m_IsTriggerForCamera= false;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Camera Collision Trigger Entered Form " + other.gameObject.name);
        m_IsTriggerForCamera = true;
        
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Camera Collision Trigger Exited Form " + other.gameObject.name);
        m_IsTriggerForCamera = false;
    }

    public bool GetIsTriggerForCamera()
    {
        return m_IsTriggerForCamera;
    }
}
