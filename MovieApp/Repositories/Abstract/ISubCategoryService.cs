using MovieApp.Models.Domain;

namespace MovieApp.Repositories.Abstract
{
    public interface ISubCategoryService
    {
        bool Add(SubCategory model);
        bool Update(SubCategory model);
        SubCategory GetById(int id);
        bool Delete(int id);
        IQueryable<SubCategory> List();
    }
}
