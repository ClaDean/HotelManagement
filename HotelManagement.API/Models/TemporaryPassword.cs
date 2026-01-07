namespace HotelManagement.API.Models;

/// <summary>
/// 临时密码实体
/// </summary>
public class TemporaryPassword
{
    public int Id { get; set; }
    
    /// <summary>
    /// 订单ID
    /// </summary>
    public int BookingId { get; set; }
    
    /// <summary>
    /// 关联的订单
    /// </summary>
    public Booking Booking { get; set; } = null!;
    
    /// <summary>
    /// 门锁ID
    /// </summary>
    public int DoorLockId { get; set; }
    
    /// <summary>
    /// 关联的门锁
    /// </summary>
    public DoorLock DoorLock { get; set; } = null!;
    
    /// <summary>
    /// 密码（通常是6-8位数字）
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// 有效开始时间
    /// </summary>
    public DateTime ValidFrom { get; set; }
    
    /// <summary>
    /// 有效结束时间
    /// </summary>
    public DateTime ValidUntil { get; set; }
    
    /// <summary>
    /// 是否已使用
    /// </summary>
    public bool IsUsed { get; set; } = false;
    
    /// <summary>
    /// 首次使用时间
    /// </summary>
    public DateTime? FirstUsedAt { get; set; }
    
    /// <summary>
    /// 使用次数
    /// </summary>
    public int UsageCount { get; set; } = 0;
    
    /// <summary>
    /// 是否已发送给客人
    /// </summary>
    public bool IsSent { get; set; } = false;
    
    /// <summary>
    /// 发送时间
    /// </summary>
    public DateTime? SentAt { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
