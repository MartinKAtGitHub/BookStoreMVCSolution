using BookStoreMVC.DataAccess.Data;
using BookStoreMVC.DataAccess.Repository.IRepository;
using BookStoreMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookStoreMVC.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;

        }

        public void Update(Category category)
        {
            var objFromDb = _dbContext.Categories.FirstOrDefault(x => x.Id == category.Id);
            if(objFromDb != null)
            {
                objFromDb.Name = category.Name;
            }
        }
    }
}
