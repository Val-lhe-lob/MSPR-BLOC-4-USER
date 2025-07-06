using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MSPR_BLOC_4_USER.Models;

public partial class UserDbContext : DbContext
{
    public UserDbContext()
    {
    }

    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {

            entity.ToTable("User");

            entity.HasKey(e => e.Id).HasName("PK__User__3214EC079DDEC034");

            entity.Property(e => e.AccountType).HasDefaultValue("user");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastModifiedAt).HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
