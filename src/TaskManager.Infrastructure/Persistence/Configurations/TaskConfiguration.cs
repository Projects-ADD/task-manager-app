using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<TaskManager.Domain.Entities.Task>
{
    public void Configure(EntityTypeBuilder<TaskManager.Domain.Entities.Task> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();
        builder.Property(t => t.Title).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Description).IsRequired().HasMaxLength(500);
        builder.Property(t => t.Status).IsRequired();
        builder.Property(t => t.Priority).IsRequired();
        builder.Property(t => t.DueAt).IsRequired();
    }
}