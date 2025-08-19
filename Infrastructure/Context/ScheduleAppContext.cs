using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Models;

namespace Infrastructure.Context;

public class ScheduleAppContext : DbContext
{
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<EnabledDate> EnabledDates { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<Slot> Slots { get; set; }   

    public ScheduleAppContext(DbContextOptions<ScheduleAppContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Appointment_Id_PK");

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("UserId");

            entity.Property(e => e.SlotId)
                .IsRequired()
                .HasColumnName("SlotId");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Appointments)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("Appointment_UserId_FK");

            entity.HasOne(e => e.Slot)
                .WithOne(s => s.Appointment)
                .HasForeignKey<Appointment>(s => s.SlotId)
                .HasConstraintName("Appointment_SlotId_FK");

            entity.Property(e => e.State)
                .IsRequired()
                .HasColumnType("varchar(100)")
                .HasConversion<string>() // Converts the ENUM value to a string in the DB
                .HasDefaultValue(AppointmentState.ACTIVE);

            // entity.HasIndex(e => new { e.UserId, e.ShiftId }) maybe
        });

        modelBuilder.Entity<EnabledDate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("EnabledDate_Id_PK");

            entity.Property(e => e.StartDate)
                .IsRequired()
                .HasColumnType("date")
                .HasColumnName("StartDate");

            entity.Property(e => e.EndDate)
                .IsRequired()
                .HasColumnType("date")
                .HasColumnName("EndDate");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Schedule_Id_PK");

            entity.Property(e => e.StartTime)
                .IsRequired()
                .HasColumnType("time(0)")
                .HasColumnName("StartTime");

            entity.Property(e => e.EndTime)
                .IsRequired()
                .HasColumnType("time(0)")
                .HasColumnName("EndTime");

            entity.Property(e => e.Description)
                .IsRequired()
                .HasColumnType("varchar(50)")
                .HasColumnName("Description");
            ;
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Shift_Id_PK");

            entity.Property(e => e.Date)
                .IsRequired()
                .HasColumnType("date")
                .HasColumnName("Date");

            entity.Property(e => e.ServicesSlots)
                .IsRequired()
                .HasColumnType("smallint")
                .HasColumnName("ServicesSlots");

            entity.Property(e => e.MeetingDurationOnMinutes)
                .IsRequired()
                .HasColumnType("tinyint")
                .HasColumnName("MeetingDurationOnMinutes");

            entity.HasOne(e => e.Schedule)
                .WithMany(e => e.Shifts)
                .HasForeignKey(e => e.ScheduleId)
                .HasConstraintName("Shift_ScheduleId_FK");
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Slot_Id_PK");
            
            entity.HasOne(s => s.Shift)
                .WithMany(s => s.Slots)
                .HasForeignKey(s => s.ShiftId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Slot_ShiftId_FK");
            
            
            entity.Property(s => s.StartTime)
                .IsRequired()
                .HasColumnType("time(0)")
                .HasColumnName("StartTime");
            
            entity.Property(s => s.EndTime)
                .IsRequired()
                .HasColumnType("time(0)")
                .HasColumnName("EndTime");
            
            modelBuilder.Entity<Slot>()
                .Property(s => s.isTaken)
                .HasDefaultValue(false); 
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_Id_PK");

            entity.Property(e => e.UserName)
                .IsRequired()
                .HasColumnType("varchar(100)")
                .HasColumnName("UserName");

            entity.Property(e => e.Password)
                .IsRequired()
                .HasColumnType("varchar(100)")
                .HasColumnName("Password");

            entity.Property(e => e.Email)
                .IsRequired()
                .HasColumnType("varchar(100)")
                .HasColumnName("Email");
            
            entity.Property(e => e.Role)
                .HasConversion<string>() // converts to string in the DB
                .HasColumnType("varchar(100)")
                .IsRequired();

            // We just made the Relation of Users - Appointments.           
            entity.HasIndex(e => e.UserName)
                .IsUnique();
            
            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.HasData(
                new User
                {
                    Id = 1,
                    UserName = "admin",
                    Password = "$2a$11$isXTfmHGobkbBdrnlICaMO1DXjxTtaWahqOgKsBDLejKWotlWiTF2",
                    Email = "test@gmail.com",
                    Role = Role.ADMIN
                }
                );

        });
    }
}