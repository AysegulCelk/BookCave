﻿using BookCave.Application.Abstracts.Repository;
using BookCave.BookCave.UI.Abstracts.Shop;
using BookCave.Application.Feature.Specifications;
using BookCave.BookCave.UI.ViewModels;
using BookCave.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookCave.BookCave.UI.Concretes.Shop
{
    public class AuthorService : IAuthorService
    {
        private readonly IRepository<Author> _repository;

        public AuthorService(IRepository<Author> repository)
        {
            _repository = repository;
        }

        public async Task<AuthorViewModel> GetAuthorViewModelAsync()
        {
            ShopAuthorFilterSpecification shopAuthorFilterSpec = new();

            List<Author> authors = await _repository.GetAllAsync(shopAuthorFilterSpec);

            return new()
            {
                AuthorSelects = authors.Select(x => new AuthorSelect()
                {
                    AuthorId = x.Id,
                    AuthorName = x.FullName
                }).ToList()
            };
        }
    }
}