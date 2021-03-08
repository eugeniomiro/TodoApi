using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.DataAccess.Concrete
{
    using Contract;
    using Domain.Models;

    public class TodoRepository : ITodoRepository
    {
        public TodoRepository(TodoContext todoContext)
        {
            _todoContext = todoContext ?? throw new ArgumentNullException(nameof(todoContext));
        }

        public async Task<IEnumerable<TodoItem>> GetAllAsync()
        {
            return await _todoContext.TodoItems.ToListAsync();
        }

        public async Task<TodoItem> GetTodoAsync(long id)
        {
            return await _todoContext.TodoItems.FindAsync(id);
        }

        public async Task<TodoItem> CreateAsync(TodoItem todoItem)
        {
            if (todoItem is null)
            {
                throw new ArgumentNullException(nameof(todoItem));
            }

            await _todoContext.TodoItems.AddAsync(todoItem);
            await _todoContext.SaveChangesAsync();

            return todoItem;
        }

        private readonly TodoContext _todoContext;

        public async Task<TodoItem> UpdateAsync(long id, TodoItem todoItem)
        {
            if (todoItem is null)
            {
                throw new ArgumentNullException(nameof(todoItem));
            }
            if (todoItem.Id != id)
            {
                throw new ArgumentException("id");
            }
            _todoContext.Entry(todoItem).State = EntityState.Modified;
            try
            {
                await _todoContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    throw new KeyNotFoundException("id");
                }
                throw;
            }
            return todoItem;
        }

        private bool TodoItemExists(long id)
        {
            return _todoContext.TodoItems.Any(e => e.Id == id);
        }

        public async Task<TodoItem> DeleteAsync(long id)
        {
            var todo = await _todoContext.TodoItems.FindAsync(id);
            if (todo == null)
            {
                throw new KeyNotFoundException("id");
            }
            _todoContext.TodoItems.Remove(todo);
            await _todoContext.SaveChangesAsync();
            return todo;
        }
    }
}
