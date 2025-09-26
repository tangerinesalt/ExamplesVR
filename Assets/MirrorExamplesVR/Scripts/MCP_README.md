# MCP Cube Creator for Unity

这个Unity项目包含了使用MCP (Model Context Protocol) 创建和管理3D cubes的功能。

## 文件说明

### 1. MCPCubeCreator.cs
主要的cube创建脚本，提供以下功能：
- 创建3D cubes
- 管理创建的cubes列表
- 基本的物理属性设置
- 简单的UI控制界面

**主要方法：**
- `CreateCube()` - 创建一个新的cube
- `ClearAllCubes()` - 删除所有创建的cubes
- `CreateCubeViaMCP()` - 通过MCP创建cube

### 2. MCPIntegration.cs
MCP服务器集成脚本，提供以下功能：
- 与MCP服务器通信
- 发送HTTP请求
- 处理MCP响应
- 队列管理

**主要方法：**
- `InitializeMCPConnection()` - 初始化MCP连接
- `CreateCubeViaMCP()` - 通过MCP创建cube
- `DeleteCubeViaMCP()` - 通过MCP删除cube
- `GetCubesViaMCP()` - 获取所有cubes

### 3. MCPExample.cs
示例脚本，展示如何使用MCP功能：
- 自动创建多个cubes
- 随机位置生成
- 示例控制界面

## 使用方法

### 基本设置

1. 将脚本添加到场景中的GameObject上
2. 配置MCP服务器设置：
   - `mcpServerUrl`: MCP服务器地址
   - `apiKey`: API密钥

### 键盘快捷键

- `C` - 创建cube
- `X` - 清除所有cubes
- `R` - 重新运行示例

### 通过代码使用

```csharp
// 获取MCP组件
MCPCubeCreator cubeCreator = FindObjectOfType<MCPCubeCreator>();
MCPIntegration mcpIntegration = FindObjectOfType<MCPIntegration>();

// 创建cube
cubeCreator.CreateCube();

// 通过MCP创建cube
mcpIntegration.CreateCubeViaMCP(Vector3.zero, Vector3.one, "MyCube");
```

## MCP服务器配置

确保您的MCP服务器支持以下API端点：

- `POST /api/mcp` - 主要API端点
- 支持的方法：
  - `initialize` - 初始化连接
  - `create_cube` - 创建cube
  - `delete_cube` - 删除cube
  - `get_cubes` - 获取所有cubes

## 注意事项

1. 确保MCP服务器正在运行
2. 检查网络连接和API密钥
3. 查看Unity Console获取调试信息
4. 可以根据需要修改cube的材质和物理属性

## 扩展功能

您可以扩展这些脚本来支持：
- 更多3D对象类型
- 复杂的MCP命令
- 实时同步
- 多人协作功能