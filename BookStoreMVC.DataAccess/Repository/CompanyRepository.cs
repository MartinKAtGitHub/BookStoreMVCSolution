using BookStoreMVC.DataAccess.Data;
using BookStoreMVC.DataAccess.Repository.IRepository;
using BookStoreMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookStoreMVC.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CompanyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;

        }

        public void Update(Company company)
        {
            //we can do all this OR
            //var objFromDb = _dbContext.Companies.FirstOrDefault(x => x.Id == company.Id);
            //if (objFromDb != null)
            //{
            //    objFromDb.Name = company.Name;
            //    objFromDb.StreetAddress = company.StreetAddress;
            //    objFromDb.City = company.City;
            //    objFromDb.State = company.State;
            //    objFromDb.PostalCode = company.PostalCode;
            //    objFromDb.PhoneNumber = company.PhoneNumber;
            //    objFromDb.IsAuthorizedCompany = company.IsAuthorizedCompany;
            //}

            _dbContext.Update(company);
        }
    }
}
