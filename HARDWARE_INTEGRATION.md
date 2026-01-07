# 智能门锁硬件集成指南

## 概述

本文档说明如何将酒店管理系统与智能门锁硬件设备进行集成对接。

## 支持的对接方式

### 1. MQTT协议集成（推荐）

**适用场景**：大多数物联网智能门锁，如涂鸦、TTLock等

**优势**：
- 实时性好
- 支持双向通信
- 低带宽消耗
- 断线重连机制

**集成流程**：

```
1. 门锁设备连接到MQTT Broker
2. 系统订阅门锁主题：hotel/locks/{deviceId}/command
3. 门锁订阅响应主题：hotel/locks/{deviceId}/response
4. 系统发送指令，门锁执行并返回结果
```

**消息格式示例**：

```json
// 系统发送开锁指令
{
  "command": "unlock",
  "deviceId": "LOCK001",
  "timestamp": "2026-01-07T10:30:00Z",
  "params": {
    "password": "123456",
    "validUntil": "2026-01-10T12:00:00Z"
  }
}

// 门锁返回响应
{
  "deviceId": "LOCK001",
  "result": "success",
  "timestamp": "2026-01-07T10:30:01Z",
  "batteryLevel": 85
}
```

### 2. HTTP API集成

**适用场景**：提供云平台API的智能门锁厂商

**优势**：
- 简单易用
- 无需额外中间件
- 标准RESTful接口

**集成示例**：

```csharp
public class DoorLockService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public async Task<bool> UnlockDoor(string deviceId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, 
            $"https://api.lockvendor.com/v1/locks/{deviceId}/unlock");
        
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");
        
        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    public async Task<string> GeneratePassword(string deviceId, DateTime validFrom, DateTime validUntil)
    {
        var payload = new
        {
            deviceId,
            validFrom,
            validUntil
        };
        
        var request = new HttpRequestMessage(HttpMethod.Post,
            $"https://api.lockvendor.com/v1/locks/{deviceId}/passwords");
        
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");
        request.Content = JsonContent.Create(payload);
        
        var response = await _httpClient.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<PasswordResponse>();
        
        return result?.Password ?? string.Empty;
    }
}
```

### 3. 蓝牙集成

**适用场景**：近距离操作，如前台办理入住时配置门锁

**注意事项**：
- 需要蓝牙网关或移动设备作为中继
- 适合一次性配置，不适合远程实时控制

## 推荐硬件厂商

### 1. TTLock（科技侠）

**特点**：
- 提供完整SDK和云平台API
- 支持MQTT和HTTP两种方式
- 国内市场占有率高

**集成方式**：
```csharp
// 安装SDK
dotnet add package TTLock.SDK

// 使用示例
var client = new TTLockClient(apiKey, secretKey);
var password = await client.CreateTemporaryPassword(
    lockId: "12345678",
    startDate: DateTime.Now,
    endDate: DateTime.Now.AddDays(3)
);
```

**API文档**：https://open.ttlock.com/doc

### 2. 涂鸦智能（Tuya）

**特点**：
- 全球化物联网平台
- 支持多种设备类型
- 提供完整的云服务

**集成方式**：
```csharp
// 安装SDK
dotnet add package TuyaConnector

// 使用示例
var tuyaClient = new TuyaOpenAPI(clientId, secret);
await tuyaClient.SendCommand(deviceId, new
{
    code = "unlock",
    value = true
});
```

**API文档**：https://developer.tuya.com/

### 3. 小米智能门锁

**特点**：
- 与小米生态绑定
- 质量稳定
- 需要通过米家云平台

**集成方式**：
- 需要注册小米开发者账号
- 使用米家开放平台API
- 文档：https://iot.mi.com/new/doc

## 集成步骤

### 第一步：选择硬件供应商

根据预算、功能需求和技术支持选择合适的供应商。

### 第二步：注册开发者账号

在供应商平台注册账号，获取API密钥或SDK。

### 第三步：设备注册

将门锁设备注册到系统中：

```bash
POST /api/doorlocks
{
  "deviceId": "LOCK001",
  "deviceName": "101房间门锁",
  "manufacturer": "TTLock",
  "model": "T1-Pro",
  "roomId": 1
}
```

### 第四步：实现服务层

创建硬件适配服务：

```csharp
public interface IDoorLockHardwareService
{
    Task<bool> UnlockAsync(string deviceId);
    Task<string> CreatePasswordAsync(string deviceId, DateTime validFrom, DateTime validUntil);
    Task<bool> DeletePasswordAsync(string deviceId, string passwordId);
    Task<DeviceStatus> GetStatusAsync(string deviceId);
}

// TTLock实现
public class TTLockService : IDoorLockHardwareService
{
    public async Task<string> CreatePasswordAsync(string deviceId, DateTime validFrom, DateTime validUntil)
    {
        // 调用TTLock API
        var password = await _ttlockClient.CreateTemporaryPassword(deviceId, validFrom, validUntil);
        return password;
    }
    
    // ... 其他方法实现
}
```

### 第五步：配置依赖注入

在 `Program.cs` 中注册服务：

```csharp
// 根据配置选择不同的实现
var lockProvider = builder.Configuration["DoorLock:Provider"];
if (lockProvider == "TTLock")
{
    builder.Services.AddSingleton<IDoorLockHardwareService, TTLockService>();
}
else if (lockProvider == "Tuya")
{
    builder.Services.AddSingleton<IDoorLockHardwareService, TuyaService>();
}
```

### 第六步：测试集成

使用Postman或Swagger测试API：

```bash
# 远程开锁
POST /api/doorlocks/1/unlock
{
  "roomId": 101,
  "userId": 1
}

# 查看开锁记录
GET /api/doorlocks/1/records
```

## 安全建议

1. **API密钥保护**：
   - 使用环境变量或Azure Key Vault存储API密钥
   - 不要将密钥提交到Git仓库

2. **通信加密**：
   - 使用HTTPS/TLS加密通信
   - MQTT使用SSL/TLS

3. **权限控制**：
   - 实现基于角色的访问控制
   - 记录所有开锁操作日志

4. **密码策略**：
   - 临时密码应设置有效期
   - 过期自动失效
   - 一次性密码使用后立即删除

## 故障排查

### 常见问题

**1. 门锁离线**
- 检查网络连接
- 检查电池电量
- 重启门锁设备

**2. 密码无效**
- 检查时间同步
- 确认密码有效期
- 检查密码是否已被删除

**3. API调用失败**
- 检查API密钥是否正确
- 检查设备ID是否存在
- 查看API调用限额

## 示例代码

完整的MQTT集成示例：

```csharp
using MQTTnet;
using MQTTnet.Client;

public class MqttDoorLockService
{
    private readonly IMqttClient _mqttClient;
    
    public async Task InitializeAsync()
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();
        
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("mqtt.example.com", 1883)
            .WithCredentials("username", "password")
            .Build();
            
        await _mqttClient.ConnectAsync(options);
        
        // 订阅门锁响应主题
        await _mqttClient.SubscribeAsync("hotel/locks/+/response");
        
        _mqttClient.ApplicationMessageReceivedAsync += HandleMessageAsync;
    }
    
    public async Task<bool> UnlockDoorAsync(string deviceId)
    {
        var message = new MqttApplicationMessageBuilder()
            .WithTopic($"hotel/locks/{deviceId}/command")
            .WithPayload(JsonSerializer.Serialize(new
            {
                command = "unlock",
                timestamp = DateTime.UtcNow
            }))
            .Build();
            
        await _mqttClient.PublishAsync(message);
        return true;
    }
    
    private Task HandleMessageAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
        // 处理门锁响应
        return Task.CompletedTask;
    }
}
```

## 下一步

1. 选择合适的硬件供应商
2. 购买测试设备
3. 实现对应的服务适配器
4. 进行现场测试
5. 部署到生产环境

## 技术支持

如有问题，请联系：
- 技术支持邮箱：support@yourhotel.com
- 厂商技术文档：参考各厂商官网
