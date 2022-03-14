﻿using BookCave.UI.Abstracts.Cart;
using BookCave.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookCave.UI.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartViewModelService _cartViewModelService;

        public CartController(ICartViewModelService cartViewModelService)
        {
            _cartViewModelService = cartViewModelService;
        }

        public async Task<IActionResult> Index()
        {
            CartViewModel model = await _cartViewModelService.GetCartViewModel();

            return View(model);
        }

        public async Task<IActionResult> AddBookToCart(string isbn, int quantity = 1)
        {
            var cartVm = await _cartViewModelService.AddToCartAsync(isbn, quantity);

            return Json(new NavCartViewModel() { TotalQuantityCartLines = cartVm.TotalCartLines, TotalPriceCartLine = cartVm.TotalPrice });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(Dictionary<int, int> quantities)
        {
            var cart = await _cartViewModelService.UpdateCartAsync(quantities);
            return RedirectToAction(nameof(Index));
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCartLine(int cartLineId)
        {
            await _cartViewModelService.RemoveCartLineAsync(cartLineId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove()
        {
            await _cartViewModelService.RemoveCartAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
