using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Severino.Template.Hangfire.Models;

namespace Severino.Template.Hangfire.Repositories.Configurations.Models
{
    /// <summary>
    /// Classe para a configuração da entidade <see cref="Customer">
    /// </summary>
    public class CustomerEntityTypeConfiguration : BaseEntityTypeConfiguration<Customer>
    {
        /// <summary>
        /// Configuração da entidade
        /// </summary>
        /// <param name="builder">Builder de configuração</param>
        protected override void Configurations(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("customer");
            
            builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        }
    }
}