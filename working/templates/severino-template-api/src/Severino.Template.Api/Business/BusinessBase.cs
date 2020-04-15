using System;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Severino.Template.Api.Models;

namespace Severino.Template.Api.Business
{
    /// <summary>
    /// Classe base de negócios
    /// </summary>
    /// <typeparam name="TEntity">Tipo da entidade que será manipulada pela classe de negócios</typeparam>
    public abstract class BusinessBase<TEntity> : IBusinessBase<TEntity> where TEntity : class, IModelBase
    {
        private bool _disposed;
        protected ILogger<BusinessBase<TEntity>> Logger;
        protected IUnitOfWork UnitOfWork;

        public BusinessBase(ILogger<BusinessBase<TEntity>> logger, IUnitOfWork unitOfWork)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        
        /// <summary>
        /// Cria uma nova entidade
        /// </summary>
        /// <param name="entity">Nova entidade</param>
        public abstract Task<TEntity> CreateAsync(TEntity entity);

        /// <summary>
        /// Remove uma entidade pelo código
        /// </summary>
        /// <param name="id">Código da entidade</param>
        public abstract Task DeleteAsync(Guid id);

        /// <summary>
        /// Recupera a entidade pelo código
        /// </summary>
        /// <param name="id">Código da entidade</param>
        public abstract Task<TEntity> GetByIdAsync(Guid id);

        /// <summary>
        /// Atualiza a entidade
        /// </summary>
        /// <param name="id">Código da entidade que será atualizado</param>
        /// <param name="entity">Entidade que que será atualizada</param>
        public abstract Task<TEntity> UpdateAsync(Guid id, TEntity entity);

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Libera recursos utilizados pela classe
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                UnitOfWork.Dispose();
                UnitOfWork = null;
            }

            Logger = null;

            _disposed = true;
        }

        /// <summary>
        /// Cria uma exceção quando a classe já teve o métido Dispose utilizado
        /// </summary>
        public virtual void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        /// Retorna o repositório da entidade
        /// </summary>
        /// <typeparam name="TEntity">Tipo da entidade do repositório</typeparam>
        /// <returns></returns>
        protected virtual IRepository<TEntity> GetRepository() => UnitOfWork.GetRepository<TEntity>();
    }
}