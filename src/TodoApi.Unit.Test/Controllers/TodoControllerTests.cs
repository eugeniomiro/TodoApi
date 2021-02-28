using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TodoApi.Unit.Test.Controllers
{
    using Bdd;
    using Models;
    using TodoApi.Controllers;

    public static class TodoControllerTests 
    {
        public class TodoControllerContext : ContextSpecification
        {
            protected TodoController _sut;
            
            protected override void Context()
            {
                _sut = new TodoController(default(TodoContext));
            }

            [TestClass]
            public class When_Constructor_Is_Called_With_Null_Parameter : TodoControllerContext
            {
                [TestMethod]
                public void Calling_GetAll_Method__Throws_Exception()
                {
                    try
                    {
                        _sut.GetAll().Wait();
                    }
                    catch (System.AggregateException)
                    {                        
                        return;
                    }
                    Assert.Fail();
                }
            }
        }
    }
}
