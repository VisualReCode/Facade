﻿using System.Collections.Generic;
using Facade.Data;

namespace Facade.Models.Admin
{
    public class AddProductViewModel
    {
        public List<Category>? Categories { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int? CategoryId { get; set; }
    }
}