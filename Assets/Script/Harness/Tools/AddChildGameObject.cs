/****************************************************
    功能：批量为父物体们添加预制体子对象的工具，用后即删
    作者：ZZQ
    创建日期：#2025/02/20#
    修改人：ZZQ
    修改日期：#2025/02/20#
    修改内容：
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class AddChildGameObject : MonoBehaviour
{
    [Header("Add child")]public GameObject childPrefab = null;
    public Vector3 offset = Vector3.zero;
    public Vector3 rotation = Vector3.zero;
    public Vector3 scale = Vector3.one;

    [Header("ReferenceObject")]public List<GameObject> ReferenceObjects;

    [Header("Set Parent")]public GameObject parent;
    [ReadOnly]public List<Transform> childs;

    [ContextMenu("Add Child"),Button("Add Child")]
    public void AddChild()
    {
        foreach (GameObject go in ReferenceObjects)
        {
            GameObject childstarget;
            if (childPrefab!= null)
            {
                childstarget = Instantiate(childPrefab, go.transform);
                if (childPrefab.name.Contains("noInstall"))childstarget.name=go.name+"_noInstall";
                else childstarget.name=childPrefab.name;
            }
            else
            {
                childstarget = new GameObject();
                childstarget.transform.parent = go.transform;
                childstarget.name=go.name;
                childstarget.transform.localPosition =Vector3.zero;
                childstarget.transform.rotation = Quaternion.Euler(Vector3.zero);
            }
            childs.Add(childstarget.transform);

            if(parent!=null) childstarget.transform.SetParent(parent.transform);

            childstarget.transform.position+=offset;
            childstarget.transform.localRotation = Quaternion.Euler(rotation);
            childstarget.transform.localScale = scale;
        }
    }
    [ContextMenu("Remove Child"),Button("Remove Child")]
    public void RemoveChild()
    {
        foreach (Transform child in childs)
        {
            DestroyImmediate(child.gameObject);
        }
        childs.Clear();
    }
}
