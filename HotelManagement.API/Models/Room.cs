namespace HotelManagement.API.Models;

/// <summary>
/// 房间实体
/// </summary>
public class Room
{
    public int Id { get; set; }
    
    /// <summary>
    /// 房间号
    /// </summary>
    public string RoomNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// 房间类型（标准间、大床房、套房等）
    /// </summary>
    public string RoomType { get; set; } = string.Empty;
    
    /// <summary>
    /// 楼层
    /// </summary>
    public int Floor { get; set; }
    
    /// <summary>
    /// 房间状态：Available, Occupied, Maintenance, Reserved
    /// </summary>
    public string Status { get; set; } = "Available";
    
    /// <summary>
    /// 价格（每晚）
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// 房间描述
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// 关联的门锁ID
    /// </summary>
    public int? DoorLockId { get; set; }
    
    /// <summary>
    /// 关联的门锁
    /// </summary>
    public DoorLock? DoorLock { get; set; }
    
    /// <summary>
    /// 该房间的预订记录
    /// </summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
