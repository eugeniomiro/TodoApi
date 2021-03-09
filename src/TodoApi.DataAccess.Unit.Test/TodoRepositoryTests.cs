using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleSpec.Bdd;

namespace TodoApi.DataAccess.Unit.Test
{
    using Domain.Models;
    using TodoApi.DataAccess.Concrete;
    using TodoApi.Domain.SumTypes;

    public static class TodoRepositoryTests 
    {
        [TestClass]
        public class TodoRepositoryContext : ContextSpecification
        {
            [TestMethod]
            public void Sut_Cannot_Be_Constructed_With_Null_DbContext()
            {
                Assert.ThrowsException<ArgumentNullException>(() => new TodoRepository(null));
            }

            [TestMethod]
            public async Task When_GetAll_Method_Is_Called_From_Sut_It_Returns_A_The_Entire_List_Of_TodoItems()
            {
                // Arrange
                var sut = await BuildSutWithOneRecord();

                // Act
                var list = await sut.GetAllAsync();

                // Assert
                Assert.IsNotNull(list);
                Assert.AreEqual(1, list.Count());
            }

            [TestMethod]
            public async Task When_GetTodo_Method_Is_Called_With_An_Existent_Id_From_Sut_It_Returns_The_Found_Item()
            {
                // Arrange
                var sut = await BuildSutWithOneRecord();

                // Act
                var item = await sut.GetTodoAsync(1);

                // Assert
                Assert.IsNotNull(item);
                Assert.AreEqual(1, item.Id);
                Assert.AreEqual("name", item.Name);
                Assert.AreEqual(false, item.IsComplete);
            }

            [TestMethod]
            public async Task When_GetTodo_Method_Is_Called_With_A_Non_Existent_Id_From_Sut_It_Returns_Null()
            {
                // Arrange
                var sut = await BuildSutWithOneRecord();

                // Act
                var item = await sut.GetTodoAsync(3);

                // Assert
                Assert.IsNull(item);
            }

            [TestMethod]
            public async Task When_Create_Method_Is_Called_From_Sut_With_A_Non_Null_TodoItem_It_Returns_The_New_TodoItem()
            {
                // Arrange
                var sut = await BuildSutWithOneRecord();

                // Act
                var newRecord = await sut.CreateAsync(new TodoItem { Id = 0, Name = "new todo", IsComplete = true });

                // Assert
                Assert.IsNotNull(newRecord);
                Assert.AreEqual(2, newRecord.Id);
                Assert.AreEqual("new todo", newRecord.Name);
                Assert.AreEqual(true, newRecord.IsComplete);
            }

            [TestMethod]
            public async Task When_Create_Method_Is_Called_From_Sut_With_A_Null_TodoItem_It_Throws_ArgumentNullException()
            {
                // Arrange
                var sut = await BuildSutWithOneRecord();

                // Act & Assert
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => sut.CreateAsync(default));
            }

            [TestMethod]
            public async Task When_Update_Method_Is_Called_From_Sut_With_A_Null_TodoItem_It_Throws_ArgumentNullException()
            {
                // Arrange
                var sut = await BuildSutWithOneRecord();

                // Act & Assert
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => sut.UpdateAsync(0, default));
            }

            [TestMethod]
            public async Task When_Update_Method_Is_Called_From_Sut_With_A_Non_Null_TodoItem_With_Non_Matching_Id_It_Returns_UpdatedTodoItem_Invalid()
            {
                // Arrange
                var sut = await BuildSutWithOneRecord();

                // Act
                var updatedTodoItem = await sut.UpdateAsync(1, new TodoItem());

                // Assert
                Assert.IsInstanceOfType(updatedTodoItem, typeof(Updated<TodoItem>.Invalid));
            }

            [TestMethod]
            public async Task When_Update_Method_Is_Called_From_Sut_With_A_Non_Null_TodoItem_With_Matching_Id_Non_Existing_In_Database_It_Returns_UpdatedTodoItem_NotFound()
            {
                // Arrange
                var sut = await BuildSutWithOneRecord();

                // Act
                var updatedTodoItem = await sut.UpdateAsync(3, new TodoItem { Id = 3, Name = "not found name", IsComplete = true });

                // Assert 
                Assert.IsInstanceOfType(updatedTodoItem, typeof(Updated<TodoItem>.NotFound));
            }

            [TestMethod]
            public async Task When_Update_Method_Is_Called_From_Sut_With_A_Non_Null_TodoItem_With_Matching_Id_Existing_In_Database_It_Updates_And_Returns_Found_Record()
            {
                // Arrange
                var sut = await BuildSutWithOneRecord();

                // Act 
                var returnedTodoItem = await sut.UpdateAsync(1, new TodoItem { Id = 1, Name = "new name", IsComplete = true });

                // Assert
                Assert.IsInstanceOfType(returnedTodoItem, typeof(Updated<TodoItem>.Accepted));
                returnedTodoItem.TryGetValue(out var todoItem);
                Assert.AreEqual(1, todoItem.Id);
                Assert.AreEqual("new name", todoItem.Name);
                Assert.AreEqual(true, todoItem.IsComplete);
            }

            [TestMethod]
            public async Task When_Delete_Method_Is_Called_From_Sut_With_A_Non_Existing_Id_It_Throws_KeyNotFoundException()
            {
                // Arrange
                var sut = await BuildSutWithOneRecord();

                // Act & Assert 
                await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => sut.DeleteAsync(3));
            }

            [TestMethod]
            public async Task When_Delete_Method_Is_Called_From_Sut_With_An_Existing_Id_It_Throws_KeyNotFoundException()
            {
                // Arrange
                var sut = await BuildSutWithOneRecord();

                // Act 
                await sut.DeleteAsync(1);

                // Assert
            }

            protected override void Context()
            {
                _dbContextConfig = new DbContextOptionsBuilder<TodoContext>()
                                            .UseInMemoryDatabase("TodoRepository tests")
                                            .Options;

                using var dbContext = new TodoContext(_dbContextConfig);
                dbContext.Database.EnsureCreated();
                dbContext.Database.EnsureDeleted();
            }

            private async Task<TodoRepository> BuildSutWithOneRecord()
            {
                using (var dbContext = new TodoContext(_dbContextConfig))
                {
                    await dbContext.TodoItems.AddAsync(new TodoItem { Id = 1, Name = "name", IsComplete = false });
                    await dbContext.SaveChangesAsync();
                }
                return BuildSut();
            }

            private TodoRepository BuildSut()
            {
                return new TodoRepository(new TodoContext(_dbContextConfig));
            }

            private DbContextOptions<TodoContext> _dbContextConfig;
        }
    }
}
