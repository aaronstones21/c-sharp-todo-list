﻿using System;

namespace ToDoList.Model
{
    public class ToDo
    {

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public DateTime Timestamp { get; set; }

    }
}
