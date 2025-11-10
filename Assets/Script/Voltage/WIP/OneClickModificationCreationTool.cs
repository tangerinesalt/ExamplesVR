/*
功能：一键修改子对象（布线模型）的名称--根据文件名自动命名
*/
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Voltage;

public class OneClickModificationCreationTool : MonoBehaviour
{
    public List<Transform> childs;
    [Button("Get Childs")]
    void GetChilds()
    {
        childs = new List<Transform>();
        foreach (Transform child in transform)
        {
            childs.Add(child);
        }
    }
    [Button("Rename Childs")]
    void ChildsNameChange()
    {
        int num = 0;
        foreach (Transform child in childs)
        {
            string oldName = child.name;
            string newName = AutoNaming(oldName);
            if (newName != null)
            {
                if (child.name != newName)
                {
                    child.name = newName;
                    num++;
                }
            }
        }
        Debug.Log($"Renamed {num} child objects.");
    }
    /// <summary>
    /// 自动命名函数
    /// </summary>
    /// <param name="oldName"></param>
    /// <returns></returns>
    public string AutoNaming(string oldName)
    {
        if (string.IsNullOrEmpty(oldName))
        {
            Debug.LogWarning("File name cannot be empty.");
            return null;
        }

        string fileName = oldName;

        // Replace specific Chinese tokens with English equivalents.
        fileName = fileName
            .Replace("单面", "OneSided")
            .Replace("双面", "TwoSided")
            .Replace("定量", "FixedLength")
            .Replace("标准板", "Standard");

        // Clean up punctuation and whitespace.
        fileName = fileName
            .Replace("°", "")
            .Replace("&", "And")
            .Replace(" ", "_")
            .Replace("-", "_");
        
        // 将_FixedLength或者FixedLength替换到最后
        if (fileName.Contains("FixedLength"))
        {
            fileName = fileName
                .Replace("_FixedLength", "")
                .Replace("FixedLength", "");
            fileName += "_FixedLength";
        }

        // Drop any remaining Chinese characters.
        fileName = System.Text.RegularExpressions.Regex.Replace(fileName, "[\u4e00-\u9fa5]", "");

        Debug.Log($"Renaming {oldName} to {fileName}");
        return fileName;
    }
}
