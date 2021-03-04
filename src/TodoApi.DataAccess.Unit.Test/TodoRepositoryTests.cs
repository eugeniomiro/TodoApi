using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleSpec.Bdd;
using TodoApi.Domain.Models;

namespace TodoApi.DataAccess.Unit.Test
{
    public static class TodoRepositoryTests 
    {
        [TestClass]
        public class TodoRepositoryContext : ContextSpecification
        {
            [TestMethod]
            public void Sut_Can_Be_Constructed()
            {
                var sut = new TodoRepository(null);
            }

            [TestMethod]
            public async Task When_GetAll_Method_Is_Called_From_Sut_It_Returns_A_The_Entire_List_Of_TodoItems()
            {
                var sut = new TodoRepository(new List<TodoItem>{
                    new TodoItem { Id = 1, Name = "name", IsComplete = false }
                });
                var list = await sut.GetAllAsync();
                Assert.IsNotNull(list);
                Assert.AreEqual(1, list.Count());
            }
        }
    }
}
