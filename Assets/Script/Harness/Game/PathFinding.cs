/****************************************************
    功能：寻路系统
    作者：ZH
    创建日期：#2025/03/10#
    修改内容：
        1.代码优化    2025/03/10 ZH
        2.路径参数封装结构    2025/03/17 ZH
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;

namespace Voltage
{
    [Serializable]
    public class PathSmoothingSettings
    {
        /// <summary>
        /// 是否平滑路径
        /// </summary>
        public bool useCatmullRom = true;
        /// <summary>
        /// 路径简化精度
        /// </summary>
        public float precision = 1.5f;
        /// <summary>
        /// 平滑分段数
        /// </summary>
        public int smoothSegments = 200;
        /// <summary>
        /// 结束寻路距离
        /// </summary>
        public float endDistance = 0.6f;
        /// <summary>
        /// 线条动画速度
        /// </summary>
        public float lineAnimSpeed = 1f;
        /// <summary>
        /// 线条偏离高度
        /// </summary>
        public float lineOffsetHeight = 0.01f;
    }

    [Serializable]
    public class ParticleEffectSettings
    {
        /// <summary>
        /// 启用粒子效果
        /// </summary>
        public bool enable = true;
        /// <summary>
        /// 循环模式
        /// </summary>
        public bool loop = false;
        /// <summary>
        /// 跟随固定点移动速度
        /// </summary>
        public float followFixedPosMoveSpeed = 2f;
        /// <summary>
        /// 跟随距离
        /// </summary>
        public float followDistance = 0.001f;
        /// <summary>
        /// 跟随缓动速度
        /// </summary>
        public float followSlowSpeed = 0.05f;
        /// <summary>
        /// 固定距离检测
        /// </summary>
        public float checkDistance = 0.5f;
        /// <summary>
        /// 粒子循环移动速度
        /// </summary>
        public float particleLoopMoveSpeed = 1f;
        /// <summary>
        /// 粒子高度
        /// </summary>
        public float particleHeight = 1f;
    }

    public class PathFinding : MonoSingleton<PathFinding>
    {
        #region 监视面板变量
        [Header("关键组件")]
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform particleRoot;
        [SerializeField] private Transform fixedMark;

        [Header("路径设置")]
        [SerializeField] private PathSmoothingSettings pathSettings;

        [Header("粒子设置")]
        [SerializeField] private ParticleEffectSettings particleSettings;

        #endregion

        #region 运行时变量
        /// <summary>
        /// 导航状态
        /// </summary>
        private bool _isNavigating;
        /// <summary>
        /// 当前目标
        /// </summary>
        private Transform _currentTarget;
        /// <summary>
        /// 平滑路径
        /// </summary>
        private List<Vector3> _smoothedPath = new List<Vector3>();
        /// <summary>
        /// 当前路径点索引
        /// </summary>
        private int currentWayId = 0;
        /// <summary>
        /// 贴图偏移量
        /// </summary>
        private Vector2 _textureOffset;
        /// <summary>
        /// 粒子高度偏移量
        /// </summary>
        private Vector3 _particleVerticalOffset;
        #endregion

        #region 属性
        public List<Vector3> CurrentPath => agent.path?.corners.ToList() ?? new List<Vector3>();
        public bool IsNavigating => _isNavigating;
        #endregion

        #region Unity 生命周期
        private void Start() => InitializeComponents();
        private void Update() => UpdateNavigation();
        private void OnDestroy() => StopAllCoroutines();
        #endregion

        #region 公共方法
        /// <summary> 设置导航目标 </summary>
        public void SetNavigationTarget(Transform target)
        {
            if (agent == null || target == null) return;

            _currentTarget = target;
            agent.transform.localPosition = Vector3.zero;
            fixedMark.position = agent.transform.position;
            particleRoot.transform.position=agent.transform.position;
            agent.SetDestination(target.position);

            StartCoroutine(ProcessPathCalculation());
        }

        /// <summary> 切换导航状态 </summary>
        public void ToggleNavigation(bool enable)
        {
            _isNavigating = enable;
            UpdateVisuals(enable);
        }
        
        /// <summary> 更新agent位置 </summary>
        public void UpdateTransform()
        {
            Vector3 pos = agent.transform.localPosition;
            agent.gameObject.SetActive(false);
            agent.transform.localPosition = new Vector3(0, pos.y,0);
            agent.gameObject.SetActive(true);
        } 
        #endregion

        #region 核心逻辑
        private void InitializeComponents()
        {
            Debug.Assert(agent != null, "NavMeshAgent组件丢失!");
            UpdateVisuals(false);
            
        }

        private void UpdateNavigation()
        {
            if (!_isNavigating) return;

            agent.transform.localPosition = Vector3.zero;

            UpdateLineRenderer();
            UpdateParticleEffects();
            CheckNavigationCompletion();
        }

        private IEnumerator ProcessPathCalculation()
        {
            particleRoot.gameObject.SetActive(false);
            yield return new WaitUntil(() => !agent.pathPending);

            _isNavigating = true;
            currentWayId = 0;
            UpdateVisuals(true);
        }
        #endregion

        #region 路径处理
        private void CheckNavigationCompletion()
        {
            if (Utils.CalculatePathLength(CurrentPath) <= pathSettings.endDistance)
            {
                ToggleNavigation(false);
                _currentTarget?.gameObject.SetActive(false);
            }
        }

        private List<Vector3> SimplifyPath(List<Vector3> path)
        {
            List<Vector3> simplified = new List<Vector3> { path[0] };
            Vector3 lastPoint = path[0];

            for (int i = 1; i < path.Count; i++)
            {
                if (Vector3.Distance(path[i], lastPoint) > pathSettings.precision)
                {
                    simplified.Add(path[i]);
                    lastPoint = path[i];
                }
            }
            return simplified;
        }

        private void GenerateSmoothedPath(List<Vector3> simplifiedPath)
        {
            _smoothedPath.Clear();
            if (simplifiedPath.Count < 2) return;

            Vector3[] controlPoints = Utils.GenerateCatmullRomControlPoints(simplifiedPath.ToArray());
            int totalPoints = (simplifiedPath.Count - 1) * pathSettings.smoothSegments;

            for (int i = 0; i <= totalPoints; i++)
            {
                float t = (float)i / totalPoints;
                _smoothedPath.Add(Utils.InterpolateCatmullRom(controlPoints, t));
            }
        }
        #endregion

        #region Visual Effects
        private void UpdateVisuals(bool enable)
        {
            lineRenderer.gameObject.SetActive(enable);
            particleRoot.gameObject.SetActive(enable);
            fixedMark.gameObject.SetActive(enable);
        }

        private void UpdateLineRenderer()
        {
            if (pathSettings.useCatmullRom)
            {
                GenerateSmoothedPath(SimplifyPath(CurrentPath));
                lineRenderer.positionCount = _smoothedPath.Count;
                for (int i = 0; i < _smoothedPath.Count; i++)
                    lineRenderer.SetPosition(i, _smoothedPath[i] + Vector3.up * pathSettings.lineOffsetHeight);
            }
            else
            {
                lineRenderer.positionCount = CurrentPath.Count;
                for (int i = 0; i < CurrentPath.Count; i++)
                    lineRenderer.SetPosition(i, CurrentPath[i] + Vector3.up * pathSettings.lineOffsetHeight);
            }

            UpdateLineAnimation();
        }

        private void UpdateLineAnimation()
        {
            _textureOffset.x -= Time.deltaTime * pathSettings.lineAnimSpeed;
            lineRenderer.material.mainTextureOffset = _textureOffset;
        }

        private void UpdateParticleEffects()
        {
            if (!particleSettings.enable) return;

            FixedDisParticleFollow();

            _particleVerticalOffset = Vector3.up * particleSettings.particleHeight;

            if (particleSettings.loop)
                UpdateLoopingParticles();
            else
                UpdateFixedDistanceParticles();
        }

        /// <summary>
        /// 粒子跟随
        /// </summary>
        private void FixedDisParticleFollow()
        {
            Vector3 fixedMarkPos = fixedMark.position;
            Vector3 fixedMarkForward = fixedMark.forward;
            Vector3 targetPosition = fixedMarkPos - fixedMarkForward * particleSettings.followDistance + Vector3.up * (particleSettings.followDistance * 0.5f);

            Vector3 currentPos = particleRoot.position;
            float distance = Vector3.Distance(currentPos, fixedMarkPos);
            float moveDelta = (particleSettings.followFixedPosMoveSpeed + distance * particleSettings.followSlowSpeed) * Time.deltaTime;

            particleRoot.position = Vector3.MoveTowards(currentPos, targetPosition, moveDelta);
            particleRoot.LookAt(fixedMarkPos);
        }

        /// <summary>
        /// 粒子循环移动
        /// </summary>
        private void UpdateLoopingParticles()
        {
            if (currentWayId >= _smoothedPath.Count)
            {
                Vector3 startPos = agent.transform.position + _particleVerticalOffset;
                fixedMark.position = startPos;
                particleRoot.position = startPos;
                currentWayId = 0;
                return;
            }

            Vector3 targetPos = _smoothedPath[currentWayId] + _particleVerticalOffset;
            Vector3 currentMarkPos = fixedMark.position;

            fixedMark.LookAt(targetPos);
            fixedMark.position = Vector3.MoveTowards(currentMarkPos, targetPos, particleSettings.particleLoopMoveSpeed * Time.deltaTime);

            if (Vector3.SqrMagnitude(fixedMark.position - targetPos) < 0.0001f) currentWayId++;
        }

        /// <summary>
        /// 粒子保持固定距离
        /// </summary>
        private void UpdateFixedDistanceParticles()
        {
            float dis = 0;
            Vector3 endPos;

            for (int i = 1; i < _smoothedPath.Count; i++)
            {
                float d = Vector3.Distance(_smoothedPath[i], _smoothedPath[i - 1]);
                dis += d;
                if (dis >= particleSettings.checkDistance)
                {
                    Vector3 dir = (_smoothedPath[i] - _smoothedPath[i - 1]).normalized;
                    endPos = _smoothedPath[i] - dir * (dis - particleSettings.checkDistance);
                    fixedMark.transform.position = endPos + _particleVerticalOffset;
                    fixedMark.LookAt(_smoothedPath[i] + _particleVerticalOffset);
                    break;
                }
            }
        }
        #endregion
    }
}