namespace HotelManagement.API.Models;

/// <summary>
/// 订单/预订实体
/// </summary>
public class Booking
{
    public int Id { get; set; }
    
    /// <summary>
    /// 房间ID
    /// </summary>
    public int RoomId { get; set; }
    
    /// <summary>
    /// 关联的房间
    /// </summary>
    public Room Room { get; set; } = null!;
    
    /// <summary>
    /// 客人姓名
    /// </summary>
    public string GuestName { get; set; } = string.Empty;
    
    /// <summary>
    /// 客人电话
    /// </summary>
    public string GuestPhone { get; set; } = string.Empty;
    
    /// <summary>
    /// 客人身份证号
    /// </summary>
    public string? GuestIdCard { get; set; }
    
    /// <summary>
    /// 预计入住时间
    /// </summary>
    public DateTime CheckInTime { get; set; }
    
    /// <summary>
    /// 预计退房时间
    /// </summary>
    public DateTime CheckOutTime { get; set; }
    
    /// <summary>
    /// 实际入住时间
    /// </summary>
    public DateTime? ActualCheckInTime { get; set; }
    
    /// <summary>
    /// 实际退房时间
    /// </summary>
    public DateTime? ActualCheckOutTime { get; set; }
    
    /// <summary>
    /// 订单状态：Pending, Confirmed, CheckedIn, CheckedOut, Cancelled
    /// </summary>
    public string Status { get; set; } = "Pending";
    
    /// <summary>
    /// 总价
    /// </summary>
    public decimal TotalPrice { get; set; }
    
    /// <summary>
    /// 已支付金额
    /// </summary>
    public decimal PaidAmount { get; set; }
    
    /// <summary>
    /// 备注
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 该订单的临时密码
    /// </summary>
    public ICollection<TemporaryPassword> TemporaryPasswords { get; set; } = new List<TemporaryPassword>();
}
