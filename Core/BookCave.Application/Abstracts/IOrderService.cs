﻿using BookCave.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCave.Application.Abstracts
{
    public interface IOrderService
    {
        Task<Order> AddOrderAsync(int cartId, Contact contact);
    }
}