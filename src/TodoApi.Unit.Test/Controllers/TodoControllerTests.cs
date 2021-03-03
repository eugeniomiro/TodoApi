using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Unit.Test.Controllers
{
    using Bdd;
    using Models;
    using TodoApi;
    using TodoApi.Controllers;

    public static class TodoControllerTests 
    {
        public class TodoControllerContext : ContextSpecification
        {
            protected TodoController _sut;
            
            [TestClass]
            public class When_Constructor_Is_Called_With_Null_Parameter : TodoControllerContext
            {
                protected override void Context()
                {
                    _sut = new TodoController(default(TodoContext));
                }

                [TestMethod]
                public void Calling_GetAll_Method__Throws_NullReferenceException()
                {
                    Assert.ThrowsExceptionAsync<NullReferenceException>(() => _sut.GetAll());
                }
            }

            [TestClass]
            public class When_Calling_GetAll_Method_Having_One_TodoItem_In_Database : TodoControllerContext
            {
                [ClassInitialize]
                public static void ClassInitialize(TestContext _)
                {
                    _globalDbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                                                    .UseInMemoryDatabase(databaseName: "TodoContext GetAll tests")
                                                    .Options;
                    using (var dbcontext = new TodoContext(_globalDbContextOptions))
                    {
                        dbcontext.Database.EnsureDeleted();
                        dbcontext.Database.EnsureCreated();

                        dbcontext.TodoItems.Add(new TodoItem { Id = 1, Name= "my todo", IsComplete = false });
                        dbcontext.SaveChanges();
                    }
                }

                protected override void Context()
                {
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions));
                    _getAllResult = _sut.GetAll().Result.Value;
                }

                [TestMethod]
                public void Count_Equals_One()
                {
                    Assert.AreEqual(1, _getAllResult.Count());
                }

                [TestMethod]
                public void First_Record_Returned_Has_Saved_Data()
                {
                    var record = _getAllResult.First();
                    Assert.AreEqual(1, record.Id);
                    Assert.AreEqual("my todo", record.Name);
                    Assert.AreEqual(false, record.IsComplete);
                }

                private static DbContextOptions<TodoContext> _globalDbContextOptions;
                private IEnumerable<TodoItemDTO> _getAllResult;
            }

            [TestClass]
            public class When_Calling_GetTodo_Method_With_An_Existing_Id : TodoControllerContext
            {
                [ClassInitialize]
                public static void ClassInitialize(TestContext _)
                {
                    _globalDbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                                                    .UseInMemoryDatabase(databaseName: "TodoContext GetTodo tests")
                                                    .Options;
                    using (var dbcontext = new TodoContext(_globalDbContextOptions))
                    {
                        dbcontext.Database.EnsureDeleted();
                        dbcontext.Database.EnsureCreated();

                        dbcontext.TodoItems.Add(new TodoItem { Id = 1, Name = "my todo", IsComplete = false });
                        dbcontext.SaveChanges();
                    }
                }

                protected override void Context()
                {
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions));
                    _getTodoResult = _sut.GetTodo(1).Result;
                }

                [TestMethod]
                public void It_Returns_Available_Record()
                {
                    Assert.IsNotNull(_getTodoResult.Value);
                    var todoItem = _getTodoResult.Value;
                    Assert.AreEqual(1, todoItem.Id);
                    Assert.AreEqual("my todo", todoItem.Name);
                    Assert.AreEqual(false, todoItem.IsComplete);
                }

                private static DbContextOptions<TodoContext> _globalDbContextOptions;
                private ActionResult<TodoItem> _getTodoResult;
            }

            [TestClass]
            public class When_Calling_GetTodo_Method_With_A_Nonexisting_Id : TodoControllerContext
            {
                [ClassInitialize]
                public static void ClassInitialize(TestContext _)
                {
                    _globalDbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                                                    .UseInMemoryDatabase(databaseName: "TodoContext GetTodo tests")
                                                    .Options;
                    using (var dbcontext = new TodoContext(_globalDbContextOptions))
                    {
                        dbcontext.Database.EnsureDeleted();
                        dbcontext.Database.EnsureCreated();

                        dbcontext.TodoItems.Add(new TodoItem { Id = 1, Name = "my todo", IsComplete = false });
                        dbcontext.SaveChanges();
                    }
                }

                protected override void Context()
                {
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions));
                    _getTodoResult = _sut.GetTodo(2).Result;
                }

                [TestMethod]
                public void It_Returns_NotFound()
                {
                    Assert.IsInstanceOfType(_getTodoResult.Result, typeof(StatusCodeResult));
                    var statusCodeResult = _getTodoResult.Result as StatusCodeResult;
                    Assert.AreEqual(404, statusCodeResult.StatusCode);
                    Assert.IsNull(_getTodoResult.Value);
                }

                private static DbContextOptions<TodoContext> _globalDbContextOptions;
                private ActionResult<TodoItem> _getTodoResult;
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
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions));
                }

                [TestMethod]
                public void It_Throws_A_NullReferenceException()
                {
                    Assert.ThrowsExceptionAsync<NullReferenceException>(() => _sut.Create(default(TodoItem)));
                }

                private static DbContextOptions<TodoContext> _globalDbContextOptions;
                private IActionResult _createResult;
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
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions));
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
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions));
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
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions));
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
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions));
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
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions));
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
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions));
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
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions));
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
                    _sut = new TodoController(new TodoContext(_globalDbContextOptions));
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
