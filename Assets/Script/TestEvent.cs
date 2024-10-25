using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEvent : MonoBehaviour
{
    
    private void Start()
    {
        EventManager.Instance.onChangeSence += OnChangeSence;
        EventManager.Instance.OnChangeSence();
        //Debug.Log("TestEvent Start");
    }
    private void OnChangeSence()
    {
        Debug.Log("OnChangeSence for TestEvent");
    }
    private void OnDestroy()
    {
        EventManager.Instance.onChangeSence -= OnChangeSence;
    }
}
