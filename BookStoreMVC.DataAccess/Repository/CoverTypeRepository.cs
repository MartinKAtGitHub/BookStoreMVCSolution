using BookStoreMVC.DataAccess.Data;
using BookStoreMVC.DataAccess.Repository.IRepository;
using BookStoreMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookStoreMVC.DataAccess.Repository
{
    class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {

        private readonly ApplicationDbContext _dbContext;

        public CoverTypeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(CoverType coverType)
        {
            var objFromDb = _dbContext.CoverTypes.FirstOrDefault(x => x.Id == coverType.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = coverType.Name;
            }
            // We call save from out side to make it more consistent with other operations like Delete
        }
    }
}
