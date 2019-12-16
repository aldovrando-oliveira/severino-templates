using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Severino.Template.Api.Exceptions;
using Severino.Template.Api.Models;

namespace Severino.Template.Api.Business
{
    /// <summary>
    /// Classe de negócio de clientes
    /// </summary>
    public class CustomerBusiness : BusinessBase<Customer>
    {
        /// <summary>
        /// Cria uma nova instância de <see cref="CustomerBusiness"/>
        /// </summary>
        /// <param name="logger">Objeto para registro de logs</param>
        /// <param name="unitOfWork">Objeto para manipulação dos repositórios</param>
        public CustomerBusiness(ILogger<BusinessBase<Customer>> logger, IUnitOfWork unitOfWork) 
            : base(logger, unitOfWork)
        {
        }

        /// <summary>
        /// Insere um novo cliente
        /// </summary>
        /// <param name="entity">Cliente a ser inserido</param>
        /// <returns></returns>
        public override async Task<Customer> CreateAsync(Customer entity)
        {
            ThrowIfDisposed();
            ThrowIfParameterNull(entity, nameof(entity));

            IRepository<Customer> repository = GetRepository();

            Logger.LogInformation("Inserindo um novo cliente");
            
            try
            {
                Logger.LogDebug("Inserindo um novo cliente no repositório");
                repository.Insert(entity);
                
                Logger.LogDebug("Comitando informações no banco de dados");
                await UnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Erro ao inserir um novo cliente: {0}", ex.Message);
                throw;
            }

            return entity;
        }

        /// <summary>
        /// Atualiza o cliente
        /// </summary>
        /// <param name="id">Código do cliente</param>
        /// <param name="entity">Objeto com os dados de cliente a serem atualizados</param>
        /// <returns></returns>
        public override async Task UpdateAsync(Guid id, Customer entity)
        {
            ThrowIfDisposed();
            ThrowIfGuidInvalid(id, nameof(id));
            ThrowIfParameterNull(entity, nameof(entity));

            Customer customer = await GetByIdAsync(id);

            IRepository<Customer> repository = GetRepository();

            customer.Name = entity.Name;
            customer.UpdatedAt = DateTimeOffset.Now;

            Logger.LogInformation("Atualizando o cliente com o código '{0}'", id);

            try
            {
                Logger.LogDebug("Atualizando cliente no repositório");
                repository.Update(entity);
                
                Logger.LogDebug("Comitando informações no banco de dados");
                await UnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Erro ao atualizar informações do cliente: {0}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Exclui um cliente
        /// </summary>
        /// <param name="id">Código do cliente a ser excluído</param>
        /// <returns></returns>
        public override async Task DeleteAsync(Guid id)
        {
            ThrowIfDisposed();
            ThrowIfGuidInvalid(id, nameof(id));

            Customer customer = await GetByIdAsync(id);

            IRepository<Customer> repository = GetRepository();
            
            Logger.LogInformation("Excluindo o cliente '{0}'", customer.Name);

            try
            {
                Logger.LogDebug("Excluindo cliente '{0}' do repositório", id);
                repository.Delete(customer);
                
                Logger.LogDebug("Comitando informações no banco de dados");
                await UnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Erro ao excluir cliente '{0}' com o código '{1}'", customer.Name, customer.Id);
                throw;
            }
        }

        /// <summary>
        /// Consulta um cliente pelo código
        /// </summary>
        /// <param name="id">Código do cliente</param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException">Ocorre quando o cliente não é encontrado</exception>
        public override async Task<Customer> GetByIdAsync(Guid id)
        {
            ThrowIfDisposed();
            ThrowIfGuidInvalid(id, nameof(id));

            Customer customer;
            
            IRepository<Customer> repository = GetRepository();
            
            Logger.LogInformation("Consultando cliente código '{0}'", id);

            try
            {
                Logger.LogDebug("Consultando cliente '{0}' no repositório", id);
                customer = await repository.GetFirstOrDefaultAsync(predicate: x => x.Id == id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Erro ao consultar cliente '{0}': {1}", id, ex.Message);
                throw;
            }

            if (customer == null)
                throw new EntityNotFoundException(nameof(customer));

            return customer;
        }
    }
}