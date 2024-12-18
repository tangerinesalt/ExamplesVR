using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager_NoneVR : SingleBehaviour<MainManager_NoneVR>
{
    [SerializeField] private GameObject m_NoneVRPlayer;
    public GameObject GetNoneVRPlayer()=> m_NoneVRPlayer;
    public Transform PlayerTransform => m_NoneVRPlayer.transform;
}
