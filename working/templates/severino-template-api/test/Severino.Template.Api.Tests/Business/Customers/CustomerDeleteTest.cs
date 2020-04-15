using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using Severino.Template.Api.Business;
using Severino.Template.Api.Exceptions;
using Severino.Template.Api.Models;
using Xunit;

namespace Severino.Template.Api.Tests.Business.Customers
{
    public class CustomerDeleteTest
    {
        [Fact]
        public async Task Delete_InvalidId_Async()
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();
            Mock<IUnitOfWork> moqUnitOfWork = new Mock<IUnitOfWork>();
            
            Guid id = Guid.Empty;

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => customerBusiness.DeleteAsync(id));
            
            Assert.NotEmpty(exception.Message);
            Assert.NotEmpty(exception.ParamName);
        }

        [Fact]
        public async Task Delete_NotFound_Async()
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();
            Mock<IUnitOfWork> moqUnitOfWork = new Mock<IUnitOfWork>();
            Mock<IRepository<Customer>> moqRepository = new Mock<IRepository<Customer>>();
            
            Guid id = Guid.NewGuid();
            
            moqRepository.Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Customer, bool>>>(),
                It.IsAny<Func<IQueryable<Customer>, IOrderedQueryable<Customer>>>(),
                It.IsAny<Func<IQueryable<Customer>, IIncludableQueryable<Customer, object>>>(),
                It.IsAny<bool>(), false
            )).ReturnsAsync(default(Customer));

            moqUnitOfWork.Setup(x => x.GetRepository<Customer>(It.IsAny<bool>())).Returns(moqRepository.Object);

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => customerBusiness.DeleteAsync(id));

            Assert.NotEmpty(exception.Message);
            Assert.NotEmpty(exception.Entity);
            Assert.Equal(nameof(Customer), exception.Entity);
            Assert.Equal("Customer not found", exception.Message);
        }

        [Fact]
        public async Task Delete_RepositoryError_Async()
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();
            Mock<IUnitOfWork> moqUnitOfWork = new Mock<IUnitOfWork>();
            Mock<IRepository<Customer>> moqRepository = new Mock<IRepository<Customer>>();
            
            Guid id = Guid.NewGuid();

            Customer customer = new Customer
            {
                Name = "Lampião"
            };

            moqRepository.Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Customer, bool>>>(),
                It.IsAny<Func<IQueryable<Customer>, IOrderedQueryable<Customer>>>(),
                It.IsAny<Func<IQueryable<Customer>, IIncludableQueryable<Customer, object>>>(),
                It.IsAny<bool>(), false
            )).ReturnsAsync(customer);

            moqRepository.Setup(x => x.Delete(It.IsAny<Customer>()))
                .Throws(new Exception("Erro ao excluir cliente do contexto"));

            moqUnitOfWork.Setup(x => x.GetRepository<Customer>(It.IsAny<bool>())).Returns(moqRepository.Object);

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            var exception = await Assert.ThrowsAsync<Exception>(() => customerBusiness.DeleteAsync(id));

            Assert.NotEmpty(exception.Message);
            Assert.Equal("Erro ao excluir cliente do contexto", exception.Message);
        }

        [Fact]
        public async Task Delete_UnitOfWorkError_Async()
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();
            Mock<IUnitOfWork> moqUnitOfWork = new Mock<IUnitOfWork>();
            Mock<IRepository<Customer>> moqRepository = new Mock<IRepository<Customer>>();
            
            Guid id = Guid.NewGuid();

            Customer customer = new Customer
            {
                Name = "Lampião"
            };

            moqRepository.Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Customer, bool>>>(),
                It.IsAny<Func<IQueryable<Customer>, IOrderedQueryable<Customer>>>(),
                It.IsAny<Func<IQueryable<Customer>, IIncludableQueryable<Customer, object>>>(),
                It.IsAny<bool>(), false
            )).ReturnsAsync(customer);

            moqUnitOfWork.Setup(x => x.GetRepository<Customer>(It.IsAny<bool>())).Returns(moqRepository.Object);

            moqUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<bool>()))
                .ThrowsAsync(new Exception("Erro ao excluir cliente do banco de dados"));
            
            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            var exception = await Assert.ThrowsAsync<Exception>(() => customerBusiness.DeleteAsync(id));

            Assert.NotEmpty(exception.Message);
            Assert.Equal("Erro ao excluir cliente do banco de dados", exception.Message);
        }

        [Fact]
        public async Task Delete_Success_Async()
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();
            Mock<IUnitOfWork> moqUnitOfWork = new Mock<IUnitOfWork>();
            Mock<IRepository<Customer>> moqRepository = new Mock<IRepository<Customer>>();
            
            Guid id = Guid.NewGuid();

            int getAttemps = 0;
            int deleteAttemps = 0;
            int saveAttemps = 0;
            
            Customer customer = new Customer
            {
                Name = "Lampião"
            };

            moqRepository.Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Customer, bool>>>(),
                It.IsAny<Func<IQueryable<Customer>, IOrderedQueryable<Customer>>>(),
                It.IsAny<Func<IQueryable<Customer>, IIncludableQueryable<Customer, object>>>(),
                It.IsAny<bool>(), false
            )).ReturnsAsync(customer).Callback(() =>
            {
                getAttemps++;
            });

            moqRepository.Setup(x => x.Delete(It.IsAny<Customer>())).Callback(() =>
            {
                deleteAttemps++;
            });
            
            moqUnitOfWork.Setup(x => x.GetRepository<Customer>(It.IsAny<bool>())).Returns(moqRepository.Object);

            moqUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<bool>()))
                .ReturnsAsync(1).Callback(() =>
                {
                    saveAttemps++;
                });
            
            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            await customerBusiness.DeleteAsync(id);

            Assert.Equal(1, deleteAttemps);
            Assert.Equal(1, saveAttemps);
            Assert.Equal(1, getAttemps);
        }
    }
}