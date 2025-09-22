using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voltage;

public class Test202502 : MonoBehaviour
{
    [SerializeField] private bool IsEnableTest = true;
    [SerializeField] private float m_reachDistance = 0.8f;
    [SerializeField]private bool m_isShowHandProjector = true;

    void Start()
    {
        if (!IsEnableTest)
        {
            Debug.Log("Test202502 is disabled.".FontColoring(Color.red));
            return;
        }

        PlayerManager.Instance.SetReachDistance(m_reachDistance);
        PlayerManager.Instance.SetHandProjectorState(m_isShowHandProjector);
    }
}