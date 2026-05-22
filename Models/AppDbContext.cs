using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SchoolApp.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Trainee> Trainees { get; set; }
        public DbSet<CrsResult> CrsResults { get; set; }
        public DbSet<CourseMaterial> CourseMaterials { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<MaterialProgress> MaterialProgresses { get; set; }
        public DbSet<Announcement> Announcements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure relationships
            modelBuilder.Entity<Instructor>()
                .HasOne(i => i.Department)
                .WithMany(d => d.Instructors)
                .HasForeignKey(i => i.Dept_id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Instructor>()
                .HasOne(i => i.Course)
                .WithMany(c => c.Instructors)
                .HasForeignKey(i => i.Crs_id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.Dept_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Trainee>()
                .HasOne(t => t.Department)
                .WithMany(d => d.Trainees)
                .HasForeignKey(t => t.Dept_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CrsResult>()
                .HasOne(cr => cr.Course)
                .WithMany(c => c.CrsResults)
                .HasForeignKey(cr => cr.Crs_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CrsResult>()
                .HasOne(cr => cr.Trainee)
                .WithMany(t => t.CrsResults)
                .HasForeignKey(cr => cr.Trainee_id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CourseMaterial>()
                .HasOne(cm => cm.Course)
                .WithMany(c => c.CourseMaterials)
                .HasForeignKey(cm => cm.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Trainee)
                .WithMany(t => t.Attendances)
                .HasForeignKey(a => a.TraineeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Trainee)
                .WithMany()
                .HasForeignKey(e => e.TraineeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MaterialProgress>()
                .HasOne(mp => mp.Trainee)
                .WithMany()
                .HasForeignKey(mp => mp.TraineeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Seeding Data
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Computer Science", Manager = "Ahmed" },
                new Department { Id = 2, Name = "Information Systems", Manager = "Sara" }
            );

            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, Name = "C# Programming", Degree = 100, MinDegree = 50, Hrs = 60, Dept_id = 1 },
                new Course { Id = 2, Name = "Database Design", Degree = 100, MinDegree = 50, Hrs = 40, Dept_id = 2 }
            );

            modelBuilder.Entity<Instructor>().HasData(
                new Instructor { Id = 1, Name = "Mohamed Ali", Image = "mohamed.jpg", Salary = 5000, Address = "Cairo", Dept_id = 1, Crs_id = 1 },
                new Instructor { Id = 2, Name = "Mona Zaki", Image = "mona.jpg", Salary = 6000, Address = "Alexandria", Dept_id = 2, Crs_id = 2 }
            );

            modelBuilder.Entity<Trainee>().HasData(
                new Trainee { Id = 1, Name = "Ali Hassan", Image = "ali.jpg", Address = "Giza", Grade = 85, Dept_id = 1 },
                new Trainee { Id = 2, Name = "Noha Fathy", Image = "noha.jpg", Address = "Cairo", Grade = 92, Dept_id = 2 }
            );

            modelBuilder.Entity<CrsResult>().HasData(
                new CrsResult { Id = 1, Degree = 85, Crs_id = 1, Trainee_id = 1 },
                new CrsResult { Id = 2, Degree = 92, Crs_id = 2, Trainee_id = 2 }
            );
        }
    }
}
