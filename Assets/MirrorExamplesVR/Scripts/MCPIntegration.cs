using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

[System.Serializable]
public class MCPRequest
{
    public string method;
    public Dictionary<string, object> parameters;
}

[System.Serializable]
public class MCPResponse
{
    public bool success;
    public string message;
    public object data;
}

public class MCPIntegration : MonoBehaviour
{
    [Header("MCP Server Configuration")]
    public string mcpServerUrl = "http://localhost:3000/api/mcp";
    public string apiKey = "your-api-key-here";
    public float requestTimeout = 10f;
    
    [Header("Debug Settings")]
    public bool enableDebugLogs = true;
    
    private Queue<MCPRequest> requestQueue = new Queue<MCPRequest>();
    private bool isProcessingRequest = false;
    
    void Start()
    {
        // 初始化MCP连接
        InitializeMCPConnection();
    }
    
    /// <summary>
    /// 初始化MCP连接
    /// </summary>
    public void InitializeMCPConnection()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"Initializing MCP connection to: {mcpServerUrl}");
        }
        
        // 发送初始化请求
        SendMCPRequest("initialize", new Dictionary<string, object>
        {
            {"client_type", "unity"},
            {"version", Application.unityVersion}
        });
    }
    
    /// <summary>
    /// 发送MCP请求
    /// </summary>
    public void SendMCPRequest(string method, Dictionary<string, object> parameters = null)
    {
        if (parameters == null)
        {
            parameters = new Dictionary<string, object>();
        }
        
        MCPRequest request = new MCPRequest
        {
            method = method,
            parameters = parameters
        };
        
        requestQueue.Enqueue(request);
        
        if (!isProcessingRequest)
        {
            StartCoroutine(ProcessRequestQueue());
        }
    }
    
    /// <summary>
    /// 处理请求队列
    /// </summary>
    private IEnumerator ProcessRequestQueue()
    {
        isProcessingRequest = true;
        
        while (requestQueue.Count > 0)
        {
            MCPRequest request = requestQueue.Dequeue();
            yield return StartCoroutine(SendHTTPRequest(request));
        }
        
        isProcessingRequest = false;
    }
    
    /// <summary>
    /// 发送HTTP请求到MCP服务器
    /// </summary>
    private IEnumerator SendHTTPRequest(MCPRequest request)
    {
        string jsonData = JsonUtility.ToJson(request);
        
        if (enableDebugLogs)
        {
            Debug.Log($"Sending MCP request: {jsonData}");
        }
        
        using (UnityWebRequest webRequest = new UnityWebRequest(mcpServerUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            webRequest.timeout = (int)requestTimeout;
            
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string responseText = webRequest.downloadHandler.text;
                
                if (enableDebugLogs)
                {
                    Debug.Log($"MCP Response: {responseText}");
                }
                
                // 处理响应
                HandleMCPResponse(responseText, request.method);
            }
            else
            {
                Debug.LogError($"MCP Request failed: {webRequest.error}");
                HandleMCPError(webRequest.error, request.method);
            }
        }
    }
    
    /// <summary>
    /// 处理MCP响应
    /// </summary>
    private void HandleMCPResponse(string responseText, string method)
    {
        try
        {
            MCPResponse response = JsonUtility.FromJson<MCPResponse>(responseText);
            
            if (response.success)
            {
                switch (method)
                {
                    case "initialize":
                        Debug.Log("MCP connection initialized successfully");
                        break;
                    case "create_cube":
                        Debug.Log("Cube created via MCP");
                        break;
                    case "delete_cube":
                        Debug.Log("Cube deleted via MCP");
                        break;
                    default:
                        Debug.Log($"MCP method {method} completed successfully");
                        break;
                }
            }
            else
            {
                Debug.LogError($"MCP method {method} failed: {response.message}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to parse MCP response: {e.Message}");
        }
    }
    
    /// <summary>
    /// 处理MCP错误
    /// </summary>
    private void HandleMCPError(string error, string method)
    {
        Debug.LogError($"MCP method {method} error: {error}");
    }
    
    /// <summary>
    /// 创建cube的MCP方法
    /// </summary>
    public void CreateCubeViaMCP(Vector3 position, Vector3 scale, string name = "MCP_Cube")
    {
        SendMCPRequest("create_cube", new Dictionary<string, object>
        {
            {"name", name},
            {"position", new Dictionary<string, float>
            {
                {"x", position.x},
                {"y", position.y},
                {"z", position.z}
            }},
            {"scale", new Dictionary<string, float>
            {
                {"x", scale.x},
                {"y", scale.y},
                {"z", scale.z}
            }}
        });
    }
    
    /// <summary>
    /// 删除cube的MCP方法
    /// </summary>
    public void DeleteCubeViaMCP(string cubeName)
    {
        SendMCPRequest("delete_cube", new Dictionary<string, object>
        {
            {"name", cubeName}
        });
    }
    
    /// <summary>
    /// 获取场景中所有cubes的MCP方法
    /// </summary>
    public void GetCubesViaMCP()
    {
        SendMCPRequest("get_cubes", new Dictionary<string, object>());
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 120, 300, 150));
        
        GUILayout.Label("MCP Integration Controls:");
        
        if (GUILayout.Button("Initialize MCP"))
        {
            InitializeMCPConnection();
        }
        
        if (GUILayout.Button("Create Cube via MCP"))
        {
            CreateCubeViaMCP(Vector3.zero, Vector3.one, "MCP_Cube_" + Time.time);
        }
        
        if (GUILayout.Button("Get All Cubes"))
        {
            GetCubesViaMCP();
        }
        
        GUILayout.Label($"Requests in queue: {requestQueue.Count}");
        GUILayout.Label($"Processing: {isProcessingRequest}");
        
        GUILayout.EndArea();
    }
}