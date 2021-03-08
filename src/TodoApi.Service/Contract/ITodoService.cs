using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Domain.Models;

namespace TodoApi.Service.Contract
{
    public interface ITodoService
    {
        Task<TodoItem> CreateAsync(TodoItem todoItem);
        Task<TodoItem> DeleteAsync(long id);
        Task<IEnumerable<TodoItem>> GetAllAsync();
        Task<TodoItem> GetTodoAsync(long id);
        Task<TodoItem> UpdateAsync(long id, TodoItem todoItem);
    }
}
