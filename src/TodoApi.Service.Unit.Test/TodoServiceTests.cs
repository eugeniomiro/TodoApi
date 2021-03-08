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
                    _repositoryMock = new Mock<ITodoRepository>().SetupGetAllAsyncToReturn(new List<TodoItem> { new TodoItem { } });
                    
                    var sut = new TodoService(_repositoryMock.Object);

                    // Act
                    list = sut.GetAllAsync().Result;
                }

                [TestMethod]
                public void It_Calls_GetAll_Repository_Method()
                {
                    // Assert
                    _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
                }

                [TestMethod]
                public void Resulting_List_Has_Data()
                {
                    // Assert
                    Assert.AreEqual(1, list.Count());
                }

                private IEnumerable<TodoItem> list;
            }

            [TestClass]
            public class When_Calling_GetTodo_Method_From_Sut_With_Matching_Ids : TodoServiceContext
            {
                protected override void Context()
                {
                    // Arrange
                    _repositoryMock = new Mock<ITodoRepository>().SetupGetTodoAsyncToReturnItem(1, new TodoItem { Id = 1, Name = "name", IsComplete = false });
                    _sut = new TodoService(_repositoryMock.Object);

                    // Act
                    _returnedTodo = _sut.GetTodoAsync(1L).Result;
                }

                [TestMethod]
                public void It_Returns_An_Expected_Item_TodoItem()
                {
                    // Assert
                    Assert.IsNotNull(_returnedTodo);
                    Assert.AreEqual(1, _returnedTodo.Id);
                    Assert.AreEqual("name", _returnedTodo.Name);
                    Assert.AreEqual(false, _returnedTodo.IsComplete);
                }

                [TestMethod]
                public void It_Calls_GetTodoAsync_Repository_Method()
                {
                    // Assert
                    _repositoryMock.Verify(r => r.GetTodoAsync(1), Times.Once);
                }

                private TodoItem _returnedTodo;
                private TodoService _sut;
            }

            [TestClass]
            public class When_Calling_Create_Method_From_Sut : TodoServiceContext
            {
                protected override void Context()
                {
                    // Arrange
                    _repositoryMock = new Mock<ITodoRepository>().SetupCreateAsyncToReturnItem();
                    _sut = new TodoService(_repositoryMock.Object);
                    _returnedTodo = _sut.CreateAsync(new TodoItem { Id = 1, Name = "name", IsComplete = false }).Result;
                }

                [TestMethod]
                public void It_Returns_An_Expected_Item_TodoItem()
                {
                    // Assert
                    Assert.IsNotNull(_returnedTodo);
                }

                [TestMethod]
                public void It_Calls_CreateAsync_Method_From_Repository()
                {
                    _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<TodoItem>()), Times.Once);
                }

                private TodoItem _returnedTodo;
                private TodoService _sut;
            }

            [TestClass]
            public class When_Calling_Update_Method_From_Sut : TodoServiceContext
            {
                protected override void Context()
                {
                    // Arrange
                    _repositoryMock = new Mock<ITodoRepository>();
                    _sut = new TodoService(_repositoryMock.Object);

                    // Act
                    var _ = _sut.UpdateAsync(1L, new TodoItem()).Result;
                }

                [TestMethod]
                public void It_Calls_UpdateAsync_Method_From_Repository()
                {
                    _repositoryMock.Verify(r => r.UpdateAsync(1L, It.IsAny<TodoItem>()), Times.Once);
                }

                private TodoService _sut;
            }

            [TestClass]
            public class When_Calling_Delete_Method_From_Sut : TodoServiceContext
            {
                protected override void Context()
                {
                    // Arrange
                    _repositoryMock = new Mock<ITodoRepository>();
                    _sut = new TodoService(_repositoryMock.Object);

                    // Act
                    var _ = _sut.DeleteAsync(1L).Result;
                }

                [TestMethod]
                public void It_Calls_DeleteAsync_Method_From_Repository()
                {
                    _repositoryMock.Verify(r => r.DeleteAsync(1L), Times.Once);
                }

                private TodoService _sut;
            }

            private Mock<ITodoRepository> _repositoryMock;
        }
    }
}
