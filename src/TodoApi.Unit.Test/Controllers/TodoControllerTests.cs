using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    try
                    {
                        _sut.GetAll().Wait();
                    }
                    catch (AggregateException ae) when (ae.InnerException is NullReferenceException)
                    {                        
                        return;
                    }
                    Assert.Fail();
                }
            }

            [TestClass]
            public class When_Calling_GetAll_Method : TodoControllerContext
            {
                public static DbContextOptions<TodoContext> GlobalDbContextOptions { get; private set; }

                [ClassInitialize]
                public static void ClassInitialize(TestContext _)
                {
                    GlobalDbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                                                    .UseInMemoryDatabase(databaseName: "TodoContext test")
                                                    .Options;
                    using (var dbcontext = new TodoContext(GlobalDbContextOptions))
                    {
                        dbcontext.TodoItems.Add(new TodoItem { Id = 1, Name= "my todo", IsComplete = false });
                        dbcontext.SaveChanges();
                    }
                }

                public IEnumerable<TodoItemDTO> GetAllResult { get; private set; }

                protected override void Context()
                {
                    _sut = new TodoController(new TodoContext(GlobalDbContextOptions));
                    GetAllResult = _sut.GetAll().Result.Value;
                }

                [TestMethod]
                public void Count_Equals_One()
                {
                    Assert.AreEqual(1, GetAllResult.Count());
                }

                [TestMethod]
                public void First_Record_Returned_Has_Saved_Data()
                {
                    var record = GetAllResult.First();
                    Assert.AreEqual(1, record.Id);
                    Assert.AreEqual("my todo", record.Name);
                    Assert.AreEqual(false, record.IsComplete);
                }
            }
        }
    }
}
