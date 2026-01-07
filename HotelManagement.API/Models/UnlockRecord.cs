namespace HotelManagement.API.Models;

/// <summary>
/// 开锁记录实体
/// </summary>
public class UnlockRecord
{
    public int Id { get; set; }
    
    /// <summary>
    /// 门锁ID
    /// </summary>
    public int DoorLockId { get; set; }
    
    /// <summary>
    /// 关联的门锁
    /// </summary>
    public DoorLock DoorLock { get; set; } = null!;
    
    /// <summary>
    /// 房间ID
    /// </summary>
    public int RoomId { get; set; }
    
    /// <summary>
    /// 开锁方式：Password, Card, Remote, Fingerprint, App
    /// </summary>
    public string UnlockMethod { get; set; } = string.Empty;
    
    /// <summary>
    /// 开锁时间
    /// </summary>
    public DateTime UnlockTime { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 操作用户ID（如果是远程开锁）
    /// </summary>
    public int? UserId { get; set; }
    
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// 失败原因
    /// </summary>
    public string? FailureReason { get; set; }
    
    /// <summary>
    /// 额外信息（如使用的密码ID）
    /// </summary>
    public string? AdditionalInfo { get; set; }
}
