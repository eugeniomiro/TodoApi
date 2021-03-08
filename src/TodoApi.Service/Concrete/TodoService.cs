using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApi.Service.Concrete
{
    using DataAccess.Contract;
    using Domain.Models;

    public class TodoService
    {
        public TodoService(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        private readonly ITodoRepository _todoRepository;

        public async Task<IEnumerable<TodoItem>> GetAllAsync()
        {
            return await _todoRepository.GetAllAsync();
        }

        public async Task<TodoItem> GetTodoAsync(long id)
        {
            return await _todoRepository.GetTodoAsync(id);
        }

        public async Task<TodoItem> CreateAsync(TodoItem todoItem)
        {
            return await _todoRepository.CreateAsync(todoItem);
        }

        public async Task<TodoItem> UpdateAsync(long id, TodoItem todoItem)
        {
            return await _todoRepository.UpdateAsync(id, todoItem);
        }

        public async Task<TodoItem> DeleteAsync(long id)
        {
            return await _todoRepository.DeleteAsync(id);
        }
    }
}
