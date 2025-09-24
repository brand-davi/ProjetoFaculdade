using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectManagerMvc.Models;

namespace ProjectManagerMvc.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<ProjectTeam> ProjectTeams => Set<ProjectTeam>();
        public DbSet<TeamUser> TeamUsers => Set<TeamUser>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProjectTeam>().HasKey(pt => new { pt.ProjectId, pt.TeamId });
            builder.Entity<ProjectTeam>()
                .HasOne(pt => pt.Project)
                .WithMany(p => p.ProjectTeams)
                .HasForeignKey(pt => pt.ProjectId);

            builder.Entity<ProjectTeam>()
                .HasOne(pt => pt.Team)
                .WithMany(t => t.ProjectTeams)
                .HasForeignKey(pt => pt.TeamId);

            builder.Entity<TeamUser>().HasKey(tu => new { tu.TeamId, tu.UserId });
            builder.Entity<TeamUser>()
                .HasOne(tu => tu.Team)
                .WithMany(t => t.TeamUsers)
                .HasForeignKey(tu => tu.TeamId);

            builder.Entity<TeamUser>()
                .HasOne(tu => tu.User)
                .WithMany(u => u.TeamUsers)
                .HasForeignKey(tu => tu.UserId);

            builder.Entity<Project>()
                .HasOne(p => p.Manager)
                .WithMany()
                .HasForeignKey(p => p.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TaskItem>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId);

            builder.Entity<TaskItem>()
                .HasOne(t => t.Assignee)
                .WithMany()
                .HasForeignKey(t => t.AssigneeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
