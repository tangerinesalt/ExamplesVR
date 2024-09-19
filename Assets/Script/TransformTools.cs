

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTools
{
    // Start is called before the first frame update
    public static  Transform GetTransformForName(Transform parent, string name)
    {

        if (parent.childCount > 0)
        {

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                // 检查当前子对象  
                if (child.name == name)
                {
                    return child; // 找到匹配项，直接返回  
                }
                Transform result = GetTransformForName(child, name);
                if (result != null)
                {
                    return result; // 找到匹配项，直接返回      
                }
            }
        }
        Debug.Log("Can't find the transform with name " + name);
        return null; // 没有找到匹配项，返回null  
    }
    // public static Component GetTransformForComponent(Transform parent, Component component)
    // {

    //     if (parent.childCount > 0)
    //     {

    //         for (int i = 0; i < parent.childCount; i++)
    //         {
    //             Transform child = parent.GetChild(i);
    //             try
    //             {
    //                 if (child.GetComponent(component.GetType()) == component)
    //                     return child.GetComponent(component.GetType());
    //             }
    //             catch (System.Exception e)
    //             {
    //                 Debug.Log(e.Message);
    //             }
    //             Component result = GetTransformForComponent(child, component);
    //             if (result != null)
    //             {
    //                 return result; // 找到匹配项，直接返回      
    //             }
    //         }

    //     }

    //     Debug.Log("Can't find the transform with component " + component.GetType().ToString() + "");
    //     return null; // 没有找到匹配项，返回null  
    // }
}

