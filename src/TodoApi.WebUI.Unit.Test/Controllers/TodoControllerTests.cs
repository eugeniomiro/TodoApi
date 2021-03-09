using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleSpec.Bdd;

namespace TodoApi.Unit.Test.Controllers
{
    using Domain.Models;
    using Service.Contract;
    using TodoApi.Domain.SumTypes;
    using WebUI;
    using WebUI.Controllers;

    public static class TodoControllerTests 
    {
        public class TodoControllerContext : ContextSpecification
        {
            protected TodoController _sut;
            
            [TestClass]
            public class Creating_TodoController_With_Default_TodoService : TodoControllerContext
            {
                [TestMethod]
                public void It_Throws_ArgumentNullException()
                {
                    Assert.ThrowsException<ArgumentNullException>(() => new TodoController(default));
                }
            }

            [TestClass]
            public class When_Calling_GetAll_Method : TodoControllerContext
            {
                protected override void Context()
                {
                    _todoServiceMock = new Mock<ITodoService>();
                    _sut = new TodoController(_todoServiceMock.Object);
                    _sut.GetAll().Wait();
                }

                [TestMethod]
                public void It_Calls_GetAllAsync_Method_Of_TodoService()
                {
                    _todoServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
                }

                private Mock<ITodoService> _todoServiceMock;
            }

            [TestClass]
            public class When_Calling_GetTodo_Method_Finds_A_Record : TodoControllerContext
            {
                protected override void Context()
                {
                    _todoServiceMock = new Mock<ITodoService>();
                    _todoServiceMock.Setup(s => s.GetTodoAsync(1))
                                    .ReturnsAsync(new TodoItem());
                    _sut = new TodoController(_todoServiceMock.Object);
                    _todoItemResult = _sut.GetTodo(1).Result;
                }

                [TestMethod]
                public void It_Calls_ITodoService_GetTodoAsync_Method()
                {
                    // Assert
                    _todoServiceMock.Verify(s => s.GetTodoAsync(1), Times.Once);
                }

                [TestMethod]
                public void It_Returns_A_TodoItem()
                {
                    // Assert
                    Assert.IsInstanceOfType(_todoItemResult, typeof(ActionResult<TodoItem>));
                    Assert.IsNotNull(_todoItemResult.Value);
                }

                private ActionResult<TodoItem> _todoItemResult;
                private Mock<ITodoService> _todoServiceMock;
            }

            [TestClass]
            public class When_Calling_GetTodo_Method_That_Doesnt_Find_A_Record : TodoControllerContext
            {
                private ActionResult<TodoItem> _todoItemResult;
                private Mock<ITodoService> _todoServiceMock;

                protected override void Context()
                {
                    // Arrange
                    _todoServiceMock = new Mock<ITodoService>();
                    _sut = new TodoController(_todoServiceMock.Object);

                    // Act
                    _todoItemResult = _sut.GetTodo(1).Result;
                }

                [TestMethod]
                public void It_Calls_ITodoService_GetTodoAsync_Method()
                {
                    // Assert
                    _todoServiceMock.Verify(s => s.GetTodoAsync(1), Times.Once);
                }

                [TestMethod]
                public void It_Returns_NotFound_Result()
                {
                    // Assert
                    Assert.IsInstanceOfType(_todoItemResult.Result, typeof(NotFoundResult));
                }
            }

            [TestClass]
            public class When_Calling_Create_Method_With_No_Item : TodoControllerContext
            {
                protected override void Context()
                {
                    _sut = new TodoController(new Mock<ITodoService>().Object);
                }

                [TestMethod]
                public async Task It_Throws_A_ArgumentNullException()
                {
                    await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _sut.Create(default));
                }
            }

            [TestClass]
            public class When_Calling_Create_Method_With_A_Valid_Item : TodoControllerContext
            {
                protected override void Context()
                {
                    // Arrange
                    _todoServiceMock = new Mock<ITodoService>();
                    _todoServiceMock.Setup(s => s.CreateAsync(It.IsAny<TodoItem>()))
                                    .ReturnsAsync(new TodoItem { Id = 1 });
                    _sut = new TodoController(_todoServiceMock.Object);

                    // Act
                    _createResult = _sut.Create(new TodoItem()).Result;
                }

                [TestMethod]
                public void It_Returns_Create_And_Creates_TodoItem_In_Database()
                {
                    // Assert
                    Assert.IsInstanceOfType(_createResult, typeof(CreatedAtRouteResult));

                    var createdAtRouteResult = _createResult as CreatedAtRouteResult;
                    Assert.AreEqual("GetTodo", createdAtRouteResult.RouteName);
                    Assert.AreEqual(1, createdAtRouteResult.RouteValues.Count);

                    var createdAtRouteValuesEnumerator = createdAtRouteResult.RouteValues.GetEnumerator();
                    createdAtRouteValuesEnumerator.MoveNext();
                    Assert.AreEqual("id", createdAtRouteValuesEnumerator.Current.Key);
                    Assert.AreEqual(1L, createdAtRouteValuesEnumerator.Current.Value);
                }

                private IActionResult _createResult;
                private Mock<ITodoService> _todoServiceMock;
            }

            [TestClass]
            public class When_Calling_Update_Method_Returns_Updated_NotFound : TodoControllerContext
            {
                protected override void Context()
                {
                    _todoServiceMock = new Mock<ITodoService>();
                    _todoServiceMock.Setup(s => s.UpdateAsync(1, It.IsAny<TodoItem>()))
                                   .ReturnsAsync(Updated.NotFound);
                    _sut = new TodoController(_todoServiceMock.Object);
                    _notFoundResult = _sut.Update(1, new TodoItem { Id = 1 }).Result;
                }

                [TestMethod]
                public void It_Returns_A_NotFoundResult()
                {
                    Assert.IsInstanceOfType(_notFoundResult, typeof(NotFoundResult));
                }

                [TestMethod]
                public void It_Calls_TodoService_UpdateAsync_Method_Once()
                {
                    _todoServiceMock.Verify(s => s.UpdateAsync(1, It.IsAny<TodoItem>()), Times.Once);
                }

                private Mock<ITodoService> _todoServiceMock;
                private IActionResult _notFoundResult;
            }

            [TestClass]
            public class When_Calling_Update_Method_Returns_Updated_Invalid : TodoControllerContext
            {
                protected override void Context()
                {
                    _todoServiceMock = new Mock<ITodoService>();
                    _todoServiceMock.Setup(s => s.UpdateAsync(1, It.IsAny<TodoItem>()))
                                   .ReturnsAsync(Updated.Invalid);
                    _sut = new TodoController(_todoServiceMock.Object);
                    _badRequestResult = _sut.Update(1, new TodoItem { Id = 2 }).Result;
                }

                [TestMethod]
                public void It_Returns_A_BadRequestResult()
                {
                    Assert.IsInstanceOfType(_badRequestResult, typeof(BadRequestResult));
                }

                [TestMethod]
                public void It_Calls_TodoService_UpdateAsync_Method_Once()
                {
                    _todoServiceMock.Verify(s => s.UpdateAsync(1, It.IsAny<TodoItem>()), Times.Once);
                }

                private Mock<ITodoService> _todoServiceMock;
                private IActionResult _badRequestResult;
            }

            [TestClass]
            public class When_Calling_Update_Method_Returns_Updated_Accepted : TodoControllerContext
            {
                protected override void Context()
                {
                    _todoServiceMock = new Mock<ITodoService>();
                    _sut = new TodoController(_todoServiceMock.Object);
                    _noContentResult = _sut.Update(1, new TodoItem { Id = 1, Name = "new name", IsComplete = true }).Result;
                }

                [TestMethod]
                public void It_Updates_Database_Record_And_Returns_A_NoContent_Result()
                {
                    Assert.IsInstanceOfType(_noContentResult, typeof(NoContentResult));
                }

                [TestMethod]
                public void It_Calls_TodoService_UpdateAsync_Method_Once()
                {
                    _todoServiceMock.Verify(s => s.UpdateAsync(1, It.Is<TodoItem>(i => i.Id == 1 && i.Name == "new name" && i.IsComplete)), Times.Once);
                }

                private Mock<ITodoService> _todoServiceMock;
                private IActionResult _noContentResult;
            }

            [TestClass]
            public class When_Calling_Delete_Method_With_A_NonExisting_Id : TodoControllerContext
            {
                protected override void Context()
                {
                    _todoServiceMock = new Mock<ITodoService>();
                    _todoServiceMock.Setup(s => s.DeleteAsync(2))
                                    .ReturnsAsync(default(TodoItem));
                    _sut = new TodoController(_todoServiceMock.Object);
                    _deleteResult = _sut.Delete(2).Result;
                }

                [TestMethod]
                public void It_Returns_NotFound_Result()
                {
                    Assert.IsInstanceOfType(_deleteResult.Result, typeof(NotFoundResult));
                    Assert.IsNull(_deleteResult.Value);
                }

                [TestMethod]
                public void It_Calls_TodoService_DeleteAsync_Method_Once()
                {
                    _todoServiceMock.Verify(s => s.DeleteAsync(2), Times.Once);
                }

                private ActionResult<TodoItemDTO> _deleteResult;
                private Mock<ITodoService> _todoServiceMock;
            }

            [TestClass]
            public class When_Calling_Delete_Method_With_An_Existing_Id : TodoControllerContext
            {
                protected override void Context()
                {
                    _todoServiceMock = new Mock<ITodoService>();
                    _todoServiceMock.Setup(s => s.DeleteAsync(1))
                                    .ReturnsAsync(new TodoItem { Id = 1, Name = "name", IsComplete = true });
                    _sut = new TodoController(_todoServiceMock.Object);
                    _deleteResult = _sut.Delete(1).Result;
                }

                [TestMethod]
                public void It_Returns_An_ActionResult_Of_TodoItemDTO()
                {
                    Assert.IsInstanceOfType(_deleteResult, typeof(ActionResult<TodoItemDTO>));
                    Assert.IsNotNull(_deleteResult.Value);
                    var deletedItem = _deleteResult.Value;

                    Assert.AreEqual(1, deletedItem.Id);
                    Assert.AreEqual("name", deletedItem.Name);
                    Assert.AreEqual(true, deletedItem.IsComplete);
                }

                private ActionResult<TodoItemDTO> _deleteResult;
                private Mock<ITodoService> _todoServiceMock;
            }
        }
    }
}
