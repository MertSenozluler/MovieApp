using MovieApp.Models.Domain;
using MovieApp.Models.DTO;

namespace MovieApp.Repositories.Abstract
{
    public interface ICategoryService
    {
            bool Add(Category model);
            bool Update(Category model);
            Category GetById(int id);
            bool Delete(int id);
            IQueryable<Category> List();
            
        
    }
}
