using System.Linq;
using EventTick.Model.Enum;
using EventTick.Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using projectDemo.Data.InitDB;
using projectDemo.Data.Seed;
using projectDemo.Entity.Models;

namespace projectDemo.Data
{
    public class EventTickDbContext : DbContext
    {
        public EventTickDbContext(DbContextOptions<EventTickDbContext> options)
            : base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<UserLogin> UserLogin { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<TicketType> TicketType { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<ApiLog> apiLogs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Catetory> Catetory { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }
        public DbSet<Upgrade> Upgrade { get; set; }
        public DbSet<UserUpgrade> UserUpgrades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserUpgrade>()
                .HasOne(uu => uu.User)
                .WithMany(u => u.UserUpgrades)
                .HasForeignKey(uu => uu.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserUpgrade>()
                .HasOne(uu => uu.Upgrade)
                .WithMany(u => u.UserUpgrades)
                .HasForeignKey(uu => uu.UpgradeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder
                .Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(u => u.UserId);

            modelBuilder
                .Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRole)
                .HasForeignKey(r => r.RoleId);

            modelBuilder
                .Entity<User>()
                .HasMany(u => u.UserLogins)
                .WithOne(ul => ul.user)
                .HasForeignKey(ul => ul.UserId);

            modelBuilder
                .Entity<User>()
                .HasMany(u => u.Events)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserID);

            modelBuilder
                .Entity<Event>()
                .HasMany(e => e.TicketTypes)
                .WithOne(tt => tt.Event)
                .HasForeignKey(tt => tt.EventID);

            modelBuilder
                .Entity<TicketType>()
                .HasMany(tt => tt.OrderDetails)
                .WithOne(od => od.TicketTypes)
                .HasForeignKey(od => od.TicketTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderID);

            modelBuilder
                .Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserID);

            modelBuilder
                .Entity<OrderDetail>()
                .HasMany(o => o.Ticket)
                .WithOne(t => t.OrderDetail)
                .HasForeignKey(t => t.OrderDetailID);

            modelBuilder
                .Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Orders)
                .HasForeignKey<Payment>(p => p.OrderID);

            modelBuilder.Entity<Order>()
                .Property(o => o.UserUpgradeId)
                .IsRequired(false);

            modelBuilder
                .Entity<User>()
                .HasMany(u => u.ApiLogs)
                .WithOne(al => al.User)
                .HasForeignKey(al => al.UserId);

            modelBuilder
                .Entity<Role>()
                .HasMany(r => r.RolePermissions)
                .WithOne(rp => rp.Role)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermissions>().HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder
                .Entity<Permissions>()
                .HasMany(r => r.RolePermissions)
                .WithOne(rp => rp.Permissions)
                .HasForeignKey(rp => rp.PermissionId);

            modelBuilder
                .Entity<Catetory>()
                .HasMany(c => c.Events)
                .WithOne(e => e.Catetory)
                .HasForeignKey(c => c.CatetoryID);

            modelBuilder.Entity<Role>().HasData(GetRoleSeed.GetRole().ToArray());
            modelBuilder.Entity<Permissions>().HasData(GetPermissionSeed.GetPermission().ToArray());

            var adminId = SeedConstants.AdminUserId;

            modelBuilder
                .Entity<User>()
                .HasData(
                    new User
                    {
                        Id = adminId,
                        Username = "admin",
                        FirstName = "admin",
                        LastName = "admin",
                        Email = "admin@system.com",
                        IsActive = true,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    }
                );
           

            modelBuilder
                .Entity<UserRole>()
                .HasData(
                    new UserRole
                    {
                        Id = 1,
                        UserId = adminId,
                        RoleId = 1,
                    }
                );

            var permissions = GetPermissionSeed.GetPermission();
            var rolePermissions = permissions
                .Select(
                    (p, index) =>
                        new RolePermissions
                        {
                            Id = index + 1,
                            RoleId = 1,
                            PermissionId = p.Id,
                        }
                )
                .ToList();

            modelBuilder.Entity<RolePermissions>().HasData(rolePermissions);

            modelBuilder
                .Entity<Catetory>()
                .HasData(
                    new Catetory
                    {
                        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Name = "Âm Nhạc",
                    },
                    new Catetory
                    {
                        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        Name = "Thể Thao",
                    },
                    new Catetory
                    {
                        Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                        Name = "Trí Tuệ",
                    }
                );
        }
    }
}
