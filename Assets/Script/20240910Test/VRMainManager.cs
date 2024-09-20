using System.Collections;
using System.Collections.Generic;
using Tangerine;
using UnityEngine;

public class VRMainManager : SingleBehaviour<VRMainManager>
{
    [SerializeField] private Transform m_VRRoot;
    [SerializeField] private Transform m_VRHead;
    [SerializeField] private Transform m_VRLeftHand;
    [SerializeField] private Transform m_VRRightHand;
    [SerializeField] private Transform m_VRLeftController;
    [SerializeField] private Transform m_VRRightController;



    private void Start()
    {
        
    }
    #region Tool-GetVRcomponent
    public Transform GetVRRoot()
    {
        return m_VRRoot;
    }

    public Transform GetVRHead()
    {
        return m_VRHead;
    }

    public Transform GetVRLeftHand()
    {
        return m_VRLeftHand;
    }

    public Transform GetVRRightHand()
    {
        return m_VRRightHand;
    }

    public Transform GetVRLeftController()
    {
        return m_VRLeftController;
    }

    public Transform GetVRRightController()
    {
        return m_VRRightController;
    }

    #endregion
    #region Tool-GetVRStatus

   

    #endregion
}
