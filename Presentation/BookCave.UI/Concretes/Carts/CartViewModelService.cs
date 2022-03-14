﻿using BookCave.Application.Abstracts;
using BookCave.Application.Abstracts.Repository;
using BookCave.Application.Feature.Specifications;
using BookCave.Domain.Entities;
using BookCave.UI.Abstracts.Cart;
using BookCave.UI.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookCave.UI.Concretes.Carts
{
    public class CartViewModelService : ICartViewModelService
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ICartService _cartService;

        public CartViewModelService(IRepository<Cart> cartRepository, IHttpContextAccessor httpContext, ICartService cartService)
        {
            _cartRepository = cartRepository;
            _httpContext = httpContext;
            _cartService = cartService;
        }

        public async Task<CartViewModel> GetCartViewModel()
        {
            return await GetCartVm();
        }


        private async Task<CartViewModel> GetCartVm()
        {
            int cartId = (await GetOrCreateCartAsync()).Id;
            var cart = await _cartRepository.FirstOrDefaultAsync(new CartSpecification(cartId));
            return CartToViewModel(cart);
        }

        public async Task<decimal> GetTotalPriceCartLinesAsync()
        {
            var cartVM = await GetCartVm();
            return cartVM.TotalPrice;
        }

        public async Task<int> GetCartLinesCountAsync()
        {
            var cartVM = await GetCartVm();
            return cartVM.TotalCartLines;
        }
        public async Task<CartViewModel> AddToCartAsync(string isbn, int quantity)
        {
            var cart = await GetOrCreateCartAsync();
            cart = await _cartService.AddCartLineToCart(isbn, quantity, cart.Id);

            return CartToViewModel(cart);
        }
        public async Task<CartViewModel> UpdateCartAsync(Dictionary<int, int> quantities)
        {
            var cartId = (await GetOrCreateCartAsync()).Id;
            var cart = await _cartService.SetQuantitiesAsync(cartId, quantities);
            return CartToViewModel(cart);
        }
        public async Task RemoveCartAsync()
        {
            var cartId = (await GetOrCreateCartAsync()).Id;
            await _cartService.RemoveCartAsync(cartId);
        }

        public async Task RemoveCartLineAsync(int cartLineId)
        {
            var cartId = (await GetOrCreateCartAsync()).Id;
            await _cartService.RemoveCartLineFromCartAsync(cartId, cartLineId);
        }

        private CartViewModel CartToViewModel(Cart cart)
        {
            return new()
            {
                CartId = cart.Id,
                CustomerId = cart.CustomerId,
                CartLines = cart.CartLines.Select(x => new CartLineViewModel()
                {
                    BookName = x.Book.Name,
                    ISBN = x.Book.ISBN,
                    UnitPrice = x.Book.UnitPrice,
                    PictureUri = x.Book.ImageUri,
                    Quantity = x.Quantity,
                    CartLineId = x.Id,
                    AuthorName = x.Book.Author.FullName
                }).ToList()
            };
        }

        private async Task<Cart> GetOrCreateCartAsync()
        {
            var userId = await GetOrCreateUserAsync();
            var cart = await _cartRepository.FirstOrDefaultAsync(new CartSpecification(userId));

            if (cart == null)
            {
                cart = new Cart()
                {
                    CustomerId = userId
                };
                await _cartRepository.AddAsync(cart);
            }
            return cart;
        }

        private async Task<string> GetOrCreateUserAsync()
        {
            string loggedUserId = GetUserLogged();
            if (!string.IsNullOrEmpty(loggedUserId)) return loggedUserId;

            string anonUserId = GetUserAnon();
            if (!string.IsNullOrEmpty(anonUserId)) return anonUserId;

            string newUserId = Guid.NewGuid().ToString();

            _httpContext.HttpContext.Response.Cookies.Append(Constants.COOKIE_NAME, newUserId, new CookieOptions()
            {
                IsEssential = true,
                Expires = DateTime.Now.AddDays(14)
            });

            return newUserId;
        }

        private string GetUserAnon() => _httpContext.HttpContext.Request.Cookies[Constants.COOKIE_NAME];

        private string GetUserLogged()
        {
            if (!_httpContext.HttpContext.User.Identity.IsAuthenticated)
                return null;

            return _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

    }
}
