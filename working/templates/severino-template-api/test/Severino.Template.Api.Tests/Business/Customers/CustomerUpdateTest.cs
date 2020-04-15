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
    public class CustomerUpdateTest
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Update_InvalidName_Async(string name)
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();
            Mock<IUnitOfWork> moqUnitOfWork = new Mock<IUnitOfWork>();

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => customerBusiness.UpdateAsync(Guid.NewGuid(), new Customer { Name = name }));

            Assert.IsType<ArgumentNullException>(exception);
            Assert.NotEmpty(exception.Message);
            Assert.NotEmpty(exception.ParamName);
            Assert.Equal(nameof(Customer.Name), exception.ParamName);
        }

        [Fact]
        public async Task Update_InvalidId_Async()
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();
            Mock<IUnitOfWork> moqUnitOfWork = new Mock<IUnitOfWork>();

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => customerBusiness.UpdateAsync(Guid.Empty, new Customer()));

            Assert.IsType<ArgumentException>(exception);
            Assert.NotEmpty(exception.Message);
            Assert.NotEmpty(exception.ParamName);
            Assert.Equal("id", exception.ParamName);
        }

        [Fact]
        public async Task Update_CustomerNull_Async()
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();
            Mock<IUnitOfWork> moqUnitOfWork = new Mock<IUnitOfWork>();

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => customerBusiness.UpdateAsync(Guid.NewGuid(), null));

            Assert.IsType<ArgumentNullException>(exception);
            Assert.NotEmpty(exception.Message);
            Assert.NotEmpty(exception.ParamName);
            Assert.Equal("Value cannot be null. (Parameter 'entity')", exception.Message);
            Assert.Equal("entity", exception.ParamName);
        }

        [Fact]
        public async Task Update_CustomerNotFound_Async()
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

            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => customerBusiness.UpdateAsync(Guid.NewGuid(), new Customer
            {
                Name = "Lampião"
            }));

            Assert.NotEmpty(exception.Message);
            Assert.NotEmpty(exception.Entity);
            Assert.Equal(nameof(Customer), exception.Entity);
            Assert.Equal("Customer not found", exception.Message);
        }

        [Fact]
        public async Task Update_RepositoryError_Async()
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
            moqRepository.Setup(x => x.Update(It.IsAny<Customer>())).Throws(new Exception("Erro ao atualizar cliente no contexto"));
            moqUnitOfWork.Setup(x => x.GetRepository<Customer>(It.IsAny<bool>())).Returns(moqRepository.Object);

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            var exception = await Assert.ThrowsAsync<Exception>(() => customerBusiness.UpdateAsync(id, new Customer
            {
                Name = "Maria Bonita"
            }));

            Assert.IsType<Exception>(exception);
            Assert.NotNull(exception.Message);
            Assert.Equal("Erro ao atualizar cliente no contexto", exception.Message);
        }

        [Fact]
        public async Task Update_UnitOfWorkError_Async()
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
            moqUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<bool>())).ThrowsAsync(new Exception("Erro ao salvar dados do cliente no banco de dados"));

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            var exception = await Assert.ThrowsAsync<Exception>(() => customerBusiness.UpdateAsync(id, new Customer
            {
                Name = "Maria Bonita"
            }));

            Assert.IsType<Exception>(exception);
            Assert.NotNull(exception.Message);
            Assert.Equal("Erro ao salvar dados do cliente no banco de dados", exception.Message);
        }

        [Fact]
        public async Task Update_Success_Async()
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
            moqUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            Customer updatedCustomer = await customerBusiness.UpdateAsync(id, new Customer
            {
                Name = "Maria Bonita"
            });

            Assert.Equal(id, updatedCustomer.Id);
            Assert.Equal("Maria Bonita", updatedCustomer.Name);
        }
    }
}