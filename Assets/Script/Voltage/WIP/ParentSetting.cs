/*
功能：为每个对象创建父物体
*/
using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ParentSetting : MonoBehaviour
{
    public Transform GrandParent;
    public Transform[] Childs;
    [ReadOnly]
    public List<Transform> Parents;
    [Button("设置父物体")]
    void SetParent()
    {
        if (Childs == null || Childs.Length == 0)
        {
            Debug.Log("没有任何子物体");
            return;
        }
        Parents = new List<Transform>();

        foreach (Transform child in Childs)
        {
            String name = child.name;
            GameObject go = new GameObject(name);
            if (GrandParent == null) GrandParent = this.transform;
            go.transform.SetParent(GrandParent);
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            child.SetParent(go.transform);
            Parents.Add(go.transform);
        }
    }
    [Button("重置子物体位置")]
    void childsPosReset()
    {
        foreach (Transform child in Childs)
        {
            PosReset(child);
        }
    }
    void PosReset(Transform child)
    {
        child.localPosition = Vector3.zero;
        // child.localRotation = Quaternion.identity;
        // child.localScale = Vector3.one;
    }
}
