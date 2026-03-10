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

        // 在场景原点创建一个Cube
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "Cube";
        cube.transform.position = Vector3.zero;
        Debug.Log("已在场景原点创建Cube".FontColoring(Color.green));
    }
}