﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreMVC.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        ICoverTypeRepository CoverType { get;  }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IStoredProcedure_Call StoredProcedure_Call { get; }
        void Save();
    }
}
