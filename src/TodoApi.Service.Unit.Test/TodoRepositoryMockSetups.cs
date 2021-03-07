using System.Collections.Generic;
using Moq;
using TodoApi.DataAccess.Contract;
using TodoApi.Domain.Models;

namespace TodoApi.Service.Unit.Test
{
    internal static class TodoRepositoryMockSetups
    {
        internal static Mock<ITodoRepository> SetupGetAllAsyncToReturn(this Mock<ITodoRepository> todoRepositoryMock, List<TodoItem> todoItems)
        {
            todoRepositoryMock.Setup(r => r.GetAllAsync())
                              .ReturnsAsync(todoItems);
            return todoRepositoryMock;
        }

        internal static Mock<ITodoRepository> SetupGetTodoAsyncToReturnItem(this Mock<ITodoRepository> todoRepositoryMock, long expectedId, TodoItem expectedItem)
        {
            todoRepositoryMock.Setup(r => r.GetTodoAsync(expectedId))
                              .ReturnsAsync(expectedItem);
            return todoRepositoryMock;
        }

        internal static Mock<ITodoRepository> SetupCreateAsyncToReturnItem(this Mock<ITodoRepository> todoRepositoryMock)
        {
            todoRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<TodoItem>()))
                              .ReturnsAsync(new TodoItem());
            return todoRepositoryMock;
        }
    }
}
