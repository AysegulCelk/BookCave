﻿using BookCave.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookCave.UI.Abstracts.Cart
{
    public interface ICartViewModelService
    {
        Task<CartViewModel> GetCartViewModel();
        Task<CartViewModel> AddToCartAsync(string isbn, int quantity);
        Task<int> GetCartLinesCountAsync();
        Task<decimal> GetTotalPriceCartLinesAsync();
        Task RemoveCartLineAsync(int cartLineId);
        Task RemoveCartAsync();
        Task<CartViewModel> UpdateCartAsync(Dictionary<int, int> quantities);
    }
}