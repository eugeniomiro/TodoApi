using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApi.DataAccess
{
    using Domain.Models;

    public class TodoRepository
    {
        public TodoRepository(List<TodoItem> items)
        {
            _items = items;
        }
        public async Task<IEnumerable<TodoItem>> GetAllAsync()
        {
            return await Task.Run(() => _items);
        }

        private readonly List<TodoItem> _items;
    }
}