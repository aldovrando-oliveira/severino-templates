using System;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Severino.Template.Api.Business;
using Severino.Template.Api.Models;
using Xunit;

namespace Severino.Template.Api.Tests.Business.Customers
{
    public class CustomerCreateTest
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Create_InvalidName_Async(string name)
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();
            Mock<IUnitOfWork> moqUnitOfWork = new Mock<IUnitOfWork>();

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => customerBusiness.CreateAsync(new Customer { Name = name }));

            Assert.IsType<ArgumentNullException>(exception);
            Assert.NotEmpty(exception.Message);
            Assert.NotEmpty(exception.ParamName);
            Assert.Equal(nameof(Customer.Name), exception.ParamName);
        }

        [Fact]
        public async Task Create_InvalidParameter_Async()
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();
            Mock<IUnitOfWork> moqUnitOfWork = new Mock<IUnitOfWork>();

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => customerBusiness.CreateAsync(null));

            Assert.IsType<ArgumentNullException>(exception);
            Assert.NotEmpty(exception.Message);
            Assert.NotEmpty(exception.ParamName);
            Assert.Equal("entity", exception.ParamName);
        }

        [Fact]
        public async Task Create_RepositoryError_Async()
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();
            Mock<IUnitOfWork> moqUnitOfWork = new Mock<IUnitOfWork>();
            Mock<IRepository<Customer>> moqRepository = new Mock<IRepository<Customer>>();

            moqRepository.Setup(x => x.Insert(It.IsAny<Customer>())).Throws(new Exception("Erro ao inserir cliente no contexto"));
            moqUnitOfWork.Setup(x => x.GetRepository<Customer>(It.IsAny<bool>())).Returns(moqRepository.Object);

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            Customer customer = new Customer
            {
                Name = "Lampião"
            };

            var exception = await Assert.ThrowsAsync<Exception>(() => customerBusiness.CreateAsync(customer));

            Assert.IsType<Exception>(exception);
            Assert.NotEmpty(exception.Message);
            Assert.Equal("Erro ao inserir cliente no contexto", exception.Message);
        }

        [Fact]
        public async Task Create_UnitOfWorkError_Async()
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();
            Mock<IUnitOfWork> moqUnitOfWork = new Mock<IUnitOfWork>();
            Mock<IRepository<Customer>> moqRepository = new Mock<IRepository<Customer>>();

            moqRepository.Setup(x => x.Insert(It.IsAny<Customer>())).Verifiable();
            moqUnitOfWork.Setup(x => x.GetRepository<Customer>(It.IsAny<bool>())).Returns(moqRepository.Object);
            moqUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<bool>())).ThrowsAsync(new Exception("Erro ao salvar o cliente no banco de dados"));

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            Customer customer = new Customer
            {
                Name = "Lampião"
            };

            var exception = await Assert.ThrowsAsync<Exception>(() => customerBusiness.CreateAsync(customer));

            Assert.IsType<Exception>(exception);
            Assert.NotEmpty(exception.Message);
            Assert.Equal("Erro ao salvar o cliente no banco de dados", exception.Message);
        }

        [Fact]
        public async Task Create_Success_Async()
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();
            Mock<IUnitOfWork> moqUnitOfWork = new Mock<IUnitOfWork>();
            Mock<IRepository<Customer>> moqRepository = new Mock<IRepository<Customer>>();

            moqRepository.Setup(x => x.Insert(It.IsAny<Customer>())).Verifiable();

            moqUnitOfWork.Setup(x => x.GetRepository<Customer>(It.IsAny<bool>())).Returns(moqRepository.Object);
            moqUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            Customer customer = new Customer
            {
                Name = "Lampião"
            };

            var response = await customerBusiness.CreateAsync(customer);
        }
    }
}