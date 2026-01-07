using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelManagement.API.Data;
using HotelManagement.API.Models;
using HotelManagement.API.DTOs;

namespace HotelManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly HotelDbContext _context;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(HotelDbContext context, ILogger<BookingsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有订单
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Booking>>> GetBookings(
        [FromQuery] string? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var query = _context.Bookings.Include(b => b.Room).AsQueryable();
        
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(b => b.Status == status);
        }
        
        if (startDate.HasValue)
        {
            query = query.Where(b => b.CheckInTime >= startDate.Value);
        }
        
        if (endDate.HasValue)
        {
            query = query.Where(b => b.CheckOutTime <= endDate.Value);
        }
        
        var bookings = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
        return Ok(bookings);
    }

    /// <summary>
    /// 根据ID获取订单
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Booking>> GetBooking(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.TemporaryPasswords)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            return NotFound(new { message = "订单不存在" });
        }

        return Ok(booking);
    }

    /// <summary>
    /// 创建新订单
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Booking>> CreateBooking(CreateBookingDto bookingDto)
    {
        // 检查房间是否可用
        var room = await _context.Rooms.FindAsync(bookingDto.RoomId);
        if (room == null)
        {
            return NotFound(new { message = "房间不存在" });
        }

        if (room.Status != "Available")
        {
            return BadRequest(new { message = "房间不可用" });
        }

        // 检查时间段是否有冲突（排除已取消和已退房的订单）
        var hasConflict = await _context.Bookings
            .AnyAsync(b => b.RoomId == bookingDto.RoomId 
                && b.Status != "Cancelled"
                && b.Status != "CheckedOut"
                && b.CheckInTime < bookingDto.CheckOutTime 
                && b.CheckOutTime > bookingDto.CheckInTime);

        if (hasConflict)
        {
            return BadRequest(new { message = "该时间段房间已被预订" });
        }

        var booking = new Booking
        {
            RoomId = bookingDto.RoomId,
            GuestName = bookingDto.GuestName,
            GuestPhone = bookingDto.GuestPhone,
            GuestIdCard = bookingDto.GuestIdCard,
            CheckInTime = bookingDto.CheckInTime,
            CheckOutTime = bookingDto.CheckOutTime,
            TotalPrice = bookingDto.TotalPrice,
            PaidAmount = bookingDto.PaidAmount,
            Notes = bookingDto.Notes,
            CreatedAt = DateTime.UtcNow,
            Status = "Pending"
        };
        
        _context.Bookings.Add(booking);
        
        // 更新房间状态
        room.Status = "Reserved";
        
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
    }

    /// <summary>
    /// 办理入住
    /// </summary>
    [HttpPost("{id}/checkin")]
    public async Task<ActionResult> CheckIn(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Room)
            .ThenInclude(r => r.DoorLock)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            return NotFound(new { message = "订单不存在" });
        }

        if (booking.Status != "Pending" && booking.Status != "Confirmed")
        {
            return BadRequest(new { message = "订单状态不允许办理入住" });
        }

        booking.Status = "CheckedIn";
        booking.ActualCheckInTime = DateTime.UtcNow;
        booking.Room.Status = "Occupied";

        // 生成临时密码
        if (booking.Room.DoorLock != null)
        {
            var password = GenerateTemporaryPassword();
            var tempPassword = new TemporaryPassword
            {
                BookingId = booking.Id,
                DoorLockId = booking.Room.DoorLock.Id,
                Password = password,
                ValidFrom = DateTime.UtcNow,
                ValidUntil = booking.CheckOutTime.AddHours(2), // 退房后2小时失效
                CreatedAt = DateTime.UtcNow
            };

            _context.TemporaryPasswords.Add(tempPassword);
            
            _logger.LogInformation($"为订单 {id} 生成临时密码: {password}");
        }

        await _context.SaveChangesAsync();

        return Ok(new 
        { 
            message = "入住成功", 
            booking = booking,
            password = booking.Room.DoorLock != null ? "已发送至手机" : null
        });
    }

    /// <summary>
    /// 办理退房
    /// </summary>
    [HttpPost("{id}/checkout")]
    public async Task<ActionResult> CheckOut(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            return NotFound(new { message = "订单不存在" });
        }

        if (booking.Status != "CheckedIn")
        {
            return BadRequest(new { message = "订单状态不允许办理退房" });
        }

        booking.Status = "CheckedOut";
        booking.ActualCheckOutTime = DateTime.UtcNow;
        booking.Room.Status = "Available";

        await _context.SaveChangesAsync();

        return Ok(new { message = "退房成功" });
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> CancelBooking(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            return NotFound(new { message = "订单不存在" });
        }

        if (booking.Status == "CheckedIn" || booking.Status == "CheckedOut")
        {
            return BadRequest(new { message = "已入住或已退房的订单无法取消" });
        }

        booking.Status = "Cancelled";
        booking.Room.Status = "Available";

        await _context.SaveChangesAsync();

        return Ok(new { message = "订单已取消" });
    }

    /// <summary>
    /// 生成6位数字临时密码
    /// </summary>
    private string GenerateTemporaryPassword()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}
