using Microsoft.EntityFrameworkCore;
using MemberRewardsApi.Models;

namespace MemberRewardsApi.Data
{
    public class MemberRewardsDbContext : DbContext
    {
        public MemberRewardsDbContext(DbContextOptions<MemberRewardsDbContext> options) : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<MemberPoint> MemberPoints { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponRedemption> CouponRedemptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Member entity
            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.MobileNumber).IsUnique();
                entity.Property(e => e.MobileNumber).IsRequired().HasMaxLength(15);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property(e => e.Otp).IsRequired().HasMaxLength(6);
            });

            // Configure MemberPoint entity
            modelBuilder.Entity<MemberPoint>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PurchaseAmount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasOne(e => e.Member)
                      .WithMany(m => m.MemberPoints)
                      .HasForeignKey(e => e.MemberId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Coupon entity
            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CouponValue).HasColumnType("decimal(10,2)");
            });

            // Configure CouponRedemption entity
            modelBuilder.Entity<CouponRedemption>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CouponCode).HasMaxLength(50);
                entity.HasOne(e => e.Member)
                      .WithMany(m => m.CouponRedemptions)
                      .HasForeignKey(e => e.MemberId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Coupon)
                      .WithMany(c => c.CouponRedemptions)
                      .HasForeignKey(e => e.CouponId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed some default coupons
            modelBuilder.Entity<Coupon>().HasData(
                new Coupon
                {
                    Id = 1,
                    Name = "₹50 Off Coupon",
                    Description = "Get ₹50 off on your next purchase",
                    PointsRequired = 500,
                    CouponValue = 50,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Coupon
                {
                    Id = 2,
                    Name = "₹100 Off Coupon",
                    Description = "Get ₹100 off on your next purchase",
                    PointsRequired = 1000,
                    CouponValue = 100,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}