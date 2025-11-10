/*
功能：控制子对象行为
*/
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ChildControl : MonoBehaviour
{
    [Header("父对象相关设置")]
    [Tooltip("是否允许同名子对象存在")]
    public bool AllowSameNameObjects = false;
    [Tooltip("父对象")]
    public Transform[] parents;

    [Space(10)]
    [Header("预制子对象相关设置"), Tooltip("子对象名称")]
    public string childName = "Rope";
    [Tooltip("子对象预制体")]
    public GameObject childPrefab;

    [Space(10)]
    [Header("空白子对象相关设置"), Tooltip("是否名称相同")]
    public bool IsSameNameWithParent = true;
    [HideIf("IsSameNameWithParent")]
    public string emptyChildName = "Empty";

    [Space(10)][ReadOnly]
    public List<GameObject> childTransform;

    /// <summary> 为所有父对象添加子对象（通过预制体） </summary>
    [Button("添加预制子对象")]
    void AddPrefabChildForParents()
    {
        AddChilds(true);
    }
    /// <summary> 为所有父对象添加空白子对象 </summary>
    [Button("添加空白子对象")]
    void AddEmptyChildForParents()
    {
        AddChilds(false);
    }
    /// <summary> 为所有父对象添加子对象（通过预制体或空白） </summary>
    void AddChilds(bool isPrefab = false)
    {
        if (parents == null || parents.Length == 0)
        {
            Debug.LogError("没有找到父对象");
            return;
        }
        foreach (Transform parent in parents)
        {
            AddChild(parent, isPrefab);
        }
    }

    /// <summary>
    /// 为单个父对象添加子对象（通过预制体或空白）
    /// </summary>
    /// <param name="parent">父对象</param>
    /// <param name="isPrefab">是否使用预制体</param>
    void AddChild(Transform parent, bool isPrefab = false)
    {
        if (parent == null)
        {
            Debug.LogError("没有找到父对象");
            return;
        }
        if (isPrefab)
        {
            AddPrefabChild(parent);
            return;
        }
        else
        {
            AddEmptyChild(parent);
            return;
        }
    }
    /// <summary> 为父对象添加预制子对象 </summary>
    void AddPrefabChild(Transform parent)
    {
        if (parent.Find(childName) != null && !AllowSameNameObjects)
        {
            Debug.Log($"父对象已有子对象{childName}，跳过创建".FontColoring("red"), parent);
            return;
        }
        if (childPrefab != null)
        {
            GameObject child;
            // child= Instantiate(childPrefab);
            // 与预制体关联
#if UNITY_EDITOR
            child = (GameObject)PrefabUtility.InstantiatePrefab(childPrefab);
#endif
            if (child == null)
            {
                Debug.LogError("子对象预制体实例化失败");
                return;
            }
            child.name = childName;
            child.transform.SetParent(parent);
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;

            childTransform.Add(child);
        }
        else
        {
            Debug.LogError("没有指定子对象预制体");
        }
    }
    /// <summary> 为父对象添加空白子对象 </summary>
    void AddEmptyChild(Transform parent)
    {
        string finalChildName = IsSameNameWithParent ? parent.name : emptyChildName;
        if (parent.Find(finalChildName) != null && !AllowSameNameObjects)
        {
            Debug.Log($"父对象已有子对象{finalChildName}，跳过创建".FontColoring("red"), parent);
            return;
        }
        GameObject child = new GameObject(finalChildName);
        child.transform.SetParent(parent);
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;
        child.transform.localScale = Vector3.one;

        childTransform.Add(child);
    }
    [Button("删除子对象")]
    void RemoveChild()
    {
        if (childTransform == null || childTransform.Count == 0)
        {
            Debug.LogWarning("没有子对象可以删除");
            return;
        }
        for (int i = childTransform.Count - 1; i >= 0; i--)
        {
            DestroyImmediate(childTransform[i]);
            childTransform.RemoveAt(i);
        }
    }
}
