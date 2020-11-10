using BookStoreMVC.DataAccess.Data;
using BookStoreMVC.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreMVC.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public ICategoryRepository Category { get; private set; }
        public ICoverTypeRepository CoverType { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IStoredProcedure_Call StoredProcedure_Call { get; private set; }


        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            Category = new CategoryRepository(_dbContext);
            CoverType = new CoverTypeRepository(_dbContext);
            Product = new ProductRepository(_dbContext);
            Company = new CompanyRepository(_dbContext);
            ApplicationUser = new ApplicationUserRepository(_dbContext);
            StoredProcedure_Call = new StoredProcedure_Call(_dbContext);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
