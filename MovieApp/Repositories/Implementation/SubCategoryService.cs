using MovieApp.Models.Domain;
using MovieApp.Repositories.Abstract;

namespace MovieApp.Repositories.Implementation
{
    public class SubCategoryService:ISubCategoryService
    {
        private readonly DatabaseContext ctx;

        public SubCategoryService(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }

        public bool Add(SubCategory model)
        {
            try
            {
                ctx.SubCategory.Add(model);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var data = this.GetById(id);
                if (data == null) return false;
                ctx.SubCategory.Remove(data);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public SubCategory GetById(int id)
        {
            return ctx.SubCategory.Find(id);
        }

        public IQueryable<SubCategory> List()
        {
            var data = ctx.SubCategory.AsQueryable();
            return data;
        }

        public bool Update(SubCategory model)
        {
            try
            {
                ctx.SubCategory.Update(model);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
