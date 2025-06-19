using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class SimulatorXRPlayerSetting : MonoBehaviour
{

    [Header("XR Device Simulator高度自适应设置")]
    public bool EnableHeightAdaptation = true;
    [BoxGroup("摄像机位置")] public Transform m_cameraOffset;
    [BoxGroup("摄像机位置")] public float m_cameraHeight = 1.1176f;
    [BoxGroup("Controller位置")] public Transform m_controllerOffset;
    [BoxGroup("Controller位置")] public float m_controllerHeight = 1.0f;

    private void Start()
    {
#if UNITY_EDITOR
        if (EnableHeightAdaptation)
            CheckAndSetXROffset();
#endif
    }
    private void CheckAndSetXROffset()
    {
        //检查是否启用XR
        if (XRGeneralSettings.Instance == null)
        {
            Debug.LogError("XRGeneralSettings is not found in the scene. Please add it to the scene.");
            return;
        }

        SetOffset();
    }

    /// <summary> 根据是否识别的真实XR设备-设置不同的跟踪驱动 </summary>
    private void SetOffset()
    {
        string loadedDevice = XRSettings.loadedDeviceName;
        if (loadedDevice.Length > 1)
        {
            Debug.Log($"Device is found. And device name: {loadedDevice}");
            m_cameraOffset.position = Vector3.zero;
            m_controllerOffset.position = Vector3.zero;
        }
        else
        {
            Debug.Log($"Device is not founD.");
            m_cameraOffset.position = new Vector3(0, m_cameraHeight, 0);
            m_controllerOffset.position = new Vector3(0, m_controllerHeight, 0);
        }
    }
}
