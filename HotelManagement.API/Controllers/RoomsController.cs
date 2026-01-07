using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelManagement.API.Data;
using HotelManagement.API.Models;

namespace HotelManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly HotelDbContext _context;
    private readonly ILogger<RoomsController> _logger;

    public RoomsController(HotelDbContext context, ILogger<RoomsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有房间
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Room>>> GetRooms([FromQuery] string? status = null)
    {
        var query = _context.Rooms.Include(r => r.DoorLock).AsQueryable();
        
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(r => r.Status == status);
        }
        
        var rooms = await query.ToListAsync();
        return Ok(rooms);
    }

    /// <summary>
    /// 根据ID获取房间
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> GetRoom(int id)
    {
        var room = await _context.Rooms
            .Include(r => r.DoorLock)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (room == null)
        {
            return NotFound(new { message = "房间不存在" });
        }

        return Ok(room);
    }

    /// <summary>
    /// 创建新房间
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Room>> CreateRoom(Room room)
    {
        try
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "创建房间失败");
            return BadRequest(new { message = "创建房间失败，可能房间号已存在" });
        }
    }

    /// <summary>
    /// 更新房间信息
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRoom(int id, Room room)
    {
        if (id != room.Id)
        {
            return BadRequest(new { message = "ID不匹配" });
        }

        _context.Entry(room).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await RoomExists(id))
            {
                return NotFound(new { message = "房间不存在" });
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// 删除房间
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
        {
            return NotFound(new { message = "房间不存在" });
        }

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// 更新房间状态
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateRoomStatus(int id, [FromBody] string status)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
        {
            return NotFound(new { message = "房间不存在" });
        }

        room.Status = status;
        await _context.SaveChangesAsync();

        return Ok(new { message = "状态更新成功", status = room.Status });
    }

    private async Task<bool> RoomExists(int id)
    {
        return await _context.Rooms.AnyAsync(e => e.Id == id);
    }
}
