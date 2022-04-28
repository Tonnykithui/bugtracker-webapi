using Core2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;

namespace DataStore.EF
{
    public class AppDbContext : IdentityDbContext<ApplicationsUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        //ObjectContent.ExecuteStoreCommand("SET IDENTITY_INSERT[dbo].[MyUser] ON");
        public DbSet<Project> Projects { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<ProjUser> ProjUsers { get; set; }

        public DbSet<TicketUser> TicketUser { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Ticket>()
                .Ignore(f => f.AssignedUserId);

            builder.Entity<Project>()
                .Ignore(f => f.AssignedUserId);

            builder.Entity<ProjUser>()
                .Ignore(f => f.ApplicationUserIds);

            builder.Entity<ApplicationsUser>()
                .Ignore(f => f.Role);

            builder.Entity<ProjUser>()
                .HasKey(pu => new { pu.ProjectId, pu.ApplicationUserId });

            builder.Entity<TicketUser>()
                .HasKey(tu => new { tu.TicketId, tu.UserId });

            builder.Entity<ProjUser>()
                .HasOne(pu => pu.Project)
                .WithMany(pu => pu.ProjUser)
                .HasForeignKey(pu => pu.ProjectId);

            builder.Entity<ProjUser>()
                .HasOne(pu => pu.ApplicationsUser)
                .WithMany(pu => pu.ProjUser)
                .HasForeignKey(pu => pu.ApplicationUserId);

            builder.Entity<TicketUser>()
                .HasOne(tu => tu.Ticket)
                .WithMany(tu => tu.TicketUser)
                .HasForeignKey(tu => tu.TicketId);

            builder.Entity<TicketUser>()
                .HasOne(tu => tu.ApplicationsUser)
                .WithMany(tu => tu.TicketUser)
                .HasForeignKey(tu => tu.UserId);

        }
    }
}
