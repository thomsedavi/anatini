using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class CommonExtensions
    {
        public static ReferenceCollectionBuilder<TRelatedEntity, TEntity> HasOneWithMany<TEntity, TRelatedEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, TRelatedEntity?>> navigationExpression1, Expression<Func<TRelatedEntity, IEnumerable<TEntity>?>> navigationExpression2, Expression<Func<TEntity, object?>> navigationExpression3, DeleteBehavior deleteBehavior) where TEntity : class where TRelatedEntity : class
        {
            return builder.HasOne(navigationExpression1).WithMany(navigationExpression2).HasForeignKey(navigationExpression3).OnDelete(deleteBehavior);
        }
    }
}
