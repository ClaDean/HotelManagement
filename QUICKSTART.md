# 快速启动指南

## 🚀 运行项目

### 1. 启动API服务

```bash
# 进入API项目目录
cd HotelManagement.API

# 运行项目
dotnet run
```

服务启动后会显示类似以下信息：
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

### 2. 访问Swagger文档

打开浏览器访问：
- https://localhost:7001/swagger （HTTPS）
- http://localhost:5000/swagger （HTTP）

在Swagger界面可以直接测试所有API接口。

## 📝 测试API

### 创建房间

```bash
POST https://localhost:7001/api/rooms
Content-Type: application/json

{
  "roomNumber": "101",
  "roomType": "标准间",
  "floor": 1,
  "status": "Available",
  "price": 288.00,
  "description": "温馨舒适的标准间"
}
```

### 注册门锁

```bash
POST https://localhost:7001/api/doorlocks
Content-Type: application/json

{
  "deviceId": "LOCK20260107001",
  "deviceName": "101房间门锁",
  "manufacturer": "TTLock",
  "model": "T1-Pro",
  "roomId": null
}
```

### 创建订单

```bash
POST https://localhost:7001/api/bookings
Content-Type: application/json

{
  "roomId": 1,
  "guestName": "张三",
  "guestPhone": "13800138000",
  "guestIdCard": "110101199001011234",
  "checkInTime": "2026-01-08T14:00:00",
  "checkOutTime": "2026-01-10T12:00:00",
  "totalPrice": 576.00,
  "paidAmount": 0
}
```

### 办理入住

```bash
POST https://localhost:7001/api/bookings/1/checkin
```

系统会自动：
1. 更新订单状态为"已入住"
2. 更新房间状态为"已占用"
3. 生成临时密码
4. 记录实际入住时间

### 远程开锁

```bash
POST https://localhost:7001/api/doorlocks/1/unlock
Content-Type: application/json

{
  "roomId": 1,
  "userId": 1
}
```

### 查看开锁记录

```bash
GET https://localhost:7001/api/doorlocks/1/records
```

## 🗄️ 数据库

项目使用SQLite数据库，数据库文件位于：
```
HotelManagement.API/hotel.db
```

首次运行时会自动创建数据库和表结构。

### 查看数据库

推荐使用以下工具查看SQLite数据库：
- [DB Browser for SQLite](https://sqlitebrowser.org/)
- [DBeaver](https://dbeaver.io/)
- VS Code扩展：SQLite Viewer

### 切换到SQL Server

如果要使用SQL Server，修改 [Program.cs](HotelManagement.API/Program.cs#L14)：

```csharp
// 注释掉SQLite
// options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection")));

// 启用SQL Server
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
```

然后更新 `appsettings.json` 中的连接字符串。

## 📊 API接口列表

### 房间管理
- `GET /api/rooms` - 获取所有房间
- `GET /api/rooms/{id}` - 获取指定房间
- `POST /api/rooms` - 创建房间
- `PUT /api/rooms/{id}` - 更新房间
- `DELETE /api/rooms/{id}` - 删除房间
- `PATCH /api/rooms/{id}/status` - 更新房间状态

### 门锁管理
- `GET /api/doorlocks` - 获取所有门锁
- `GET /api/doorlocks/{id}` - 获取指定门锁
- `POST /api/doorlocks` - 注册门锁
- `POST /api/doorlocks/{id}/unlock` - 远程开锁
- `GET /api/doorlocks/{id}/status` - 获取门锁状态
- `POST /api/doorlocks/{id}/heartbeat` - 更新心跳
- `GET /api/doorlocks/{id}/records` - 查看开锁记录

### 订单管理
- `GET /api/bookings` - 获取所有订单
- `GET /api/bookings/{id}` - 获取指定订单
- `POST /api/bookings` - 创建订单
- `POST /api/bookings/{id}/checkin` - 办理入住
- `POST /api/bookings/{id}/checkout` - 办理退房
- `POST /api/bookings/{id}/cancel` - 取消订单

## 🔧 开发环境配置

### 开发工具
- Visual Studio 2022 或 VS Code
- .NET 9.0 SDK
- Postman 或 Thunder Client（API测试）

### VS Code推荐扩展
- C# Dev Kit
- REST Client
- SQLite Viewer
- GitLens

### 热重载

项目支持热重载，修改代码后自动重新编译：
```bash
dotnet watch run
```

## 🐛 调试

### VS Code调试配置

按 `F5` 启动调试，或使用以下配置 `.vscode/launch.json`：

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (web)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/HotelManagement.API/bin/Debug/net9.0/HotelManagement.API.dll",
      "args": [],
      "cwd": "${workspaceFolder}/HotelManagement.API",
      "stopAtEntry": false,
      "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
      },
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "sourceFileMap": {
        "/Views": "${workspaceFolder}/Views"
      }
    }
  ]
}
```

## 📦 Docker部署（可选）

创建 `Dockerfile`：

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["HotelManagement.API/HotelManagement.API.csproj", "HotelManagement.API/"]
RUN dotnet restore "HotelManagement.API/HotelManagement.API.csproj"
COPY . .
WORKDIR "/src/HotelManagement.API"
RUN dotnet build "HotelManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HotelManagement.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HotelManagement.API.dll"]
```

构建和运行：
```bash
docker build -t hotel-management-api .
docker run -p 5000:80 hotel-management-api
```

## 🔐 安全提示

1. **修改JWT密钥**：在生产环境使用前，务必修改 `appsettings.json` 中的JWT密钥
2. **数据库密码**：如使用SQL Server，不要将密码提交到Git
3. **HTTPS证书**：生产环境配置正式SSL证书
4. **API限流**：考虑添加限流中间件防止滥用

## ❓ 常见问题

### 端口被占用
修改 `Properties/launchSettings.json` 中的端口号。

### 数据库连接失败
检查连接字符串是否正确，SQL Server服务是否启动。

### Swagger无法访问
确认项目在开发环境运行，检查防火墙设置。

## 📚 相关文档

- [README.md](README.md) - 项目总体说明
- [HARDWARE_INTEGRATION.md](HARDWARE_INTEGRATION.md) - 硬件集成指南
- [GITHUB_GUIDE.md](GITHUB_GUIDE.md) - GitHub上传指南

## 🆘 获取帮助

遇到问题？
1. 查看日志输出
2. 检查数据库是否正常创建
3. 参考 [ASP.NET Core文档](https://docs.microsoft.com/aspnet/core)
4. 查看项目Issues

祝开发顺利！🎉
