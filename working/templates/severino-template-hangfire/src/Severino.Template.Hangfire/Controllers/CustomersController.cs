using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Severino.Template.Hangfire.Business;
using Severino.Template.Hangfire.Models;
using Severino.Template.Hangfire.ViewModels.Customers;

namespace Severino.Template.Hangfire.Controllers
{
    /// <summary>
    /// Serviço para manipulação aos dados de clientes
    /// </summary>
    [ApiController]
    [Route("api/v1/customers")]
    public class CustomersController : CustomControllerBase
    {
        CustomerBusiness _customerBusiness;

        /// <summary>
        /// Cria uma nova instância de <see cref="CustomersController" />
        /// </summary>
        /// <param name="customerBusiness"></param>
        public CustomersController(CustomerBusiness customerBusiness)
        {
            _customerBusiness = customerBusiness;
        }

        /// <summary>
        /// Consulta cliente pelo código
        /// </summary>
        /// <param name="id">Código do cliente</param>
        /// <returns>Retorna o cliente encontrado</returns>
        [HttpGet("{id}", Name = "GetCustomerById")]
        public async Task<IActionResult> GetAsync([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Customer customer = await _customerBusiness.GetByIdAsync(id);

            CustomerViewModel response = new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };

            return Ok(response);
        }

        /// <summary>
        /// Cria um novo cliente
        /// </summary>
        /// <param name="request">Informações do novo cliente</param>
        /// <returns>Retorna o cliente criado</returns>
        [HttpPost(Name = "CreateCustomer")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUpdateCustomerRequestViewModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Customer customer = new Customer
            {
                Name = request.Name
            };

            customer = await _customerBusiness.CreateAsync(customer);

            CustomerViewModel response = new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };

            return CreatedAtRoute("GetCustomerById", new { id = customer.Id }, response);
        }

        /// <summary>
        /// Exclui um cliente
        /// </summary>
        /// <param name="id">Código do cliente</param>
        /// <returns></returns>
        [HttpDelete(Name = "DeleteCustomer")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _customerBusiness.DeleteAsync(id);

            return NoContent();
        }

        /// <summary>
        /// Atualiza as informações do cliente
        /// </summary>
        /// <param name="id">Código do cliente</param>
        /// <param name="request">Informações do cliente que serão atualizados</param>
        /// <returns>Retorna o cliente atualizado</returns>
        [HttpPut("{id}", Name = "UpdateCustomer")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] CreateUpdateCustomerRequestViewModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Customer customer = new Customer
            {
                Name = request.Name
            };

            await _customerBusiness.UpdateAsync(id, customer);

            customer = await _customerBusiness.GetByIdAsync(id);

            return Ok(customer);
        }
    }
}