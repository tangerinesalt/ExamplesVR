using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Autohand;
using Autohand.Demo;
using UnityEngine.InputSystem;

namespace Tangerine
{
    [CustomEditor(typeof(OpenXRHandPlayerControllerLink_tangerine))]
    public class OpenXRHandPlayerControllerLink_tangerineEditor : Editor

    {
        OpenXRHandPlayerControllerLink_tangerine _playerControll;
        
        private SerializedProperty dualHandMoveSwitchProperty;
        private void OnEnable()
        {
            _playerControll = target as OpenXRHandPlayerControllerLink_tangerine;
            // 获取你想要检查的公共引用的SerializedProperty
            dualHandMoveSwitchProperty = serializedObject.FindProperty("EnableTurnOrDualHandMovement");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 绘制默认的Inspector内容
            DrawDefaultInspector();

            //if (!dualHandMoveSwitchProperty.boolValue)
            if (!_playerControll.EnableTurnOrDualHandMovement)
            {
                if (_playerControll.moveAxisR.action == null)
                {
                    // 显示警告
                    EditorGUILayout.HelpBox("Your moveAxisR Reference is not set!", MessageType.Warning);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}