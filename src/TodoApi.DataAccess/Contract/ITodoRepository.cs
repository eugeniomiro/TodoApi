﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApi.DataAccess.Contract
{
    using Domain.Models;
    using Domain.SumTypes;

    public interface ITodoRepository
    {
        Task<TodoItem> CreateAsync(TodoItem todoItem);
        Task<TodoItem> DeleteAsync(long id);
        Task<IEnumerable<TodoItem>> GetAllAsync();
        Task<TodoItem> GetTodoAsync(long id);
        Task<Updated<TodoItem>> UpdateAsync(long id, TodoItem todoItem);
    }
}
