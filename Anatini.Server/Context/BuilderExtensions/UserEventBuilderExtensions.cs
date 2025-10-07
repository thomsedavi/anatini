using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.BuilderExtensions
{
    public static class UserEventBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<UserEvent> userEventBuilder)
        {
            userEventBuilder.ToContainer("UserEvents");
            userEventBuilder.HasKey(userEvent => userEvent.ItemId);
            userEventBuilder.HasPartitionKey(userEvent => new { userEvent.UserId, userEvent.EventType });
            userEventBuilder.Property(userEvent => userEvent.ItemId).ToJsonProperty("id");
        }
    }
}
