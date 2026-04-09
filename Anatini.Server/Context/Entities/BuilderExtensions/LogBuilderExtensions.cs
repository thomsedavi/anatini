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

            logBuilder.Property(log => log.Id).Has(order: 0);
            logBuilder.Property(log => log.UserId).Has(order: 1);
            logBuilder.Property(log => log.ChannelId).Has(order: 2);
            logBuilder.Property(log => log.EventType).Has(order: 3);
            logBuilder.Property(log => log.IPAddress)!.Has(maxLength: 16, order: 4);
            logBuilder.Property(log => log.UserAgent)!.Has(maxLength: 16, order: 5);
            logBuilder.Property(log => log.MetaData).HasColumnOrder(6).HasConversion();
            logBuilder.Property(log => log.DateTimeUtc).Has(order: 7);

            logBuilder.HasOneWithMany(log => log.User, user => user.Logs, log => log.UserId, DeleteBehavior.Restrict, required: false);
            logBuilder.HasOneWithMany(log => log.Channel, user => user.Logs, log => log.ChannelId, DeleteBehavior.Restrict, required: false);

            logBuilder.HasIndex(userHandle => userHandle.EventType);
            logBuilder.HasIndex(userHandle => userHandle.DateTimeUtc);
        }
    }
}
