namespace HotelManagement.API.Models;

/// <summary>
/// 门锁设备实体
/// </summary>
public class DoorLock
{
    public int Id { get; set; }
    
    /// <summary>
    /// 设备唯一标识（如MAC地址或设备序列号）
    /// </summary>
    public string DeviceId { get; set; } = string.Empty;
    
    /// <summary>
    /// 关联的房间ID
    /// </summary>
    public int? RoomId { get; set; }
    
    /// <summary>
    /// 设备名称
    /// </summary>
    public string DeviceName { get; set; } = string.Empty;
    
    /// <summary>
    /// 制造商
    /// </summary>
    public string Manufacturer { get; set; } = string.Empty;
    
    /// <summary>
    /// 型号
    /// </summary>
    public string Model { get; set; } = string.Empty;
    
    /// <summary>
    /// 设备状态：Online, Offline, Fault
    /// </summary>
    public string Status { get; set; } = "Offline";
    
    /// <summary>
    /// 最后心跳时间
    /// </summary>
    public DateTime? LastHeartbeat { get; set; }
    
    /// <summary>
    /// 固件版本
    /// </summary>
    public string? FirmwareVersion { get; set; }
    
    /// <summary>
    /// 电池电量（0-100）
    /// </summary>
    public int? BatteryLevel { get; set; }
    
    /// <summary>
    /// 注册时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 该门锁的开锁记录
    /// </summary>
    public ICollection<UnlockRecord> UnlockRecords { get; set; } = new List<UnlockRecord>();
    
    /// <summary>
    /// 该门锁的临时密码
    /// </summary>
    public ICollection<TemporaryPassword> TemporaryPasswords { get; set; } = new List<TemporaryPassword>();
}
