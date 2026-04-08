using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class LogBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Log> logBuilder)
        {
            logBuilder.ToTable("logs");

            logBuilder.HasKey(log => log.Id);

            logBuilder.Property(log => log.Id).HasColumnOrder(0);
            logBuilder.Property(log => log.UserId).HasColumnOrder(1);
            logBuilder.Property(log => log.ChannelId).HasColumnOrder(2);
            logBuilder.Property(log => log.EventType).HasMaxLength(16).HasColumnOrder(3);
            logBuilder.Property(log => log.IPAddress).HasMaxLength(16).HasColumnOrder(4);
            logBuilder.Property(log => log.UserAgent).HasMaxLength(16).HasColumnOrder(5);
            logBuilder.Property(log => log.MetaData).HasConversion(log => JsonSerializer.Serialize(log), log => JsonSerializer.Deserialize<MetaData>(log)).HasColumnType("jsonb").HasColumnOrder(6);
            logBuilder.Property(log => log.DateTimeUtc).HasColumnType("timestamp with time zone").HasColumnOrder(7);

            logBuilder.HasOne(log => log.User).WithMany(user => user.Logs).HasForeignKey(log => log.UserId).OnDelete(DeleteBehavior.Restrict);
            logBuilder.HasOne(log => log.Channel).WithMany(channel => channel.Logs).HasForeignKey(log => log.ChannelId).OnDelete(DeleteBehavior.Restrict);

            logBuilder.HasIndex(userHandle => userHandle.EventType);
            logBuilder.HasIndex(userHandle => userHandle.DateTimeUtc);
        }
    }
}
