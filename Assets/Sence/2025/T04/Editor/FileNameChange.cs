using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 将该路径下所有文件名修改为自动命名
/// </summary>
public class FileNameChange : EditorWindow
{
    // 默认路径
    private string prePath = "Assets/Art/Models/Test/";

    [MenuItem("Tools/Change File Name")]
    public static void ShowWindow()
    {
        GetWindow(typeof(FileNameChange));
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("默认路径:", prePath);

        if (GUILayout.Button("选择路径"))
        {
            ChangePrePath();
        }

        if (GUILayout.Button("开始修改"))
        {
            StartChange();
        }

        EditorGUILayout.EndVertical();
    }

    // 选择路径
    private void ChangePrePath()
    {
        string newPath = EditorUtility.OpenFolderPanel("选择保存路径", "Assets", "");
        if (string.IsNullOrEmpty(newPath))
        {
            return;
        }

        newPath = newPath.Replace("\\", "/");

        if (newPath.StartsWith(Application.dataPath, StringComparison.OrdinalIgnoreCase))
        {
            prePath = "Assets" + newPath.Substring(Application.dataPath.Length);
            if (!prePath.EndsWith("/", StringComparison.Ordinal))
            {
                prePath += "/";
            }
        }
        else
        {
            Debug.LogError("请选择项目的 Assets 文件夹内的路径");
        }
    }

    /// <summary>
    /// 修改默认路径下所有文件名
    /// </summary>
    private void StartChange()
    {
        if (string.IsNullOrEmpty(prePath))
        {
            Debug.LogError("默认路径为空");
            return;
        }

        string targetFolder = prePath.TrimEnd('/');
        if (!AssetDatabase.IsValidFolder(targetFolder))
        {
            Debug.LogError($"路径不存在: {prePath}");
            return;
        }

        string[] guids = AssetDatabase.FindAssets(string.Empty, new[] { targetFolder });
        if (guids.Length == 0)
        {
            Debug.LogWarning($"路径 {prePath} 下没有可修改的资源");
            return;
        }

        AssetDatabase.StartAssetEditing();
        try
        {
            int renameCount = 0;
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.IsValidFolder(assetPath))
                {
                    continue;
                }

                string currentName = Path.GetFileNameWithoutExtension(assetPath);
                string newName = AutoNaming(currentName);
                if (string.IsNullOrEmpty(newName) || string.Equals(currentName, newName, StringComparison.Ordinal))
                {
                    continue;
                }

                string error = AssetDatabase.RenameAsset(assetPath, newName);
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"重命名失败 {assetPath}: {error}");
                    continue;
                }

                renameCount++;
            }

            Debug.Log($"共重命名 {renameCount} 个文件");
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    // 修改文件名
    public string AutoNaming(string oldName)
    {
        if (string.IsNullOrEmpty(oldName))
        {
            Debug.LogWarning("文件名不能为空！");
            return null;
        }

        string fileName = oldName;

        // 修改中文字符
        fileName = fileName
            .Replace("单面", "OneSided")
            .Replace("双面", "TwoSided")
            .Replace("定量", "FixedLength")
            .Replace("标准板", "Standard");

        // 修改英文和其他字符
        fileName = fileName
            .Replace("°", string.Empty)
            .Replace("&", "And")
            .Replace(" ", "_")
            .Replace("-", "_");

        // 去除其他所有中文字符
        fileName = System.Text.RegularExpressions.Regex.Replace(fileName, "[\\u4e00-\\u9fa5]", string.Empty);

        Debug.Log($"将文件名 {oldName} 修改为 {fileName}");
        return fileName;
    }
}
