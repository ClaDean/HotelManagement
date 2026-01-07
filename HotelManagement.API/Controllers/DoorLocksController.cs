using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelManagement.API.Data;
using HotelManagement.API.Models;

namespace HotelManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoorLocksController : ControllerBase
{
    private readonly HotelDbContext _context;
    private readonly ILogger<DoorLocksController> _logger;

    public DoorLocksController(HotelDbContext context, ILogger<DoorLocksController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有门锁
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoorLock>>> GetDoorLocks()
    {
        var doorLocks = await _context.DoorLocks.ToListAsync();
        return Ok(doorLocks);
    }

    /// <summary>
    /// 根据ID获取门锁
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<DoorLock>> GetDoorLock(int id)
    {
        var doorLock = await _context.DoorLocks.FindAsync(id);

        if (doorLock == null)
        {
            return NotFound(new { message = "门锁不存在" });
        }

        return Ok(doorLock);
    }

    /// <summary>
    /// 注册新门锁
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<DoorLock>> RegisterDoorLock(DoorLock doorLock)
    {
        try
        {
            doorLock.CreatedAt = DateTime.UtcNow;
            doorLock.Status = "Offline";
            
            _context.DoorLocks.Add(doorLock);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDoorLock), new { id = doorLock.Id }, doorLock);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "注册门锁失败");
            return BadRequest(new { message = "注册门锁失败，设备ID可能已存在" });
        }
    }

    /// <summary>
    /// 远程开锁
    /// </summary>
    [HttpPost("{id}/unlock")]
    public async Task<IActionResult> UnlockDoor(int id, [FromBody] UnlockRequest request)
    {
        var doorLock = await _context.DoorLocks.FindAsync(id);
        if (doorLock == null)
        {
            return NotFound(new { message = "门锁不存在" });
        }

        if (doorLock.Status != "Online")
        {
            return BadRequest(new { message = "门锁离线，无法远程开锁" });
        }

        try
        {
            // 这里应该调用实际的硬件接口
            // await _doorLockService.SendUnlockCommand(doorLock.DeviceId);
            
            // 记录开锁日志
            var unlockRecord = new UnlockRecord
            {
                DoorLockId = id,
                RoomId = request.RoomId,
                UnlockMethod = "Remote",
                UnlockTime = DateTime.UtcNow,
                UserId = request.UserId,
                Success = true,
                AdditionalInfo = $"远程开锁by用户{request.UserId}"
            };
            
            _context.UnlockRecords.Add(unlockRecord);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"远程开锁成功: DoorLock={id}, User={request.UserId}");
            
            return Ok(new { message = "开锁指令已发送", success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "远程开锁失败");
            
            // 记录失败日志
            var unlockRecord = new UnlockRecord
            {
                DoorLockId = id,
                RoomId = request.RoomId,
                UnlockMethod = "Remote",
                UnlockTime = DateTime.UtcNow,
                UserId = request.UserId,
                Success = false,
                FailureReason = ex.Message
            };
            
            _context.UnlockRecords.Add(unlockRecord);
            await _context.SaveChangesAsync();
            
            return StatusCode(500, new { message = "开锁失败", error = ex.Message });
        }
    }

    /// <summary>
    /// 获取门锁状态
    /// </summary>
    [HttpGet("{id}/status")]
    public async Task<ActionResult> GetDoorLockStatus(int id)
    {
        var doorLock = await _context.DoorLocks.FindAsync(id);
        if (doorLock == null)
        {
            return NotFound(new { message = "门锁不存在" });
        }

        return Ok(new
        {
            deviceId = doorLock.DeviceId,
            status = doorLock.Status,
            batteryLevel = doorLock.BatteryLevel,
            lastHeartbeat = doorLock.LastHeartbeat,
            firmwareVersion = doorLock.FirmwareVersion
        });
    }

    /// <summary>
    /// 更新门锁心跳
    /// </summary>
    [HttpPost("{id}/heartbeat")]
    public async Task<IActionResult> UpdateHeartbeat(int id, [FromBody] HeartbeatData data)
    {
        var doorLock = await _context.DoorLocks.FindAsync(id);
        if (doorLock == null)
        {
            return NotFound(new { message = "门锁不存在" });
        }

        doorLock.LastHeartbeat = DateTime.UtcNow;
        doorLock.Status = "Online";
        doorLock.BatteryLevel = data.BatteryLevel;
        
        await _context.SaveChangesAsync();

        return Ok(new { message = "心跳更新成功" });
    }

    /// <summary>
    /// 获取开锁记录
    /// </summary>
    [HttpGet("{id}/records")]
    public async Task<ActionResult<IEnumerable<UnlockRecord>>> GetUnlockRecords(int id, [FromQuery] int limit = 50)
    {
        var records = await _context.UnlockRecords
            .Where(r => r.DoorLockId == id)
            .OrderByDescending(r => r.UnlockTime)
            .Take(limit)
            .ToListAsync();

        return Ok(records);
    }
}

/// <summary>
/// 开锁请求模型
/// </summary>
public class UnlockRequest
{
    public int RoomId { get; set; }
    public int UserId { get; set; }
}

/// <summary>
/// 心跳数据模型
/// </summary>
public class HeartbeatData
{
    public int BatteryLevel { get; set; }
    public string? FirmwareVersion { get; set; }
}
