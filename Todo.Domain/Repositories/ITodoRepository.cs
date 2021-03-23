﻿using System;
using System.Collections.Generic;
using System.Text;
using Todo.Domain.Entities;

namespace Todo.Domain.Repositories
{
    public interface ITodoRepository
    {
        void Create(TodoItem todo);

        void Update(TodoItem todo);

        TodoItem GetById(Guid id, string user);

        IEnumerable<TodoItem> GetAll(string user);

        IEnumerable<TodoItem> GetAllDone(string user);

        IEnumerable<TodoItem> GetAllUndone(string user);

        IEnumerable<TodoItem> GetByPeriod(string user,DateTime date,bool done);

        //IEnumerable - ele é bem mais leve do que o IList, porque tem menos método, por exemplos o add

    }
}
