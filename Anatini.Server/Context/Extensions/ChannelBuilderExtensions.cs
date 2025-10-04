using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Extensions
{
    public static class ChannelBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Channel> channelBuilder)
        {
            channelBuilder.ToContainer("Channels");
            channelBuilder.HasKey(channel => channel.Id);
            channelBuilder.HasPartitionKey(channel => channel.Id);
            channelBuilder.Property(channel => channel.Id).ToJsonProperty("id");
            channelBuilder.Property(channel => channel.Name).ToJsonProperty("name");
            channelBuilder.OwnsMany(channel => channel.Users, ConfigureUsers);
            channelBuilder.OwnsMany(channel => channel.Aliases, ConfigureAliases);
            channelBuilder.OwnsMany(channel => channel.TopDraftPosts, ConfigureTopDraftPosts);
            channelBuilder.OwnsMany(channel => channel.TopPublishedPosts, ConfigureTopPublishedPosts);
        }

        private static void ConfigureUsers(OwnedNavigationBuilder<Channel, ChannelOwnedUser> usersBuilder)
        {
            usersBuilder.ToJsonProperty("users");
            usersBuilder.Property(user => user.Id).ToJsonProperty("id");
            usersBuilder.Property(user => user.ChannelId).ToJsonProperty("channelId");
            usersBuilder.Property(user => user.Name).ToJsonProperty("name");
        }

        private static void ConfigureAliases(OwnedNavigationBuilder<Channel, ChannelOwnedAlias> aliasesBuilder)
        {
            aliasesBuilder.ToJsonProperty("aliases");
            aliasesBuilder.Property(alias => alias.Slug).ToJsonProperty("slug");
            aliasesBuilder.Property(alias => alias.ChannelId).ToJsonProperty("channelId");
        }

        private static void ConfigureTopDraftPosts(OwnedNavigationBuilder<Channel, ChannelOwnedPost> topDraftPostsBuilder)
        {
            topDraftPostsBuilder.ToJsonProperty("topDraftPosts");
            topDraftPostsBuilder.Property(topDraftPost => topDraftPost.Id).ToJsonProperty("id");
            topDraftPostsBuilder.Property(topDraftPost => topDraftPost.ChannelId).ToJsonProperty("channelId");
            topDraftPostsBuilder.Property(topDraftPost => topDraftPost.Name).ToJsonProperty("name");
            topDraftPostsBuilder.Property(topDraftPost => topDraftPost.DefaultSlug).ToJsonProperty("defaultSlug");
            topDraftPostsBuilder.Property(topDraftPost => topDraftPost.UpdatedDateTimeUTC).ToJsonProperty("updatedDateTimeUTC");
        }

        private static void ConfigureTopPublishedPosts(OwnedNavigationBuilder<Channel, ChannelOwnedPost> topPublishedPostsBuilder)
        {
            topPublishedPostsBuilder.ToJsonProperty("topPublishedPosts");
            topPublishedPostsBuilder.Property(topPublishedPost => topPublishedPost.Id).ToJsonProperty("id");
            topPublishedPostsBuilder.Property(topPublishedPost => topPublishedPost.ChannelId).ToJsonProperty("channelId");
            topPublishedPostsBuilder.Property(topPublishedPost => topPublishedPost.Name).ToJsonProperty("name");
            topPublishedPostsBuilder.Property(topPublishedPost => topPublishedPost.DefaultSlug).ToJsonProperty("defaultSlug");
            topPublishedPostsBuilder.Property(topPublishedPost => topPublishedPost.UpdatedDateTimeUTC).ToJsonProperty("updatedDateTimeUTC");
        }
    }
}
