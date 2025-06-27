using System;
using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEditor;
using UnityEngine;
using Voltage;

[CustomEditor(typeof(ObiRopePointTools))]
public class ObiRopePointToolsEditor : Editor
{
    private ObiRopePointTools TargetObj;
    private bool showBlueprintProperties = true; // 蓝图属性折叠状态
    private bool showRopeProperties = true; // 节点属性折叠状态


    private void OnEnable()
    {
        this.TargetObj = (ObiRopePointTools)this.target;
    }

    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();

        DrawInspector();
    }

    private void DrawInspector()
    {
        ObiRopeBase rope = this.TargetObj.GetComponent<ObiRopeBase>();

        EditorGUILayout.BeginVertical();
        // 居中显示标签
        GUIStyle centeredLabel = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16,
        };
        EditorGUILayout.LabelField("Rope Modify Tools", centeredLabel);
        showBlueprintProperties = EditorGUILayout.Foldout(showBlueprintProperties, "Blueprint Properties", true);
        if (showBlueprintProperties)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            TargetObj.m_thickness = EditorGUILayout.FloatField("Thickness", TargetObj.m_thickness);
            TargetObj.m_resolution = EditorGUILayout.Slider("Resolution", TargetObj.m_resolution, 0, 1);
            TargetObj.m_pooledParticles = EditorGUILayout.IntField("Pooled Particles", TargetObj.m_pooledParticles);

            EditorGUILayout.EndVertical();
        }

        showRopeProperties = EditorGUILayout.Foldout(showRopeProperties, "Rope Properties", true);
        if (showRopeProperties)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            TargetObj.m_RopeLength = EditorGUILayout.FloatField("Rope Length", TargetObj.m_RopeLength);
            TargetObj.m_InsertPointCount = EditorGUILayout.IntField("Insert Point Count", TargetObj.m_InsertPointCount);
            TargetObj.m_Mass = EditorGUILayout.Slider("Point Mass", TargetObj.m_Mass, 0, 2);
            TargetObj.m_Category = EditorGUILayout.Popup("Point Category", TargetObj.m_Category, ObiUtils.categoryNames, GUILayout.MinWidth(94));
            TargetObj.m_Mask = EditorGUILayout.MaskField("Point Collides with", TargetObj.m_Mask, ObiUtils.categoryNames, GUILayout.MinWidth(94));

            EditorGUILayout.EndVertical();
        }
        // GUILayout.TextField("Path Count", rope.path.ControlPointCount.ToString());
        
        GUILayout.Space(10);
        if (GUILayout.Button("一键修改蓝图"))
        {
            if (rope == null)
            {
                Utils.DebugLog(Color.yellow, "没有获取到 ObiRopeBase 组件");
                return;
            }
            TargetObj.ModifyBlueprint(rope);
            // 通过序列化属性所做的修改应用到目标对象上，并触发Unity内部的更新机制
            serializedObject.ApplyModifiedProperties();
            // 标记指定的目标对象为已修改，确保其状态会被保存到场景文件或资源文件中
            EditorUtility.SetDirty(this.TargetObj);
        }
        if (GUILayout.Button("一键修改节点"))
        {
            if (rope == null)
            {
                Utils.DebugLog(Color.yellow, "没有获取到 ObiRopeBase 组件");
                return;
            }
            TargetObj.ModifyRope(rope);
            // 通过序列化属性所做的修改应用到目标对象上，并触发Unity内部的更新机制
            serializedObject.ApplyModifiedProperties();
            // 标记指定的目标对象为已修改，确保其状态会被保存到场景文件或资源文件中
            EditorUtility.SetDirty(this.TargetObj);
        }

        GUILayout.Space(10);
        
        if (GUILayout.Button("清除所有控制点"))
        {
            TargetObj.ClearControlPoint(rope);
        }
        if (GUILayout.Button("清除所有中间点"))
        {
            TargetObj.RemoveMiddleControlPoint(rope);
        }

        EditorGUILayout.EndVertical();
    }
}
