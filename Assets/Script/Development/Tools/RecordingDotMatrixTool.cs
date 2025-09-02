using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RecordingDotMatrixTool : MonoBehaviour
{
    public bool selectAllDoF = false;
    public LayerMask m_TriggerLayer = 0;
    public string Direction;
    public float StartDistance = 10;
    public Vector2 RecordRange = new Vector2(3, 3);
    public Vector2 RecordCount = new Vector2(30, 30);
    private Dictionary<String, Vector3> rayStartPoints = new Dictionary<String, Vector3>();
    private Transform TestPointParent;
    
    
    public bool NeedVerifyLayerMask = false;

    // 显示顺序
    public int[] displayOrder = new int[6] { 1, 2, 3, 4, 5, 0 };
    // 计算组名称数组，用于显示每个方向的计算组名称
    public string[] calculateGroupNames = new string[6] { "Down to Up", "Up to Down", "Right to Left", "Left to Right", "Back to Forward", "Forward to Back" };
    // 计算组数组，用于存储每个方向的计算组状态
    public bool[] calculateGroup = new bool[6] { false, true, true, true, true, true };
    // 方向索引数组，用于存储下拉选项的索引
    public int[] selectedIndex = new int[6] { 0, 1, 2, 3, 4, 5 };
    // 下拉选项
    public string[] directionOptions = new string[] { "Up", "Down", "Left", "Right", "Forward", "Back" };
    // 计算方向数组，用于存储每个方向的计算方向
    public string[] calculateDirection = new string[6] { "Down to Up", "Up to Down", "Right to Left", "Left to Right", "Back to Forward", "Forward to Back" };
    // 起始距离数组，用于存储每个方向的起始距离
    public int[] StartDistanceGroups = new int[6] { -10, 10, 10, -10, -10, 10 };
    // 记录范围数组，用于存储每个方向的记录范围
    public Vector2[] RecordRangeGroups = new Vector2[6] { new Vector2(10, 10), new Vector2(10, 10), new Vector2(10, 10), new Vector2(10, 10), new Vector2(10, 10), new Vector2(10, 10) };
    // 记录数量数组，用于存储每个方向的记录数量
    public Vector2[] RecordCountGroups = new Vector2[6] { new Vector2(20, 20), new Vector2(20, 20), new Vector2(20, 20), new Vector2(20, 20), new Vector2(20, 20), new Vector2(20, 20) };

    public bool StartRecord(float _StartDistance, string _Direction, Vector2 _RecordRange, Vector2 _RecordCount, bool Verify = false)
    {
        if (Vaildate(_StartDistance, _Direction, _RecordRange, _RecordCount) == false)
            return false;

        // 记录逻辑
        Vector3 CurPos = this.transform.position;
        Vector3 StartPos = CalculateStartPos(_StartDistance, _Direction, CurPos);
        //生成射线起始点
        rayStartPoints = GenerateRayStartPoints(_Direction, _RecordRange, _RecordCount, StartPos);

        foreach (var point in rayStartPoints.Keys)
        {
            // Debug.DrawLine(rayStartPoints[point], rayStartPoints[point] + Vector3.down, Color.red, 5f);
            // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // sphere.transform.position = rayStartPoints[point];
            // sphere.transform.localScale = Vector3.one * 0.1f; // 设置球体大小
            // sphere.name = point; // 设置球体名称

            //创建射线
            Ray ray = new Ray(rayStartPoints[point], CalculateRayDirection(_Direction));
            RaycastHit hit;
            Debug.DrawLine(rayStartPoints[point], rayStartPoints[point] + (CalculateRayDirection(_Direction) * 100), Color.red, 1f);
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.transform == null)
                {
                    Debug.LogWarning($"Raycast hit null transform at point {point}");
                    continue;
                }
                if (Verify && (m_TriggerLayer.value & (1 << hit.transform.gameObject.layer)) == 0)
                {
                    continue;
                }
                Debug.Log($"Raycast hit {hit.transform.name} at point {point}");

                // 处理射线命中逻辑
                Vector3 HitPoint = hit.point;
                //验证点的位置，创建一个sphere
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = HitPoint;
                sphere.transform.localScale = Vector3.one * 0.1f; // 设置球体大小
                sphere.name = point; // 设置球体名称
                if (TestPointParent == null)
                {
                    TestPointParent = new GameObject("TestPointParent").transform;
                    TestPointParent.position = Vector3.zero;
                    TestPointParent.rotation = Quaternion.identity;
                    TestPointParent.localScale = Vector3.one;
                }
                sphere.transform.parent = TestPointParent;
            }

        }

        return true;
    }
    /// <summary>
    /// 生成射线起始点
    /// </summary>
    /// <param name="centerPos"></param>
    /// <returns></returns>
    private Dictionary<String, Vector3> GenerateRayStartPoints(string _Direction, Vector2 _RecordRange, Vector2 _RecordCount, Vector3 centerPos)
    {
        Dictionary<String, Vector3> points = new Dictionary<String, Vector3>();

        // 根据方向确定生成平面
        Vector3 axis1 = Vector3.zero;
        Vector3 axis2 = Vector3.zero;

        switch (_Direction)
        {
            case "Up":
            case "Down":
                axis1 = Vector3.right;
                axis2 = Vector3.forward;
                break;
            case "Left":
            case "Right":
                axis1 = Vector3.up;
                axis2 = Vector3.forward;
                break;
            case "Forward":
            case "Back":
                axis1 = Vector3.right;
                axis2 = Vector3.up;
                break;
        }

        // 计算步长
        Vector2 stepSize = new Vector2(
            _RecordRange.x / (_RecordCount.x - 1),
            _RecordRange.y / (_RecordCount.y - 1)
        );

        // 生成网格点
        for (int i = 0; i < _RecordCount.x; i++)
        {
            for (int j = 0; j < _RecordCount.y; j++)
            {
                // 计算偏移量
                float offsetX = (i * stepSize.x) - (_RecordRange.x / 2);
                float offsetY = (j * stepSize.y) - (_RecordRange.y / 2);

                // 计算点位置
                Vector3 point = centerPos + (axis1 * offsetX) + (axis2 * offsetY);
                points.Add($"{i}_{j}", point);
            }
        }

        return points;
    }
    /// <summary>
    /// 计算起始位置
    /// </summary>
    /// <param name="CurPos"></param>
    /// <returns></returns>
    private Vector3 CalculateStartPos(float _StartDistance, string _Direction, Vector3 CurPos)
    {
        Vector3 StartPos = CurPos;
        switch (_Direction)
        {
            case "Up":
            case "Down":
                StartPos += Vector3.up * _StartDistance;
                break;
            case "Left":
            case "Right":
                StartPos += Vector3.right * _StartDistance;
                break;
            case "Forward":
            case "Back":
                StartPos += Vector3.forward * _StartDistance;
                break;
            default:
                Debug.LogError("Direction is invalid");
                return StartPos;
        }
        return StartPos;
    }
    /// <summary>
    /// 验证参数是否正确
    /// </summary>
    /// <returns></returns>
    private bool Vaildate(float _StartDistance, string _Direction, Vector2 _RecordRange, Vector2 _RecordCount)
    {
        if (string.IsNullOrEmpty(_Direction))
        {
            Debug.LogError("Direction is empty");
            return false;
        }
        if (_StartDistance == 0)
        {
            Debug.LogError("StartDistance is 0");
            return false;

        }
        if (_RecordRange.x <= 0 || _RecordRange.y <= 0)
        {
            Debug.LogError("RecordRange is invalid");
            return false;
        }
        if (_RecordCount.x <= 0 || _RecordCount.y <= 0)
        {
            Debug.LogError("RecordCount is invalid");
            return false;
        }
        return true;
    }
    private Vector3 CalculateRayDirection(string _direction)
    {
        switch (_direction)
        {
            case "Up":
                return Vector3.up;
            case "Down":
                return Vector3.down;
            case "Left":
                return Vector3.left;
            case "Right":
                return Vector3.right;
            case "Forward":
                return Vector3.forward;
            case "Back":
                return Vector3.back;
            default:
                Debug.LogError("Direction is invalid");
                return Vector3.zero;
        }
    }



}

