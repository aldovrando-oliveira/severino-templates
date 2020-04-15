using System.Linq;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Severino.Template.Api.Business;
using Severino.Template.Api.Models;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;
using Severino.Template.Api.Exceptions;

namespace Severino.Template.Api.Tests.Business.Customers
{
    public class CustomerGetByIdTest
    {
        [Fact]
        public async Task GetById_NotFound_Async()
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

            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => customerBusiness.GetByIdAsync(id));

            Assert.NotEmpty(exception.Message);
            Assert.NotEmpty(exception.Entity);
            Assert.Equal(nameof(Customer), exception.Entity);
            Assert.Equal("Customer not found", exception.Message);
        }

        [Fact]
        public async Task GetById_IdInvalid_Async()
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();
            Mock<IUnitOfWork> moqUnitOfWork = new Mock<IUnitOfWork>();
            
            Guid id = Guid.Empty;

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => customerBusiness.GetByIdAsync(id));

            Assert.NotEmpty(exception.Message);
        }

        [Fact]
        public async Task GetById_RepositoryError_Async()
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();
            Mock<IUnitOfWork> moqUnitOfWork = new Mock<IUnitOfWork>();
            Mock<IRepository<Customer>> moqRepository = new Mock<IRepository<Customer>>();

            moqRepository.Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Customer, bool>>>(),
                It.IsAny<Func<IQueryable<Customer>, IOrderedQueryable<Customer>>>(),
                It.IsAny<Func<IQueryable<Customer>, IIncludableQueryable<Customer, object>>>(),
                It.IsAny<bool>(), false
            )).ThrowsAsync(new Exception("Erro ao consultar cliente no banco de dados"));

            moqUnitOfWork.Setup(x => x.GetRepository<Customer>(It.IsAny<bool>())).Returns(moqRepository.Object);

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            var exception = await Assert.ThrowsAsync<Exception>(() => customerBusiness.GetByIdAsync(Guid.NewGuid()));

            Assert.IsType<Exception>(exception);
            Assert.NotNull(exception.Message);
            Assert.Equal("Erro ao consultar cliente no banco de dados", exception.Message);
        }

        [Fact]
        public async Task GetById_Success_Async()
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();
            Mock<IUnitOfWork> moqUnitOfWork = new Mock<IUnitOfWork>();
            Mock<IRepository<Customer>> moqRepository = new Mock<IRepository<Customer>>();
            
            Guid id = Guid.NewGuid();

            Customer customer = new Customer
            {
                Id = id,
                Name = "Lampião"
            };
            
            moqRepository.Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Customer, bool>>>(),
                It.IsAny<Func<IQueryable<Customer>, IOrderedQueryable<Customer>>>(),
                It.IsAny<Func<IQueryable<Customer>, IIncludableQueryable<Customer, object>>>(),
                It.IsAny<bool>(), false
            )).ReturnsAsync(customer);

            moqUnitOfWork.Setup(x => x.GetRepository<Customer>(It.IsAny<bool>())).Returns(moqRepository.Object);

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            var response = await customerBusiness.GetByIdAsync(id);

            Assert.IsType<Customer>(response);
            Assert.NotEmpty(response.Name);
            Assert.Equal(id, response.Id);
            Assert.Equal(customer.Name, response.Name);
            Assert.Equal(customer.CreatedAt, response.CreatedAt);
            Assert.Equal(customer.UpdatedAt, response.UpdatedAt);
        }
    }
}