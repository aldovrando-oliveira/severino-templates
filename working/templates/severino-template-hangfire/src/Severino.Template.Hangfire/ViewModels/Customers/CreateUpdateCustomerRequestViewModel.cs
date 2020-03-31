using System.ComponentModel.DataAnnotations;

namespace Severino.Template.Hangfire.ViewModels.Customers
{
    /// <summary>
    /// View Model para criar ou atualizar um cliente
    /// </summary>
    public class CreateUpdateCustomerRequestViewModel
    {
        /// <summary>
        /// Nome do clientne
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}