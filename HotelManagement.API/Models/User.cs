namespace HotelManagement.API.Models;

/// <summary>
/// 用户实体
/// </summary>
public class User
{
    public int Id { get; set; }
    
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// 密码哈希
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// 角色：Admin, Staff, Guest
    /// </summary>
    public string Role { get; set; } = "Guest";
    
    /// <summary>
    /// 手机号
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; set; } = true;
}
