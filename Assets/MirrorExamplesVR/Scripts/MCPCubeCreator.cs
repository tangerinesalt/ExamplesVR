using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCPCubeCreator : MonoBehaviour
{
    [Header("Cube Creation Settings")]
    public Material cubeMaterial;
    public Vector3 cubeSize = Vector3.one;
    public Vector3 spawnPosition = Vector3.zero;
    
    [Header("MCP Settings")]
    public string mcpServerUrl = "http://localhost:3000";
    public string mcpApiKey = "your-api-key-here";
    
    private List<GameObject> createdCubes = new List<GameObject>();
    
    void Start()
    {
        // 自动创建一个cube作为示例
        CreateCube();
    }
    
    /// <summary>
    /// 创建一个新的cube
    /// </summary>
    public void CreateCube()
    {
        // 创建cube GameObject
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "MCP_Cube_" + createdCubes.Count;
        
        // 设置位置
        cube.transform.position = spawnPosition + Vector3.up * createdCubes.Count * 2f;
        
        // 设置大小
        cube.transform.localScale = cubeSize;
        
        // 添加材质
        if (cubeMaterial != null)
        {
            cube.GetComponent<Renderer>().material = cubeMaterial;
        }
        
        // 添加刚体组件用于物理交互
        Rigidbody rb = cube.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = cube.AddComponent<Rigidbody>();
        }
        
        // 添加碰撞器（默认cube已经有BoxCollider）
        
        // 添加到列表
        createdCubes.Add(cube);
        
        Debug.Log($"Created cube: {cube.name} at position {cube.transform.position}");
        
        // 如果启用了MCP，发送创建事件
        if (IsMCPEnabled())
        {
            SendMCPEvent("cube_created", new Dictionary<string, object>
            {
                {"cube_name", cube.name},
                {"position", cube.transform.position},
                {"scale", cube.transform.localScale}
            });
        }
    }
    
    /// <summary>
    /// 删除所有创建的cubes
    /// </summary>
    public void ClearAllCubes()
    {
        foreach (GameObject cube in createdCubes)
        {
            if (cube != null)
            {
                DestroyImmediate(cube);
            }
        }
        createdCubes.Clear();
        
        Debug.Log("Cleared all MCP cubes");
        
        if (IsMCPEnabled())
        {
            SendMCPEvent("cubes_cleared", new Dictionary<string, object>());
        }
    }
    
    /// <summary>
    /// 检查MCP是否启用
    /// </summary>
    private bool IsMCPEnabled()
    {
        return !string.IsNullOrEmpty(mcpServerUrl) && !string.IsNullOrEmpty(mcpApiKey);
    }
    
    /// <summary>
    /// 发送MCP事件
    /// </summary>
    private void SendMCPEvent(string eventType, Dictionary<string, object> data)
    {
        // 这里可以集成实际的MCP API调用
        Debug.Log($"MCP Event: {eventType}, Data: {JsonUtility.ToJson(data)}");
        
        // 实际实现中，这里应该发送HTTP请求到MCP服务器
        // 例如使用UnityWebRequest或类似的方法
    }
    
    /// <summary>
    /// 通过MCP创建cube（外部调用接口）
    /// </summary>
    public void CreateCubeViaMCP()
    {
        if (IsMCPEnabled())
        {
            CreateCube();
        }
        else
        {
            Debug.LogWarning("MCP is not properly configured. Please set mcpServerUrl and mcpApiKey.");
        }
    }
    
    void Update()
    {
        // 键盘快捷键创建cube
        if (Input.GetKeyDown(KeyCode.C))
        {
            CreateCube();
        }
        
        // 键盘快捷键清除所有cubes
        if (Input.GetKeyDown(KeyCode.X))
        {
            ClearAllCubes();
        }
    }
    
    void OnGUI()
    {
        // 简单的UI按钮
        GUILayout.BeginArea(new Rect(10, 10, 200, 100));
        
        if (GUILayout.Button("Create Cube (C)"))
        {
            CreateCube();
        }
        
        if (GUILayout.Button("Clear All Cubes (X)"))
        {
            ClearAllCubes();
        }
        
        GUILayout.Label($"Cubes Created: {createdCubes.Count}");
        
        GUILayout.EndArea();
    }
}