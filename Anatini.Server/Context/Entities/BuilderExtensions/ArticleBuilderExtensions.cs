using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ArticleBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Article> articleBuilder)
        {
            articleBuilder.ToTable("articles");

            articleBuilder.Property(article => article.Id).Has(order: 0);
        }
    }
}
