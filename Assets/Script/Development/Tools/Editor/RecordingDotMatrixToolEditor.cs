using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RecordingDotMatrixTool))]
public class RecordingDotMatrixToolEditor : Editor
{
    private RecordingDotMatrixTool tool;

    // 可选属性折叠状态
    private bool OptionalProperties = false;
    
    

    void OnEnable()
    {
        tool = (RecordingDotMatrixTool)this.target;
    }

    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI(); 
        DrawInspector();
    }

    void DrawInspector()
    {
        EditorGUILayout.LabelField("Recording Dot Matrix Tool", new GUIStyle(EditorStyles.boldLabel) { fontSize = 16, alignment = TextAnchor.MiddleCenter, });
        EditorGUILayout.Space(10);

        // 绘制可选属性折叠框
        OptionalProperties = EditorGUILayout.Foldout(OptionalProperties, "Optional Properties", true);
        if (OptionalProperties)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.HelpBox("These properties are optional and can be adjusted based on your requirements.", MessageType.Info);
            tool.NeedVerifyLayerMask = EditorGUILayout.Toggle("Verify Layer Mask", tool.NeedVerifyLayerMask);
            if (tool.NeedVerifyLayerMask)
            {
                // 绘制触发层文本框
                tool.m_TriggerLayer = (LayerMask)EditorGUILayout.LayerField("Trigger Layer", tool.m_TriggerLayer);
            }
            EditorGUILayout.EndVertical();
        }
        // 绘制统一记录属性
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Ray Unify Property", EditorStyles.boldLabel);
        tool.selectAllDoF = EditorGUILayout.Toggle("All Directions", tool.selectAllDoF);

        // 绘制特殊记录属性
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Ray Specific Property", EditorStyles.boldLabel);
        // 绘制记录点属性
        for (int i = 0; i < tool.displayOrder.Length; i++)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            tool.calculateGroup[tool.displayOrder[i]] = EditorGUILayout.Toggle(tool.calculateGroupNames[tool.displayOrder[i]], tool.calculateGroup[tool.displayOrder[i]]);
            if (tool.calculateGroup[tool.displayOrder[i]])
            {
                // 绘制计算组的方向下拉框和起始距离文本框
                tool.selectedIndex[tool.displayOrder[i]] = EditorGUILayout.Popup("Direction1", tool.selectedIndex[tool.displayOrder[i]], tool.directionOptions);
                tool.calculateDirection[tool.displayOrder[i]] = tool.directionOptions[tool.selectedIndex[tool.displayOrder[i]]];
                tool.StartDistanceGroups[tool.displayOrder[i]] = EditorGUILayout.IntField("Start Distance1", tool.StartDistanceGroups[tool.displayOrder[i]]);

                // 绘制记录范围文本框和记录数量文本框
                tool.RecordRangeGroups[tool.displayOrder[i]] = EditorGUILayout.Vector2Field("Record Range", tool.RecordRangeGroups[tool.displayOrder[i]]);
                tool.RecordCountGroups[tool.displayOrder[i]] = EditorGUILayout.Vector2Field("Record Count", tool.RecordCountGroups[tool.displayOrder[i]]);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space(6);
        if (GUILayout.Button("Start Recording"))
        {
            for (int i = 0; i < tool.displayOrder.Length; i++)
            {
                if (tool.calculateGroup[tool.displayOrder[i]])  
                {
                    Debug.Log($"开始计算{tool.calculateGroupNames[tool.displayOrder[i]]} (--计算距离:{tool.StartDistanceGroups[tool.displayOrder[i]]}--记录范围:{tool.RecordRangeGroups[tool.displayOrder[i]]}--记录数量:{tool.RecordCountGroups[tool.displayOrder[i]]}--)");
                    tool.StartRecord(tool.StartDistanceGroups[tool.displayOrder[i]], tool.calculateDirection[tool.displayOrder[i]], tool.RecordRangeGroups[tool.displayOrder[i]], tool.RecordCountGroups[tool.displayOrder[i]], tool.NeedVerifyLayerMask);
                }
            }
        }


    }
}