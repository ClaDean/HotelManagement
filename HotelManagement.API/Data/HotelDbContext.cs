using Microsoft.EntityFrameworkCore;
using HotelManagement.API.Models;

namespace HotelManagement.API.Data;

/// <summary>
/// 酒店管理系统数据库上下文
/// </summary>
public class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<DoorLock> DoorLocks { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<UnlockRecord> UnlockRecords { get; set; }
    public DbSet<TemporaryPassword> TemporaryPasswords { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // User 配置
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(20);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.HasIndex(e => e.Username).IsUnique();
        });
        
        // Room 配置
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RoomNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.RoomType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.RoomNumber).IsUnique();
            
            entity.HasOne(e => e.DoorLock)
                .WithOne()
                .HasForeignKey<Room>(e => e.DoorLockId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // DoorLock 配置
        modelBuilder.Entity<DoorLock>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DeviceId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DeviceName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Manufacturer).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.DeviceId).IsUnique();
        });
        
        // Booking 配置
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.GuestName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.GuestPhone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.GuestIdCard).HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PaidAmount).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // UnlockRecord 配置
        modelBuilder.Entity<UnlockRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnlockMethod).IsRequired().HasMaxLength(50);
            
            entity.HasOne(e => e.DoorLock)
                .WithMany(d => d.UnlockRecords)
                .HasForeignKey(e => e.DoorLockId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // TemporaryPassword 配置
        modelBuilder.Entity<TemporaryPassword>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Password).IsRequired().HasMaxLength(20);
            
            entity.HasOne(e => e.Booking)
                .WithMany(b => b.TemporaryPasswords)
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.DoorLock)
                .WithMany(d => d.TemporaryPasswords)
                .HasForeignKey(e => e.DoorLockId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // 种子数据 - 创建默认管理员账户
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "$2a$11$YourHashedPasswordHere", // 实际使用时需要用BCrypt加密
                Role = "Admin",
                Email = "admin@hotel.com",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        );
    }
}
