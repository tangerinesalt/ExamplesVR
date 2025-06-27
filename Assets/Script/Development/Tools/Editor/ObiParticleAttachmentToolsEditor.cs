using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEditor;
using UnityEngine;
using Voltage;

[CustomEditor(typeof(ObiParticleAttachmentTools))]
public class ObiParticleAttachmentToolsEditor : Editor
{
    private ObiParticleAttachmentTools TargetObj;
    void OnEnable()
    {
        TargetObj = (ObiParticleAttachmentTools)this.target;
    }
    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
        DrawInspector();
    }

    private void DrawInspector()
    {
        ObiActor obiActor = TargetObj.GetComponent<ObiActor>();
        EditorGUILayout.LabelField("全局参数", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical();
        TargetObj.m_TargetObjectPrefab = (GameObject)EditorGUILayout.ObjectField("Target Object Prefab", TargetObj.m_TargetObjectPrefab, typeof(GameObject), true);
        TargetObj.m_AttachmentType = (ObiParticleAttachment.AttachmentType)EditorGUILayout.EnumPopup("Attachment Type", TargetObj.m_AttachmentType);

        GUILayout.Space(10);
        if (GUILayout.Button("设置粒子附着"))
        {
            TargetObj.SetparticleAttachment(obiActor);
        }
        GUILayout.Space(6);
        if (GUILayout.Button("销毁附着组件"))
        {
            TargetObj.DestoryAttachmentComponent(obiActor);
        }

        EditorGUILayout.EndVertical();
    }
}
