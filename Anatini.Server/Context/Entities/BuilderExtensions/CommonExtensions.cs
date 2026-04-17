using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class CommonExtensions
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

        public static PropertyBuilder<T?> Has<T>(this PropertyBuilder<T?> propertyBuilder, int? order = null)
        {
            if (order.HasValue)
            {
                propertyBuilder.HasColumnOrder(order.Value);
            }

            return propertyBuilder;
        }

        public static PropertyBuilder<T?> HasConversion<T>(this PropertyBuilder<T?> propertyBuilder)
        {
            propertyBuilder.HasConversion(metaData => JsonSerializer.Serialize(metaData, JsonSerializerOptions), metaData => JsonSerializer.Deserialize<T>(metaData)).HasColumnType("jsonb");

            return propertyBuilder;
        }

        public static PropertyBuilder<string?> Has(this PropertyBuilder<string?> propertyBuilder, int? maxLength = null, int? order = null)
        {
            if (maxLength.HasValue)
            {
                propertyBuilder.HasMaxLength(maxLength.Value);
            }

            if (order.HasValue)
            {
                propertyBuilder.HasColumnOrder(order.Value);
            }

            return propertyBuilder;
        }

        public static PropertyBuilder<Guid> Has(this PropertyBuilder<Guid> propertyBuilder, int? order = null)
        {
            if (order.HasValue)
            {
                propertyBuilder.HasColumnOrder(order.Value);
            }

            return propertyBuilder;
        }

        public static PropertyBuilder<Guid?> Has(this PropertyBuilder<Guid?> propertyBuilder, int? order = null)
        {
            if (order.HasValue)
            {
                propertyBuilder.HasColumnOrder(order.Value);
            }

            return propertyBuilder;
        }

        public static PropertyBuilder<DateTime> Has(this PropertyBuilder<DateTime> propertyBuilder, int? order = null)
        {
            propertyBuilder.HasColumnType("timestamp with time zone");

            if (order.HasValue)
            {
                propertyBuilder.HasColumnOrder(order.Value);
            }

            return propertyBuilder;
        }

        public static PropertyBuilder<DateOnly> Has(this PropertyBuilder<DateOnly> propertyBuilder, int? order = null)
        {
            if (order.HasValue)
            {
                propertyBuilder.HasColumnOrder(order.Value);
            }

            return propertyBuilder;
        }

        public static ReferenceCollectionBuilder<TRelatedEntity, TEntity> HasOneWithMany<TEntity, TRelatedEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, TRelatedEntity?>> navigationExpression1, Expression<Func<TRelatedEntity, IEnumerable<TEntity>?>> navigationExpression2, Expression<Func<TEntity, object?>> navigationExpression3, DeleteBehavior deleteBehavior, bool required = true) where TEntity : class where TRelatedEntity : class
        {
            return builder.HasOne(navigationExpression1).WithMany(navigationExpression2).HasForeignKey(navigationExpression3).OnDelete(deleteBehavior).IsRequired(required);
        }
    }
}
