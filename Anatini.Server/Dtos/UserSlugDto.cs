namespace Anatini.Server.Dtos
{
    internal class UserSlugDto(UserSlug userSlug)
    {
        public Guid Id { get; } = userSlug.Id;
        public string Slug { get; } = userSlug.Slug;
        public Guid UserId { get; } = userSlug.UserId;
        public string UserName { get; } = userSlug.UserName;
    }
}
