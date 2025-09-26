using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCPExample : MonoBehaviour
{
    [Header("MCP Components")]
    public MCPCubeCreator cubeCreator;
    public MCPIntegration mcpIntegration;
    
    [Header("Example Settings")]
    public int numberOfCubes = 5;
    public float spawnRadius = 5f;
    public float spawnHeight = 1f;
    
    void Start()
    {
        // 获取组件引用
        if (cubeCreator == null)
            cubeCreator = FindObjectOfType<MCPCubeCreator>();
        
        if (mcpIntegration == null)
            mcpIntegration = FindObjectOfType<MCPIntegration>();
        
        // 开始示例
        StartCoroutine(RunMCPExample());
    }
    
    /// <summary>
    /// 运行MCP示例
    /// </summary>
    private IEnumerator RunMCPExample()
    {
        Debug.Log("Starting MCP Example...");
        
        // 等待MCP初始化
        yield return new WaitForSeconds(1f);
        
        // 创建多个cubes
        for (int i = 0; i < numberOfCubes; i++)
        {
            // 计算随机位置
            Vector3 randomPosition = GetRandomPosition();
            
            // 通过MCP创建cube
            if (mcpIntegration != null)
            {
                mcpIntegration.CreateCubeViaMCP(randomPosition, Vector3.one, $"Example_Cube_{i}");
            }
            
            // 本地也创建一个cube
            if (cubeCreator != null)
            {
                cubeCreator.spawnPosition = randomPosition;
                cubeCreator.CreateCube();
            }
            
            yield return new WaitForSeconds(0.5f);
        }
        
        Debug.Log("MCP Example completed!");
    }
    
    /// <summary>
    /// 获取随机位置
    /// </summary>
    private Vector3 GetRandomPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        return new Vector3(randomCircle.x, spawnHeight, randomCircle.y);
    }
    
    /// <summary>
    /// 重新运行示例
    /// </summary>
    public void RestartExample()
    {
        // 清除现有的cubes
        if (cubeCreator != null)
        {
            cubeCreator.ClearAllCubes();
        }
        
        // 重新开始示例
        StartCoroutine(RunMCPExample());
    }
    
    void Update()
    {
        // 按R键重新开始示例
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartExample();
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 280, 200, 100));
        
        GUILayout.Label("MCP Example Controls:");
        
        if (GUILayout.Button("Restart Example (R)"))
        {
            RestartExample();
        }
        
        GUILayout.Label($"Cubes to create: {numberOfCubes}");
        
        GUILayout.EndArea();
    }
}