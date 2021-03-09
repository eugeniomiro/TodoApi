using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleSpec.Bdd;

namespace TodoApi.Unit.Test.Controllers
{
    using DataAccess;
    using Domain.Models;
    using Service.Contract;
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
                    Assert.ThrowsException<ArgumentNullException>(() => new TodoController(default(TodoContext), default(ITodoService)));
                }
            }

            [TestClass]
            public class When_Calling_GetAll_Method : TodoControllerContext
            {
                protected override void Context()
                {
                    _todoServiceMock = new Mock<ITodoService>();
                    _sut = new TodoController(default(TodoContext), _todoServiceMock.Object);
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
                    _sut = new TodoController(default(TodoContext), _todoServiceMock.Object);
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
                    _sut = new TodoController(default(TodoContext), _todoServiceMock.Object);

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
                [ClassInitialize]
                public static void ClassInitialize(TestContext _)
                {
                    _globalDbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                                                    .UseInMemoryDatabase(databaseName: "TodoContext Create tests")
                                                    .Options;
                    using (var dbcontext = new TodoContext(_globalDbContextOptions))
                    {
                        dbcontext.Database.EnsureDeleted();
                        dbcontext.Database.EnsureCreated();
                    }
                }

                protected override void Context()
                {
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions), new Mock<ITodoService>().Object);
                }

                [TestMethod]
                public void It_Throws_A_NullReferenceException()
                {
                    Assert.ThrowsExceptionAsync<NullReferenceException>(() => _sut.Create(default(TodoItem)));
                }

                private static DbContextOptions<TodoContext> _globalDbContextOptions;
            }

            [TestClass]
            public class When_Calling_Create_Method_With_A_Valid_Item : TodoControllerContext
            {
                [ClassInitialize]
                public static void ClassInitialize(TestContext _)
                {
                    _globalDbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                                                    .UseInMemoryDatabase(databaseName: "TodoContext Create tests")
                                                    .Options;
                    using (var dbcontext = new TodoContext(_globalDbContextOptions))
                    {
                        dbcontext.Database.EnsureDeleted();
                        dbcontext.Database.EnsureCreated();
                    }
                }

                protected override void Context()
                {
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions), new Mock<ITodoService>().Object);
                    _createResult = _sut.Create(new TodoItem { Name = "othertodo", IsComplete = false }).Result;
                }

                [TestMethod]
                public void It_Returns_Create_And_Creates_TodoItem_In_Database()
                {
                    Assert.IsInstanceOfType(_createResult, typeof(CreatedAtRouteResult));
                    var createdAtRouteResult = _createResult as CreatedAtRouteResult;
                    Assert.AreEqual("GetTodo", createdAtRouteResult.RouteName);
                    Assert.AreEqual(1, createdAtRouteResult.RouteValues.Count);
                    var createdAtRouteValuesEnumerator = createdAtRouteResult.RouteValues.GetEnumerator();
                    createdAtRouteValuesEnumerator.MoveNext();
                    Assert.AreEqual("id", createdAtRouteValuesEnumerator.Current.Key);
                    Assert.AreEqual(1L, createdAtRouteValuesEnumerator.Current.Value);
                    using (var dbcontext = new TodoContext(_globalDbContextOptions))
                    {
                        Assert.AreEqual(1, dbcontext.TodoItems.Count());
                        var todoItem = dbcontext.TodoItems.First();
                        Assert.AreEqual(1, todoItem.Id);
                        Assert.AreEqual("othertodo", todoItem.Name);
                        Assert.AreEqual(false, todoItem.IsComplete);
                    }
                }

                private static DbContextOptions<TodoContext> _globalDbContextOptions;
                private IActionResult _createResult;
            }

            [TestClass]
            public class Wnen_Calling_Update_Method_With_An_Id_And_Null_Item : TodoControllerContext
            {
                [ClassInitialize]
                public static void ClassInitialize(TestContext _)
                {
                    _globalDbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                                                    .UseInMemoryDatabase(databaseName: "TodoContext Update tests")
                                                    .Options;
                    using (var dbcontext = new TodoContext(_globalDbContextOptions))
                    {
                        dbcontext.Database.EnsureDeleted();
                        dbcontext.Database.EnsureCreated();
                    }
                }

                protected override void Context()
                {
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions), new Mock<ITodoService>().Object);
                }

                [TestMethod]
                public void It_Throws_A_NullReferenceException()
                {
                    Assert.ThrowsExceptionAsync<NullReferenceException>(() => _sut.Update(2, default(TodoItem)));
                }

                private static DbContextOptions<TodoContext> _globalDbContextOptions;
            }

            [TestClass]
            public class Wnen_Calling_Update_Method_With_An_Id_And_An_Empty_Item : TodoControllerContext
            {
                [ClassInitialize]
                public static void ClassInitialize(TestContext _)
                {
                    _globalDbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                                                    .UseInMemoryDatabase(databaseName: "TodoContext Update tests")
                                                    .Options;
                    using (var dbcontext = new TodoContext(_globalDbContextOptions))
                    {
                        dbcontext.Database.EnsureDeleted();
                        dbcontext.Database.EnsureCreated();
                    }
                }

                protected override void Context()
                {
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions), new Mock<ITodoService>().Object);
                }

                [TestMethod]
                public void It_Returns_A_BadRequest_Result()
                {
                    var badRequestResult = _sut.Update(2, new TodoItem()).Result;
                    Assert.IsInstanceOfType(badRequestResult, typeof(BadRequestResult));
                }

                private static DbContextOptions<TodoContext> _globalDbContextOptions;
            }

            [TestClass]
            public class Wnen_Calling_Update_Method_With_A_NonExisting_Id_And_An_Empty_Item_With_The_Same_Id : TodoControllerContext
            {
                [ClassInitialize]
                public static void ClassInitialize(TestContext _)
                {
                    _globalDbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                                                    .UseInMemoryDatabase(databaseName: "TodoContext Update tests")
                                                    .Options;
                    using (var dbcontext = new TodoContext(_globalDbContextOptions))
                    {
                        dbcontext.Database.EnsureDeleted();
                        dbcontext.Database.EnsureCreated();
                    }
                }

                protected override void Context()
                {
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions), new Mock<ITodoService>().Object);
                }

                [TestMethod]
                public void It_Returns_A_NotFound_Result()
                {
                    var notFoundResult = _sut.Update(1, new TodoItem { Id = 1 }).Result;
                    Assert.IsInstanceOfType(notFoundResult, typeof(NotFoundResult));
                }

                private static DbContextOptions<TodoContext> _globalDbContextOptions;
            }

            [TestClass]
            public class Wnen_Calling_Update_Method_With_An_Existing_Id_And_An_Empty_Item_With_Different_Id : TodoControllerContext
            {
                [ClassInitialize]
                public static void ClassInitialize(TestContext _)
                {
                    _globalDbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                                                    .UseInMemoryDatabase(databaseName: "TodoContext Update tests")
                                                    .Options;
                    using (var dbcontext = new TodoContext(_globalDbContextOptions))
                    {
                        dbcontext.Database.EnsureDeleted();
                        dbcontext.Database.EnsureCreated();
                    }
                }

                protected override void Context()
                {
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions), new Mock<ITodoService>().Object);
                }

                [TestMethod]
                public void It_Returns_A_NotFound_Result()
                {
                    var badRequestResult = _sut.Update(1, new TodoItem { Id = 2 }).Result;
                    Assert.IsInstanceOfType(badRequestResult, typeof(BadRequestResult));
                }

                private static DbContextOptions<TodoContext> _globalDbContextOptions;
            }

            [TestClass]
            public class Wnen_Calling_Update_Method_With_An_Existing_Id_And_An_Empty_Item_With_The_Same_Id : TodoControllerContext
            {
                [ClassInitialize]
                public static void ClassInitialize(TestContext _)
                {
                    _globalDbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                                                    .UseInMemoryDatabase(databaseName: "TodoContext Update tests")
                                                    .Options;
                    using (var dbcontext = new TodoContext(_globalDbContextOptions))
                    {
                        dbcontext.Database.EnsureDeleted();
                        dbcontext.Database.EnsureCreated();
                        dbcontext.TodoItems.Add(new TodoItem { Id = 1, Name = "name", IsComplete = false });
                        dbcontext.SaveChanges();
                    }
                }

                protected override void Context()
                {
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions), new Mock<ITodoService>().Object);
                }

                [TestMethod]
                public void It_Updates_Database_Record_And_Returns_A_NoContent_Result()
                {
                    var noContentResult = _sut.Update(1, new TodoItem { Id = 1, Name = "new name", IsComplete = true }).Result;
                    Assert.IsInstanceOfType(noContentResult, typeof(NoContentResult));

                    // record in database was updated
                    using (var dbContext = new TodoContext(_globalDbContextOptions))
                    {
                        var record = dbContext.TodoItems.Find(1L);
                        Assert.AreEqual(1, record.Id);
                        Assert.AreEqual("new name", record.Name);
                        Assert.AreEqual(true, record.IsComplete);
                    }
                }

                private static DbContextOptions<TodoContext> _globalDbContextOptions;
            }

            [TestClass]
            public class When_Calling_Delete_Method_With_A_NonExisting_Id : TodoControllerContext
            {
                [ClassInitialize]
                public static void ClassInitialize(TestContext _)
                {
                    _globalDbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                                                    .UseInMemoryDatabase(databaseName: "TodoContext Delete tests")
                                                    .Options;
                    using (var dbcontext = new TodoContext(_globalDbContextOptions))
                    {
                        dbcontext.Database.EnsureDeleted();
                        dbcontext.Database.EnsureCreated();
                        dbcontext.TodoItems.Add(new TodoItem { Id = 1, Name = "name", IsComplete = false });
                        dbcontext.SaveChanges();
                    }
                }

                protected override void Context()
                {
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions), new Mock<ITodoService>().Object);
                    _deleteResult = _sut.Delete(2).Result;
                }

                [TestMethod]
                public void It_Returns_NotFound_Result()
                {
                    Assert.IsInstanceOfType(_deleteResult.Result, typeof(NotFoundResult));
                    Assert.IsNull(_deleteResult.Value);
                }

                private static DbContextOptions<TodoContext> _globalDbContextOptions;
                private ActionResult<TodoItemDTO> _deleteResult;
            }

            [TestClass]
            public class When_Calling_Delete_Method_With_An_Existing_Id : TodoControllerContext
            {
                [ClassInitialize]
                public static void ClassInitialize(TestContext _)
                {
                    _globalDbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                                                    .UseInMemoryDatabase(databaseName: "TodoContext Delete tests")
                                                    .Options;
                    using (var dbcontext = new TodoContext(_globalDbContextOptions))
                    {
                        dbcontext.Database.EnsureDeleted();
                        dbcontext.Database.EnsureCreated();
                        dbcontext.TodoItems.Add(new TodoItem { Id = 1, Name = "name", IsComplete = false });
                        dbcontext.SaveChanges();
                    }
                }

                protected override void Context()
                {
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions), new Mock<ITodoService>().Object);
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
                    Assert.AreEqual(false, deletedItem.IsComplete);

                    // item was deleted from database
                    using (var dbContext = new TodoContext(_globalDbContextOptions))
                    {
                        Assert.IsNull(dbContext.TodoItems.Find(1L));                        
                    }
                }

                private static DbContextOptions<TodoContext> _globalDbContextOptions;
                private ActionResult<TodoItemDTO> _deleteResult;
            }
        }
    }
}
