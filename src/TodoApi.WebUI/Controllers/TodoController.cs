using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TodoApi.WebUI.Controllers
{
    using Domain.Models;
    using Service.Contract;
    using TodoApi.Domain.SumTypes;

    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService = default)
        {
            _todoService = todoService ?? throw new ArgumentNullException(nameof(todoService));
        }
                
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetAll()
        {
            return Ok((await _todoService.GetAllAsync()).Select(x => ItemToDTO(x)));
        }

        [HttpGet("{id}", Name = nameof(GetTodo))]
        public async Task<ActionResult<TodoItem>> GetTodo(long id)
        {
            var item = await _todoService.GetTodoAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        [HttpPost]
        public async Task<IActionResult> Create(TodoItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item = await _todoService.CreateAsync(item);

            return CreatedAtRoute(nameof(GetTodo), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, TodoItem item)
        {
            var updatedTodo = await _todoService.UpdateAsync(id, item);

            switch (updatedTodo)
            {
                case Updated<TodoItem>.Invalid:
                    return BadRequest();
                case Updated<TodoItem>.NotFound:
                    return NotFound();
                default:
                    return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItemDTO>> Delete(long id)
        {
            var todo = await _todoService.DeleteAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            return ItemToDTO(todo);
        }

        private static TodoItemDTO ItemToDTO(TodoItem item)
        {
            return new TodoItemDTO
            {
                Id = item.Id,
                Name = item.Name,
                IsComplete = item.IsComplete
            };
        }
    }
}
