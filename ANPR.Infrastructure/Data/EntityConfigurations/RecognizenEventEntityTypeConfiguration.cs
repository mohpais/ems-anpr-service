using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Lonsum.Services.ANPR.Domain.Entities;

namespace Microsoft.Lonsum.Services.ANPR.Infrastructure.Data
{
    public class RecognizenEventEntityTypeConfiguration
        : IEntityTypeConfiguration<RecognizenEvent>
    {
        public void Configure(EntityTypeBuilder<RecognizenEvent> recognizeConfiguration)
        {
            recognizeConfiguration.ToTable("RecognizenEvents", ANPRContext.DEFAULT_SCHEMA);

            recognizeConfiguration.HasKey(o => o.Id);
            recognizeConfiguration.Ignore(b => b.DomainEvents);

            recognizeConfiguration
                .Property(x => x.EventType)
                .IsRequired();

            recognizeConfiguration
                .Property(x => x.OriginalLicensePlate)
                .IsRequired();

            recognizeConfiguration
                .Property(x => x.PlateNumber)
                .IsRequired();

            recognizeConfiguration
                .Property(x => x.PlateColor)
                .IsRequired();

            recognizeConfiguration
                .Property(x => x.VehicleType)
                .IsRequired();

            recognizeConfiguration
                .Property(x => x.VehicleColor)
                .IsRequired();

            recognizeConfiguration
                .Property(x => x.TransportationType)
                .IsRequired();

            recognizeConfiguration
                .Property(x => x.PlateImagePath)
                .IsRequired();

            recognizeConfiguration
                .Property(x => x.Location)
                .IsRequired();

            recognizeConfiguration
                .Property(x => x.OperatorId)
                .IsRequired();

            recognizeConfiguration
                .Property(x => x.OperatorName)
                .IsRequired();

            recognizeConfiguration
                .Property(x => x.CaptureDate)
                .IsRequired();

            recognizeConfiguration.Property(o => o.CreateBy).IsRequired().HasMaxLength(100);
            recognizeConfiguration.Property(o => o.CreateDate);
            recognizeConfiguration.Property(o => o.LastUpdateBy).IsRequired().HasMaxLength(100);
            recognizeConfiguration.Property(o => o.LastUpdateDate);
            recognizeConfiguration.Property(o => o.IsDelete).HasDefaultValue(0);
        }
    }
}
