﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Onitama.Core;
using Onitama.Core.UserAggregate;

namespace Onitama.Infrastructure;

//DO NOT TOUCH THIS FILE!!
public class OnitamaDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public OnitamaDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable("Users");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("ExternalLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
    }

    public override int SaveChanges()
    {
        // Perform any additional logic before saving changes
        //ChangeTracker.DetectChanges();
        return base.SaveChanges(); // Save changes to the database
    }
}