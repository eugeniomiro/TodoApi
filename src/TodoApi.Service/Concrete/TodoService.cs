using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.DataAccess.Contract;
using TodoApi.Domain.Models;

namespace TodoApi.Service.Concrete
{
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
    }
}
