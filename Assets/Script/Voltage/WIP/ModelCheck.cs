/*
功能：模型自动排布检查
*/
using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace WIP
{
    public enum ArrangementDirection
    {
        X,
        Y,
        Z,
        ReverseX,
        ReverseY,
        ReverseZ
    }

    public class ModelCheck : MonoBehaviour
    {
        public Transform[] models;
        public WIP.ArrangementDirection direction = WIP.ArrangementDirection.X;
        public float offset = 2f;
        public Dictionary<Transform, Vector3> modelList;

        [Button("自动排布模型")]
        void Arrange()
        {
            //如果数组为空或数组某个元素为空，则返回
            if(models == null || Array.Exists(models, element => element == null))
            {
                //重新获取数组
                models = GetComponentsInChildren<Transform>();
            }
            if (models == null || models.Length == 0)
            {
                Debug.Log("没有模型");
                return;
            }
            
            Vector3 dir = GetDirection(direction);
            Vector3 center = dir * offset;
            modelList = new Dictionary<Transform, Vector3>();
            foreach (Transform model in models)
            {
                modelList.Add(model, model.position);
                model.position += center;
                center += dir * offset;
            }
        }
        [Button("还原模型位置")]
        void Restore()
        {
            if (modelList == null || modelList.Count == 0)
            {
                Debug.Log("没有模型位置记录");
                return;
            }
            foreach (var item in modelList)
            {
                item.Key.position = item.Value;
            }
            modelList.Clear();
        }
        [Button("检查模型")]
        void Check()
        {

        }
        Vector3 GetDirection(ArrangementDirection _direction)
        {
            switch (_direction)
            {
                case ArrangementDirection.X:
                    return Vector3.right;
                case ArrangementDirection.Y:
                    return Vector3.up;
                case ArrangementDirection.Z:
                    return Vector3.forward;
                case ArrangementDirection.ReverseX:
                    return Vector3.left;
                case ArrangementDirection.ReverseY:
                    return Vector3.down;
                case ArrangementDirection.ReverseZ:
                    return Vector3.back;
                default:
                    Debug.LogError("没有该方向");
                    return Vector3.zero;
            }
        }
    }
}
