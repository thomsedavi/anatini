using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ArticleVersionBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ArticleVersion> articleVersionBuilder)
        {
            articleVersionBuilder.ToTable("article_versions");

            articleVersionBuilder.Property(articleVersion => articleVersion.Id).Has(order: 0);
        }
    }
}
