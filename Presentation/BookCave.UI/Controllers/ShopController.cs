﻿using BookCave.Application.Enums;
using BookCave.UI.Abstracts;
using BookCave.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookCave.UI.Controllers
{
    public class ShopController : Controller
    {
        private readonly IShopViewModelService _shopService;

        public ShopController(IShopViewModelService shopService)
        {
            _shopService = shopService;
        }

        public async Task<IActionResult> Index(
            List<AuthorViewModel> authors,
            List<PublisherViewModel> publishers,
            int? categoryId,
            decimal minPrice, decimal? maxPrice,
            OrderType orderType,
            string keyword,
            SearchType searchType,
            byte pageNumber = 1)
        {
            var model = await _shopService.GetShopViewModelAsync(authors, publishers, categoryId, minPrice, maxPrice, orderType, keyword, searchType, pageNumber);
            return View(model);
        }
    }
}
