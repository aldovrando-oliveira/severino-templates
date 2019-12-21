using System;
using System.Threading.Tasks;
using Severino.Template.Api.Models;

namespace Severino.Template.Api.Business
{
    /// <summary>
    /// Contrato base das classes de negócios
    /// </summary>
    /// <typeparam name="TEntity">Tipo da entidade que será manipulada pela classe de negócios</typeparam>
    public interface IBusinessBase<TEntity> : IDisposable
        where TEntity : class, IModelBase
    {
        /// <summary>
        /// Cria uma nova entidade
        /// </summary>
        /// <param name="entity">Nova entidade</param>
        Task<TEntity> CreateAsync(TEntity entity);

        /// <summary>
        /// Atualiza a entidade
        /// </summary>
        /// <param name="id">Código da entidade que será atualizado</param>
        /// <param name="entity">Entidade que que será atualizada</param>
        Task<TEntity> UpdateAsync(Guid id, TEntity entity);

        /// <summary>
        /// Recupera a entidade pelo código
        /// </summary>
        /// <param name="id">Código da entidade</param>
        Task<TEntity> GetByIdAsync(Guid id);

        /// <summary>
        /// Remove uma entidade pelo código
        /// </summary>
        /// <param name="id">Código da entidade</param>
        Task DeleteAsync(Guid id);
    }
}