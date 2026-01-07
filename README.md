# 🏨 酒店智能门锁管理系统

<div align="center">

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![SQLite](https://img.shields.io/badge/SQLite-07405E?style=flat-square&logo=sqlite)
![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)

**一个为民宿/酒店设计的智能门锁管理系统，支持远程开锁、临时密码生成、入住管理等功能**

[快速开始](#-快速开始) • [功能特性](#-功能特性) • [API文档](#-api接口) • [硬件集成](#-硬件集成) • [部署指南](#-部署)

</div>

---

## 📋 目录

- [项目简介](#-项目简介)
- [核心功能](#-核心功能)
- [技术栈](#️-技术栈)
- [项目结构](#-项目结构)
- [快速开始](#-快速开始)
- [API接口](#-api接口)
- [数据库设计](#️-数据库设计)
- [硬件集成](#-硬件集成)
- [可扩展功能](#-可扩展功能)
- [常见问题](#-常见问题)
- [贡献指南](#-贡献指南)

## 🎯 项目简介

这是一个专为民宿和小型酒店设计的智能门锁管理系统，解决传统酒店人工发放钥匙的痛点。通过集成智能门锁硬件，实现：

- 🔐 **无钥匙入住**：客人到店后自动生成临时密码
- 📱 **远程管理**：管理员可远程开锁，无需现场
- 📊 **数据追踪**：完整的开锁记录，提升安全性
- ⚡ **自动化流程**：入住/退房时密码自动生效/失效
- 🔌 **硬件对接**：支持主流智能门锁（TTLock、涂鸦、小米等）

## ✨ 核心功能

### 1️⃣ 房间管理
- ✅ 房间信息管理（房号、类型、价格、楼层）
- ✅ 房间状态追踪（空闲/已预订/已入住/维修中）
- ✅ 门锁设备绑定

### 2️⃣ 订单管理
- ✅ 在线预订创建
- ✅ 入住办理（自动生成临时密码）
- ✅ 退房处理（密码自动失效）
- ✅ 订单取消与退款

### 3️⃣ 智能门锁控制
- ✅ 门锁设备注册与管理
- ✅ 远程开锁指令下发
- ✅ 临时密码生成（6位数字，可自定义有效期）
- ✅ 设备状态监控（在线/离线、电量）
- ✅ 开锁记录查询

### 4️⃣ 安全与日志
- ✅ 完整的开锁日志记录
- ✅ JWT身份认证
- ✅ 角色权限控制
- ✅ 操作审计追踪

## 🛠️ 技术栈

### 后端
- **框架**: ASP.NET Core 9.0 Web API
- **ORM**: Entity Framework Core 9.0
- **数据库**: SQLite（开发）/ SQL Server（生产）
- **认证**: JWT Bearer Token
- **文档**: Swagger/OpenAPI

### 通信协议
- **MQTT**: 物联网设备通信（MQTTnet 5.0）
- **HTTP/HTTPS**: RESTful API
- **WebSocket**: 实时通知（可选）

### 开发工具
- **.NET SDK**: 9.0+
- **IDE**: Visual Studio 2022 / VS Code
- **测试工具**: Postman / Swagger UI

## 📁 项目结构

```
HotelManagement/
├── 📄 README.md                           # 项目说明文档
├── 📄 QUICKSTART.md                       # 快速启动指南
├── 📄 HARDWARE_INTEGRATION.md             # 硬件集成详细文档
├── 📄 GITHUB_GUIDE.md                     # GitHub使用指南
├── 📄 .gitignore                          # Git忽略配置
│
└── 📁 HotelManagement.API/                # 后端API项目
    │
    ├── 📁 Controllers/                    # API控制器
    │   ├── RoomsController.cs            # 房间管理API
    │   ├── DoorLocksController.cs        # 门锁控制API
    │   └── BookingsController.cs         # 订单管理API
    │
    ├── 📁 Models/                         # 数据模型
    │   ├── User.cs                       # 用户实体
    │   ├── Room.cs                       # 房间实体
    │   ├── DoorLock.cs                   # 门锁实体
    │   ├── Booking.cs                    # 订单实体
    │   ├── UnlockRecord.cs               # 开锁记录
    │   └── TemporaryPassword.cs          # 临时密码
    │
    ├── 📁 Data/                           # 数据访问层
    │   └── HotelDbContext.cs             # EF Core上下文
    │
    ├── 📄 Program.cs                      # 程序入口
    ├── 📄 appsettings.json               # 配置文件
    └── 📄 HotelManagement.API.csproj     # 项目文件
```

## 🚀 快速开始

### 前提条件

确保已安装以下环境：
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Git](https://git-scm.com/)
- 代码编辑器（推荐 [VS Code](https://code.visualstudio.com/) 或 [Visual Studio 2022](https://visualstudio.microsoft.com/)）

### 安装步骤

#### 1. 克隆项目

```bash
git clone https://github.com/ClaDean/HotelManagement.git
cd HotelManagement
```

#### 2. 进入API项目目录

```bash
cd HotelManagement.API
```

#### 3. 恢复NuGet包

```bash
dotnet restore
```

#### 4. 运行项目

```bash
dotnet run
```

项目启动后，你会看到：

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

#### 5. 访问Swagger文档

在浏览器打开：
- **HTTPS**: https://localhost:7001/swagger
- **HTTP**: http://localhost:5000/swagger

### 开发模式（热重载）

支持代码修改后自动重新编译：

```bash
dotnet watch run
```

## 📡 API接口

完整的API文档可通过Swagger访问，以下是核心接口：

### 房间管理

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/rooms` | 获取所有房间 |
| GET | `/api/rooms/{id}` | 获取指定房间 |
| POST | `/api/rooms` | 创建房间 |
| PUT | `/api/rooms/{id}` | 更新房间信息 |
| DELETE | `/api/rooms/{id}` | 删除房间 |
| PATCH | `/api/rooms/{id}/status` | 更新房间状态 |

### 门锁管理

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/doorlocks` | 获取所有门锁 |
| GET | `/api/doorlocks/{id}` | 获取指定门锁 |
| POST | `/api/doorlocks` | 注册新门锁 |
| POST | `/api/doorlocks/{id}/unlock` | **远程开锁** ⭐ |
| GET | `/api/doorlocks/{id}/status` | 查询门锁状态 |
| POST | `/api/doorlocks/{id}/heartbeat` | 更新设备心跳 |
| GET | `/api/doorlocks/{id}/records` | 查看开锁记录 |

### 订单管理

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/bookings` | 获取订单列表 |
| GET | `/api/bookings/{id}` | 获取订单详情 |
| POST | `/api/bookings` | 创建新订单 |
| POST | `/api/bookings/{id}/checkin` | **办理入住** ⭐ |
| POST | `/api/bookings/{id}/checkout` | **办理退房** ⭐ |
| POST | `/api/bookings/{id}/cancel` | 取消订单 |

### 使用示例

#### 创建房间

```bash
POST https://localhost:7001/api/rooms
Content-Type: application/json

{
  "roomNumber": "101",
  "roomType": "标准间",
  "floor": 1,
  "status": "Available",
  "price": 288.00,
  "description": "舒适温馨的标准间"
}
```

#### 办理入住（自动生成密码）

```bash
POST https://localhost:7001/api/bookings/1/checkin
```

系统会自动：
1. 生成6位临时密码
2. 设置密码有效期
3. 更新订单和房间状态
4. 返回密码信息

#### 远程开锁

```bash
POST https://localhost:7001/api/doorlocks/1/unlock
Content-Type: application/json

{
  "roomId": 1,
  "userId": 1
}
```

## 🗄️ 数据库设计

### 核心数据表

#### Users - 用户表
```sql
- Id (PK)              主键
- Username             用户名（唯一）
- PasswordHash         密码哈希
- Role                 角色（Admin/Staff/Guest）
- PhoneNumber          手机号
- Email                邮箱
- IsActive             是否启用
- CreatedAt            创建时间
```

#### Rooms - 房间表
```sql
- Id (PK)              主键
- RoomNumber           房间号（唯一）
- RoomType             房间类型
- Floor                楼层
- Status               状态（Available/Occupied/Maintenance/Reserved）
- Price                价格
- Description          描述
- DoorLockId (FK)      关联门锁ID
```

#### DoorLocks - 门锁设备表
```sql
- Id (PK)              主键
- DeviceId             设备唯一标识（唯一）
- DeviceName           设备名称
- Manufacturer         制造商
- Model                型号
- Status               状态（Online/Offline/Fault）
- BatteryLevel         电池电量
- LastHeartbeat        最后心跳时间
- CreatedAt            注册时间
```

#### Bookings - 订单表
```sql
- Id (PK)              主键
- RoomId (FK)          房间ID
- GuestName            客人姓名
- GuestPhone           客人电话
- GuestIdCard          身份证号
- CheckInTime          预计入住时间
- CheckOutTime         预计退房时间
- ActualCheckInTime    实际入住时间
- ActualCheckOutTime   实际退房时间
- Status               状态（Pending/Confirmed/CheckedIn/CheckedOut/Cancelled）
- TotalPrice           总价
- PaidAmount           已支付
- CreatedAt            创建时间
```

#### TemporaryPasswords - 临时密码表
```sql
- Id (PK)              主键
- BookingId (FK)       订单ID
- DoorLockId (FK)      门锁ID
- Password             密码
- ValidFrom            有效开始时间
- ValidUntil           有效结束时间
- IsUsed               是否已使用
- UsageCount           使用次数
- CreatedAt            创建时间
```

#### UnlockRecords - 开锁记录表
```sql
- Id (PK)              主键
- DoorLockId (FK)      门锁ID
- RoomId               房间ID
- UnlockMethod         开锁方式（Password/Card/Remote/App）
- UnlockTime           开锁时间
- UserId               操作用户
- Success              是否成功
- FailureReason        失败原因
```

### 数据库配置

**SQLite（开发环境，默认）**:
```json
"ConnectionStrings": {
  "SqliteConnection": "Data Source=hotel.db"
}
```

**SQL Server（生产环境）**:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HotelManagementDb;Trusted_Connection=true"
}
```

## 🔌 硬件集成

系统支持主流智能门锁厂商，通过标准化接口实现硬件对接。

### 支持的门锁类型

#### 1. TTLock（科技侠）⭐ 推荐
- **优势**: API文档完善，国内市场占有率高
- **价格**: 单锁约300-800元
- **协议**: HTTP API + MQTT
- **文档**: https://open.ttlock.com/doc

#### 2. 涂鸦智能（Tuya）
- **优势**: 全球化平台，设备种类丰富
- **价格**: 单锁约400-1000元
- **协议**: HTTP API
- **文档**: https://developer.tuya.com/

#### 3. 小米智能门锁
- **优势**: 质量稳定，米家生态
- **价格**: 单锁约600-1500元
- **协议**: 米家云平台API
- **文档**: https://iot.mi.com/

### 集成方式

系统提供两种主流集成方式：

#### MQTT协议（推荐）
- ✅ 实时性好
- ✅ 双向通信
- ✅ 低带宽消耗
- ✅ 断线自动重连

#### HTTP API
- ✅ 简单易用
- ✅ 无需额外中间件
- ✅ 标准RESTful接口

详细集成文档请参考：[HARDWARE_INTEGRATION.md](HARDWARE_INTEGRATION.md)

### 典型业务流程

```
1. 客人预订 → 创建订单
2. 支付确认 → 订单状态：已确认
3. 到店入住 → 办理入住，生成临时密码
4. 密码下发 → 通过短信/微信发送给客人
5. 客人开门 → 使用密码开锁，记录日志
6. 退房离店 → 密码自动失效
```

## 🎨 可扩展功能

基于当前架构，以下功能可快速扩展：

### 🔐 用户认证模块
- [ ] 用户注册/登录API
- [ ] JWT Token生成与刷新
- [ ] 基于角色的访问控制（RBAC）
- [ ] 密码加密（BCrypt）
- [ ] 多因素认证（MFA）

### 📧 通知系统
- [ ] 短信发送（阿里云/腾讯云）
- [ ] 邮件通知（SMTP）
- [ ] 微信公众号消息推送
- [ ] 微信小程序订阅消息
- [ ] 系统内消息中心

### 💳 支付集成
- [ ] 支付宝支付
- [ ] 微信支付
- [ ] 银行卡支付
- [ ] 订单退款处理
- [ ] 财务对账

### 📊 数据统计与报表
- [ ] 入住率统计
- [ ] 收入报表（日/月/年）
- [ ] 门锁使用分析
- [ ] 客户行为分析
- [ ] 数据可视化大屏

### 🖥️ 前端界面
- [ ] 管理后台（React/Vue）
  - 房间管理界面
  - 订单管理界面
  - 门锁控制面板
  - 数据统计看板
- [ ] 客户端小程序
  - 在线预订
  - 自助入住
  - 电子钥匙
- [ ] 移动App（Flutter）

### 🤖 智能化功能
- [ ] 智能定价（淡旺季自动调价）
- [ ] 客户画像分析
- [ ] 入住时长预测
- [ ] 设备故障预警
- [ ] 清洁任务自动分配

### 🌐 系统优化
- [ ] Redis缓存
- [ ] 消息队列（RabbitMQ）
- [ ] 日志中心（ELK）
- [ ] 性能监控（Application Insights）
- [ ] 微服务架构拆分

## 🔧 配置说明

### 修改数据库

编辑 `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "SqliteConnection": "Data Source=hotel.db",
    "DefaultConnection": "Server=YOUR_SERVER;Database=HotelManagementDb;..."
  }
}
```

修改 `Program.cs` 中的数据库提供程序：

```csharp
// SQLite
builder.Services.AddDbContext<HotelDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection")));

// 或 SQL Server
builder.Services.AddDbContext<HotelDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### JWT配置

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyForJWTTokenGeneration123456",
    "Issuer": "HotelManagementAPI",
    "Audience": "HotelManagementClient",
    "ExpireMinutes": 60
  }
}
```

⚠️ **生产环境必须修改JWT密钥！**

## 📚 相关文档

- 📖 [快速启动指南](QUICKSTART.md) - 详细的运行和测试步骤
- 🔌 [硬件集成文档](HARDWARE_INTEGRATION.md) - 智能门锁对接指南
- 📦 [GitHub使用指南](GITHUB_GUIDE.md) - Git和GitHub操作教程

## ❓ 常见问题

<details>
<summary><b>Q: 如何切换数据库？</b></summary>

A: 修改 `Program.cs` 中的数据库配置，并更新 `appsettings.json` 中的连接字符串。
</details>

<details>
<summary><b>Q: 支持哪些智能门锁？</b></summary>

A: 支持TTLock、涂鸦、小米等主流品牌，详见[硬件集成文档](HARDWARE_INTEGRATION.md)。
</details>

<details>
<summary><b>Q: 如何添加新的API接口？</b></summary>

A: 在 `Controllers` 文件夹创建新的Controller，继承 `ControllerBase` 即可。
</details>

<details>
<summary><b>Q: 生产环境部署建议？</b></summary>

A: 
- 使用SQL Server替代SQLite
- 配置HTTPS证书
- 修改JWT密钥
- 启用日志记录
- 配置反向代理（Nginx/IIS）
</details>

## 🤝 贡献指南

欢迎提交Issue和Pull Request！

1. Fork本项目
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 提交Pull Request

## 📄 开源协议

本项目基于 [MIT License](LICENSE) 开源协议。

## 👨‍💻 作者

**ClaDean**
- GitHub: [@ClaDean](https://github.com/ClaDean)
- Email: 3129908134@qq.com

## ⭐ Star历史

如果这个项目对你有帮助，请给个Star ⭐ 支持一下！

[![Star History Chart](https://api.star-history.com/svg?repos=ClaDean/HotelManagement&type=Date)](https://star-history.com/#ClaDean/HotelManagement&Date)

---

<div align="center">
Made with ❤️ by ClaDean
</div>
