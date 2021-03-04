using TodoApi.DataAccess.Contract;

namespace TodoApi.Service
{
    public class TodoService
    {
        public TodoService(ITodoRepository todoRepository)
        {
            this.todoRepository = todoRepository;
        }

        private readonly ITodoRepository todoRepository;
    }
}
