﻿using MultiTenancy.Entities;

namespace MultiTenancy.Models {
    public class HomeIndexViewModel {
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<Country> Countries { get; set; } = new List<Country>();
    }
}