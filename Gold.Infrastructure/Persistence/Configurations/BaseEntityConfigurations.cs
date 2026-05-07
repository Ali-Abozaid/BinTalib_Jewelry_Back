using Gold.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Gold.Infrastructure.Persistence.Configurations;

internal static class BaseEntityConfigurationExtensions
{
    public static EntityTypeBuilder<T> ConfigureBaseKey<T>(this EntityTypeBuilder<T> builder) where T : Gold.Core.Common.BaseEntity
    {
        builder.Property(e => e.Id).ValueGeneratedOnAdd().HasValueGenerator<SequentialGuidValueGenerator>();
        return builder;
    }
}
