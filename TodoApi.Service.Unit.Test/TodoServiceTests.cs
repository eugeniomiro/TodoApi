using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleSpec.Bdd;

namespace TodoApi.Service.Unit.Test
{
    using DataAccess;
    using DataAccess.Concrete;

    public static class TodoServiceTests
    {
        [TestClass]
        public class TodoServiceContext: ContextSpecification
        {
            [TestMethod]
            public void Can_Create_TodoService()
            {
                new TodoService(new TodoRepository(new TodoContext(new DbContextOptionsBuilder<TodoContext>()
                                                                        .UseInMemoryDatabase("TodoService tests")
                                                                        .Options)));
            }
        }
    }
}
