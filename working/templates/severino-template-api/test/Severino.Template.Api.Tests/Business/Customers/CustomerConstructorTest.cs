using System;
using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Severino.Template.Api.Business;
using Xunit;

namespace Severino.Template.Api.Tests.Business.Customers
{
    public class CustomerConstructorTest
    {
        [Fact]
        public void Constructor_LoggerInvalid()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomerBusiness(null, null));
        }

        [Fact]
        public void Constructor_UnitOfWorkInvalid()
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();

            Assert.Throws<ArgumentNullException>(() => new CustomerBusiness(moqBusiness.Object, null));
        }

        [Fact]
        public void Constructor_Success()
        {
            Mock<ILogger<CustomerBusiness>> moqBusiness = new Mock<ILogger<CustomerBusiness>>();
            Mock<IUnitOfWork> moqUnitOfWork = new Mock<IUnitOfWork>();

            var customerBusiness = new CustomerBusiness(moqBusiness.Object, moqUnitOfWork.Object);

            Assert.IsType<CustomerBusiness>(customerBusiness);
        }
    }
}