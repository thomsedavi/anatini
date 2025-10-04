using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Extensions
{
    public static class UserEventBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<UserEvent> userEventBuilder)
        {
            userEventBuilder.ToContainer("UserEvents");
            userEventBuilder.HasKey(userEvent => userEvent.Id);
            userEventBuilder.HasPartitionKey(userEvent => new { userEvent.UserId, userEvent.EventType });
            userEventBuilder.Property(userEvent => userEvent.Id).ToJsonProperty("id");
            userEventBuilder.Property(userEvent => userEvent.EventType).ToJsonProperty("eventType");
            userEventBuilder.Property(userEvent => userEvent.UserId).ToJsonProperty("userId");
            userEventBuilder.Property(userEvent => userEvent.DateTimeUtc).ToJsonProperty("dateTimeUtc");
            userEventBuilder.Property(userEvent => userEvent.Data).ToJsonProperty("data");
        }
    }
}
