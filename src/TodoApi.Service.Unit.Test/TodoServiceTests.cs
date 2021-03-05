using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleSpec.Bdd;

namespace TodoApi.Service.Unit.Test
{
    using DataAccess.Contract;
    using Domain.Models;
    using Service.Concrete;

    public static class TodoServiceTests
    {
        [TestClass]
        public class TodoServiceContext: ContextSpecification
        {
            [TestClass]
            public class TodoService_Object
            {
                [TestMethod]
                public void Can_Be_Created()
                {
                    new TodoService(new Mock<ITodoRepository>().Object);
                }
            }

            [TestClass]
            public class When_Calling_GetAll_Method_From_Sut: TodoServiceContext
            {
                protected override void Context()
                {
                    // Arrange
                    repositoryMock = new Mock<ITodoRepository>();
                    repositoryMock.Setup(r => r.GetAllAsync())
                                  .ReturnsAsync(new List<TodoItem> { new TodoItem { } });
                    var sut = new TodoService(repositoryMock.Object);

                    // Act
                    list = sut.GetAllAsync().Result;
                }

                [TestMethod]
                public void It_Calls_GetAll_Repository_Method()
                {
                    // Assert
                    repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
                }

                [TestMethod]
                public void Resulting_List_Has_Data()
                {
                    // Assert
                    Assert.AreEqual(1, list.Count());
                }

                private IEnumerable<TodoItem> list;
                private Mock<ITodoRepository> repositoryMock;
            }
        }
    }
}
