using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Extensions
{
    public static class UserBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<User> userBuilder)
        {
            userBuilder.ToContainer("Users");
            userBuilder.HasKey(user => user.Id);
            userBuilder.HasPartitionKey(user => user.Id);
            userBuilder.Property(user => user.Id).ToJsonProperty("id");
            userBuilder.Property(user => user.Name).ToJsonProperty("name");
            userBuilder.Property(user => user.DefaultSlug).ToJsonProperty("defaultSlug");
            userBuilder.Property(user => user.HashedPassword).ToJsonProperty("hashedPassword");
            userBuilder.OwnsMany(user => user.Sessions, ConfigureSessions);
            userBuilder.OwnsMany(user => user.Emails, ConfigureEmails);
            userBuilder.OwnsMany(user => user.Invites, ConfigureInvites);
            userBuilder.OwnsMany(user => user.Channels, ConfigureChannels);
            userBuilder.OwnsMany(user => user.Aliases, ConfigureAliases);
        }

        private static void ConfigureSessions(OwnedNavigationBuilder<User, UserOwnedSession> sessionsBuilder)
        {
            sessionsBuilder.ToJsonProperty("sessions");
            sessionsBuilder.Property(session => session.Id).ToJsonProperty("id");
            sessionsBuilder.Property(session => session.UserId).ToJsonProperty("userId");
            sessionsBuilder.Property(session => session.IPAddress).ToJsonProperty("ipAddress");
            sessionsBuilder.Property(session => session.UserAgent).ToJsonProperty("userAgent");
            sessionsBuilder.Property(session => session.RefreshToken).ToJsonProperty("refreshToken");
            sessionsBuilder.Property(session => session.Revoked).ToJsonProperty("revoked");
            sessionsBuilder.Property(session => session.CreatedDateTimeUtc).ToJsonProperty("createdDateTimeUtc");
            sessionsBuilder.Property(session => session.UpdatedDateTimeUtc).ToJsonProperty("updatedDateTimeUtc");
        }

        private static void ConfigureEmails(OwnedNavigationBuilder<User, UserOwnedEmail> emailsBuilder)
        {
            emailsBuilder.ToJsonProperty("emails");
            emailsBuilder.Property(email => email.Address).ToJsonProperty("address");
            emailsBuilder.Property(email => email.UserId).ToJsonProperty("userId");
            emailsBuilder.Property(email => email.Verified).ToJsonProperty("verified");
        }

        private static void ConfigureInvites(OwnedNavigationBuilder<User, UserOwnedInvite> invitesBuilder)
        {
            invitesBuilder.ToJsonProperty("invites");
            invitesBuilder.Property(invite => invite.Code).ToJsonProperty("code");
            invitesBuilder.Property(invite => invite.UserId).ToJsonProperty("userId");
            invitesBuilder.Property(invite => invite.Used).ToJsonProperty("used");
            invitesBuilder.Property(invite => invite.DateOnlyNZ).ToJsonProperty("dateOnlyNZ");
        }

        private static void ConfigureChannels(OwnedNavigationBuilder<User, UserOwnedChannel> channelsBuilder)
        {
            channelsBuilder.ToJsonProperty("channels");
            channelsBuilder.Property(channel => channel.Id).ToJsonProperty("id");
            channelsBuilder.Property(channel => channel.UserId).ToJsonProperty("userId");
            channelsBuilder.Property(channel => channel.Name).ToJsonProperty("name");
            channelsBuilder.Property(channel => channel.DefaultSlug).ToJsonProperty("defaultSlug");
        }

        private static void ConfigureAliases(OwnedNavigationBuilder<User, UserOwnedAlias> aliasesBuilder)
        {
            aliasesBuilder.ToJsonProperty("aliases");
            aliasesBuilder.Property(alias => alias.Slug).ToJsonProperty("slug");
            aliasesBuilder.Property(alias => alias.UserId).ToJsonProperty("userId");
        }
    }
}
