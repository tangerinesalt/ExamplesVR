/****************************************************
    功能：obi Rope快速修改工具
    作者：WH、ZZQ
    创建日期：#2025/06/24#
    修改内容：
        1.
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using Voltage;
using NaughtyAttributes;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine.Events; // 确保引用Obi命名空间

namespace Voltage
{
    public enum ObiPointRemoveMode
    {
        All,
        Middle
    }
    /// <summary>
    /// 按钮在ObiRopePointToolsEditor类中实现
    /// </summary>
    public class ObiRopePointTools : MonoBehaviour
    {
        //公开变量（Inspector面板-通过editor绘制）
        // 蓝图相关属性
        public float m_thickness = 0.00622f;
        public float m_resolution = 0.2f;
        public int m_pooledParticles = 0;
        // 绳索相关属性
        public float m_RopeLength = 2;
        public int m_InsertPointCount = 2;
        public float m_Mass = 0.1f;
        public int m_Category = 1;
        public int m_Mask = 65521;//代表1, 2, 3层以外的所有obi.categoryNames层
        public UnityEvent onControlPointChanged;

        //内部变量
        /// <summary> 两节点间距占全长的比例（0-1之间） </summary>
        private float IntervalRatio
        {
            get
            {
                float intervalRatio = 1.0f / (m_InsertPointCount + 1);
                intervalRatio = (float)System.Math.Round(intervalRatio, 6);//保留小数点后2位
                return intervalRatio;
            }
        }

        /// <summary> 两节点间距 </summary>
        private float IntervalSize => (float)System.Math.Round(m_RopeLength / (m_InsertPointCount + 1), 6);

        /// <summary> 添加点的大概比例列表（0-1之间, 根据添加的节点数量计算） </summary>
        private List<float> MuList
        {
            get
            {
                List<float> muList = new List<float>(m_InsertPointCount);
                for (int i = 0; i < m_InsertPointCount; i++)
                {
                    muList.Add(IntervalRatio * (i + 1));
                }
                return muList;
            }
        }
        /// <summary> 节点内切线 </summary>
        private Vector3 InTangent => new Vector3(-Mathf.Min(IntervalSize * 0.5f, 0.25f), 0, 0);
        /// <summary> 节点外切线 </summary>
        private Vector3 OutTangent => new Vector3(Mathf.Min(IntervalSize * 0.5f, 0.25f), 0, 0);

        #region blueprint修改
        public void ModifyBlueprint(ObiRopeBase rope)
        {
            RemoveControlPoint(rope, ObiPointRemoveMode.All);
            ObiRopeBlueprint blueprint;
            if (GetBlueprint(rope, out blueprint) == false)
            {
                return;
            }
            blueprint.thickness = m_thickness;
            blueprint.resolution = m_resolution;
            blueprint.pooledParticles = m_pooledParticles;
        }
        private bool GetBlueprint(ObiRopeBase rope, out ObiRopeBlueprint blueprint)
        {
            if (rope is ObiRope _rope)
            {
                blueprint = _rope.ropeBlueprint;
                if (blueprint == null)
                {
                    Debug.LogError("ObiRope组件缺少有效蓝图");
                    return false;
                }
                return true;
            }
            else
            {
                Debug.LogError("未找到ObiRope组件");
                blueprint = null;
                return false;
            }
        }
        #endregion

        #region rope修改
        public void ModifyRope(ObiRopeBase rope)
        {
            if (RopeVerify(rope) == false)
            {
                Debug.LogError("未找到正确的Rope相关资源, 退出方法体...");
                return;
            }
            // 1.移除中间的控制点
            if (rope.path.ControlPointCount > 2)
                RemoveControlPoint(rope);
            // 2.设置绳索长度
            SetRopeLength(rope);
            // 3.生成新的节点
            if (m_InsertPointCount < 0)
            {
                Debug.LogError("插入点数不能为负数");
                return;
            }
            List<float> muList = MuList;

            // 3.1遍历muList, 逐个插入点
            for (int i = 0; i < m_InsertPointCount; i++)
            {
                int newIndex = rope.path.InsertControlPoint(muList[i]);
                Utils.DebugLog(Color.green, $"插入新顶点, 索引：{newIndex}");
                if (newIndex < 0)
                {
                    Debug.LogError("插入顶点失败");
                    return;
                }
            }
            // 3.2刷新路径事件（必须调用以更新绳索状态）
            rope.path.FlushEvents();
            Utils.DebugLog(Color.green, $"当前顶点数：{rope.path.ControlPointCount}");
            // 3.3遍历所有插入的节点, 设置属性
            for (int i = 1; i < rope.path.ControlPointCount - 1; ++i)
            {
                rope.path.SetName(i, i.ToString());
                //rope ControlPoint Property
                SetControlPointProperty(rope, i);
            }
            // 3.4重新设置节点位置和切线,全部靠近一侧后重新设置点位,以避免切线被挤压
            float MiniatureIntervalc = IntervalSize / m_InsertPointCount;
            int d = 0;
            for (int i = rope.path.ControlPointCount - 2; i > 0; --i)
            {
                d++;
                rope.path.points.data[i] = new ObiWingedPoint(Vector3.zero, new Vector3((m_RopeLength * 0.5f) - (d * MiniatureIntervalc), 0, 0), Vector3.zero);
            }
            for (int i = 1; i < rope.path.ControlPointCount - 1; ++i)
            {
                rope.path.points.data[i] = new ObiWingedPoint(InTangent, new Vector3((i * IntervalSize) - (m_RopeLength * 0.5f), 0, 0), OutTangent);
            }
            // 4.重置始末点的切线
            SetRopeLength(rope);
        }
        /// <summary>
        /// 根据两端点位信息修改-设置Rope长度、name、property
        /// </summary>
        /// <param name="rope"></param>
        public void SetRopeLength(ObiRopeBase rope)
        {
            if (RopeVerify(rope) == false)
            {
                Debug.LogError("未找到正确的Rope相关资源, 退出方法体...");
                return;
            }
            //始末点设置--长度控制
            int StartIndex = 0;
            int EndIndex = rope.path.ControlPointCount - 1;
            rope.path.points.data[StartIndex] = new ObiWingedPoint(InTangent, new Vector3(-(m_RopeLength * 0.5f), 0, 0), OutTangent);
            rope.path.points.data[EndIndex] = new ObiWingedPoint(InTangent, new Vector3(m_RopeLength * 0.5f, 0, 0), OutTangent);

            rope.path.SetName(StartIndex, "Start");
            rope.path.SetName(EndIndex, "End");

            SetControlPointProperty(rope, StartIndex);
            SetControlPointProperty(rope, EndIndex);
        }
        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="rope">目标Obi绳索实例</param>
        public void RemoveControlPoint(ObiRopeBase rope, ObiPointRemoveMode mode = ObiPointRemoveMode.Middle)
        {
            if (RopeVerify(rope) == false)
            {
                Debug.LogError("未找到正确的Rope相关资源, 退出方法体...");
                return;
            }
            // 移除中间的控制点
            onControlPointChanged?.Invoke();
            switch (mode)
            {
                case ObiPointRemoveMode.All:
                    Utils.DebugLog(Color.green, "重置节点...");
                    rope.path.Clear();
                    break;
                case ObiPointRemoveMode.Middle:
                    RemoveMiddleControlPoint(rope);
                    break;
                default:
                    RemoveMiddleControlPoint(rope);
                    break;
            }

            void RemoveMiddleControlPoint(ObiRopeBase rope)
            {
                int pointCount = rope.path.ControlPointCount;
                if (pointCount > 2)
                {
                    for (int i = 1; i < pointCount - 1; i++)
                    {
                        //每次清除第二个controlPoint,防止索引越界
                        rope.path.RemoveControlPoint(1);
                    }
                }
            }
        }
        #endregion



        #region 工具方法
        /// <summary>
        /// 对rope和蓝图资源进行查找验证
        /// </summary>
        /// <param name="rope">目标Obi绳索实例</param>
        /// <returns></returns>
        private bool RopeVerify(ObiRopeBase rope)
        {
            if (rope == null || rope.path == null)
            {
                Debug.LogError("无效的Obi绳索或路径");
                return false;
            }

            // 获取绳索蓝图（用于Undo记录, 编辑器环境需要）
            var blueprint = rope.sharedBlueprint as ObiRopeBlueprintBase;
            if (blueprint == null)
            {
                Debug.LogError("绳索缺少有效蓝图");
                return false;
            }

            return true;
        }
        /// <summary>
        /// 设置节点属性(质量, 类别, 交互类)
        /// </summary>
        /// <param name="rope">目标Obi绳索实例</param>
        /// <param name="index">节点索引</param>
        private void SetControlPointProperty(ObiRopeBase rope, int index)
        {
            rope.path.masses[index] = m_Mass;
            rope.path.filters[index] = ObiUtils.MakeFilter(ObiUtils.GetMaskFromFilter(rope.path.filters[index]), m_Category);
            rope.path.filters[index] = ObiUtils.MakeFilter(m_Mask, ObiUtils.GetCategoryFromFilter(rope.path.filters[index]));
        }

        #endregion
    }

}
