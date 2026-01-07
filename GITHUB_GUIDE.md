# 上传到GitHub步骤指南

## 第一步：配置Git用户信息

在首次使用Git前，需要配置你的用户名和邮箱：

```bash
# 配置全局用户名（将"Your Name"替换为你的名字）
git config --global user.name "Your Name"

# 配置全局邮箱（将"your.email@example.com"替换为你的邮箱）
git config --global user.email "your.email@example.com"
```

## 第二步：提交代码到本地仓库

```bash
# 查看Git状态
git status

# 添加所有文件
git add .

# 提交
git commit -m "Initial commit: Hotel Management System with door lock control"
```

## 第三步：在GitHub创建新仓库

1. 打开浏览器访问 [GitHub](https://github.com)
2. 登录你的账号
3. 点击右上角的 "+" 按钮，选择 "New repository"
4. 填写仓库信息：
   - Repository name: `HotelManagement`
   - Description: `酒店门锁管理系统 - 支持智能门锁远程控制`
   - 选择 Public 或 Private
   - **不要**勾选 "Initialize this repository with a README"（我们已经有了）
5. 点击 "Create repository"

## 第四步：关联远程仓库并推送

GitHub会显示一些指令，你也可以直接使用以下命令：

```bash
# 关联远程仓库（将yourusername替换为你的GitHub用户名）
git remote add origin https://github.com/yourusername/HotelManagement.git

# 设置主分支为main
git branch -M main

# 推送代码到GitHub
git push -u origin main
```

### 使用SSH方式（推荐，无需每次输入密码）

如果你已配置SSH密钥：

```bash
# 使用SSH URL
git remote add origin git@github.com:yourusername/HotelManagement.git
git branch -M main
git push -u origin main
```

### 配置SSH密钥（首次使用）

```bash
# 生成SSH密钥
ssh-keygen -t ed25519 -C "your.email@example.com"

# 复制公钥内容
cat ~/.ssh/id_ed25519.pub

# 然后到GitHub Settings > SSH and GPG keys > New SSH key 添加
```

## 第五步：后续更新代码

之后每次修改代码后：

```bash
# 查看修改状态
git status

# 添加修改的文件
git add .

# 提交修改
git commit -m "描述你的修改内容"

# 推送到GitHub
git push
```

## 常用Git命令

```bash
# 查看提交历史
git log

# 查看简洁的提交历史
git log --oneline

# 查看远程仓库
git remote -v

# 拉取最新代码
git pull

# 创建新分支
git checkout -b feature/new-feature

# 切换分支
git checkout main

# 合并分支
git merge feature/new-feature

# 查看分支
git branch

# 删除分支
git branch -d feature/new-feature
```

## 忽略敏感信息

确保以下文件已被 `.gitignore` 忽略，不要提交到GitHub：

- ✅ `.gitignore` 已配置
- ❌ `appsettings.Development.json`（包含开发配置）
- ❌ `appsettings.Production.json`（包含生产配置）
- ❌ `*.db` 或 `*.sqlite`（数据库文件）
- ❌ `bin/` 和 `obj/`（编译输出）
- ❌ API密钥和密码

## 协作开发

如果是团队开发：

```bash
# 克隆项目
git clone https://github.com/yourusername/HotelManagement.git

# 进入项目目录
cd HotelManagement

# 创建功能分支
git checkout -b feature/your-feature-name

# 开发完成后提交
git add .
git commit -m "Add: your feature description"

# 推送分支
git push origin feature/your-feature-name

# 然后在GitHub上创建Pull Request
```

## 问题排查

### 推送失败

如果推送时提示权限错误：

```bash
# 检查远程仓库地址
git remote -v

# 修改远程仓库地址
git remote set-url origin https://github.com/yourusername/HotelManagement.git
```

### 文件太大

如果有文件超过100MB：

```bash
# 安装Git LFS
git lfs install

# 追踪大文件
git lfs track "*.db"
git lfs track "*.zip"

# 提交.gitattributes
git add .gitattributes
git commit -m "Configure Git LFS"
```

## 项目结构说明

```
HotelManagement/
├── .gitignore                      # Git忽略配置
├── README.md                       # 项目说明
├── HARDWARE_INTEGRATION.md         # 硬件集成文档
├── GITHUB_GUIDE.md                 # 本文件
└── HotelManagement.API/            # 后端API项目
    ├── Controllers/                # API控制器
    ├── Models/                     # 数据模型
    ├── Data/                       # 数据库上下文
    ├── appsettings.json           # 配置文件
    └── Program.cs                 # 程序入口
```

## 最佳实践

1. **频繁提交**：每完成一个小功能就提交一次
2. **清晰的提交信息**：使用有意义的commit message
3. **使用分支**：新功能在新分支开发，测试通过后合并
4. **定期拉取**：多人协作时经常执行 `git pull`
5. **代码审查**：使用Pull Request进行代码审查

## 示例工作流

```bash
# 1. 确保在最新代码上工作
git checkout main
git pull

# 2. 创建功能分支
git checkout -b feature/add-payment-module

# 3. 开发功能并提交
git add .
git commit -m "Add: payment module for bookings"

# 4. 推送分支
git push -u origin feature/add-payment-module

# 5. 在GitHub创建Pull Request

# 6. 代码审查通过后合并到main

# 7. 删除本地分支
git checkout main
git branch -d feature/add-payment-module
```

## 下一步

完成GitHub上传后，你可以：

1. 在README中添加项目徽章（build status, coverage等）
2. 设置GitHub Actions实现CI/CD
3. 配置Issue模板
4. 添加贡献指南（CONTRIBUTING.md）
5. 创建Wiki文档

祝你的项目开发顺利！🎉
