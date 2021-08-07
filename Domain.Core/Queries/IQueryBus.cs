﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core.Queries
{
    public interface IQueryBus
    {
        Task<TResponse> Send<TQuery, TResponse>(TQuery query) where TQuery : IQuery<TResponse>;
    }
}
