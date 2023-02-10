using MovieApp.Models.Domain;
using MovieApp.Models.DTO;
using MovieApp.Repositories.Abstract;

namespace MovieApp.Repositories.Implementation
{
    public class MovieService : IMovieService
    {
        private readonly DatabaseContext ctx;


        public MovieService(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }

        public bool Add(Movie model)
        {
            try
            {
                ctx.Movie.Add(model);
                ctx.SaveChanges();
                foreach (var categoryId in model.Categories)
                {
                    var movieCategoy = new MovieCategory
                    {
                        MovieId = model.Id,
                        CategoryId = categoryId,
                    };
                    ctx.MovieCategory.Add(movieCategoy);
                    ctx.SaveChanges();
                }
                foreach (var subCategoryId in model.SubCategories)
                {
                    var movieSubCategoy = new MovieSubCategory
                    {
                        MovieId = model.Id,
                        SubCategoryId = subCategoryId,
                    };
                    ctx.MovieSubCategory.Add(movieSubCategoy);
                    ctx.SaveChanges();
                }
                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Delete(int id)
        {
            var data = this.GetById(id);
            if (data == null) return false;
            var movieCategories = ctx.MovieCategory.Where(a => a.MovieId == data.Id);
            foreach (var moviecategory in movieCategories)
            {
                ctx.MovieCategory.Remove(moviecategory);
            }
            var movieSubCategories = ctx.MovieSubCategory.Where(a => a.MovieId == data.Id);
            foreach (var movieSubcategory in movieSubCategories)
            {
                ctx.MovieSubCategory.Remove(movieSubcategory);
            }
            ctx.Movie.Remove(data);
            ctx.SaveChanges();
            return true;
        }

        public Movie GetById(int id)
        {
            return ctx.Movie.Find(id);
        }

        public List<int> GetCategoryByMovieId(int movieId)
        {
            var categoryIds = ctx.MovieCategory.Where(a => a.MovieId == movieId).Select(a => a.CategoryId).ToList();
            return categoryIds;
        }

        public List<int> GetSubCategoryByMovieId(int movieId)
        {
            var categoryIds = ctx.MovieSubCategory.Where(a => a.MovieId == movieId).Select(a => a.SubCategoryId).ToList();
            return categoryIds;
        }

        public List<Movie> GetMovieByCategoryId(int categoryId, string term = "", bool paging = false, int currentPage = 0)
        {
            // we have movieIds according to categoryIds
            var movieIds = ctx.MovieCategory.Where(a => a.CategoryId == categoryId).Select(a => a.MovieId).ToList();

            var movieList = ctx.Movie.Where(n => movieIds.Contains(n.Id)).ToList();

            //search button
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                movieList = movieList.Where(a => a.MovieName.ToLower().Contains(term) || a.MovieDescription.ToLower().Contains(term)).ToList();
            }

            // pagination
            if (paging)
            {
                int pageSize = 4;
                int count = movieList.Count;
                int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                movieList = movieList.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                foreach (var user in movieList)
                {
                    user.PageSize = pageSize;
                    user.CurrentPage = currentPage;
                    user.TotalPages = TotalPages;
                }
            }


            return movieList;
        }



        public List<Movie> GetMovieBySubcategoryId(int subcategoryId, string term = "", bool paging = false, int currentPage = 0)
        {
            
            var movieIds = ctx.MovieSubCategory.Where(a => a.SubCategoryId == subcategoryId).Select(a => a.MovieId).ToList();

            var movieList = ctx.Movie.Where(n => movieIds.Contains(n.Id)).ToList();


            //search button
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                movieList = movieList.Where(a => a.MovieName.ToLower().Contains(term) || a.MovieDescription.ToLower().Contains(term)).ToList();
                
            }
            


            // pagination
            if (paging)
            {
                int pageSize = 4;
                int count = movieList.Count;
                int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                movieList = movieList.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                foreach (var user in movieList)
                {
                    user.PageSize = pageSize;
                    user.CurrentPage = currentPage;
                    user.TotalPages = TotalPages;
                }
            }

            return movieList;
        }

        public MovieListViewModel List(string term = "", bool paging = false, int currentPage = 0)
        {
            var data = new MovieListViewModel();
            var list = ctx.Movie.ToList();

            //search button
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                list = list.Where(a => a.MovieName.ToLower().Contains(term) || a.MovieDescription.ToLower().Contains(term)).ToList();
            }

            // pagination
            if (paging)
            {
                int pageSize = 5;
                int count = list.Count;
                int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                list = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                data.PageSize = pageSize;
                data.CurrentPage = currentPage;
                data.TotalPages = TotalPages;
            }

            foreach (var movie in list)
            {
                var categories = (from category in ctx.Category
                                  join nc in ctx.MovieCategory
                                  on category.Id equals nc.CategoryId
                                  where nc.MovieId == movie.Id
                                  select category.CategoryName).ToList();
                var categoryNames = string.Join(',', categories);
                movie.CategoryNames = categoryNames;
            }

            foreach (var movie in list)
            {
                var subcategories = (from subCategory in ctx.SubCategory
                                  join nc in ctx.MovieSubCategory
                                  on subCategory.Id equals nc.SubCategoryId
                                  where nc.MovieId == movie.Id
                                  select subCategory.SubCategoryName).ToList();
                var subcategoryNames = string.Join(',', subcategories);
                movie.SubCategoryNames = subcategoryNames;
            }


            data.MovieList = list.AsQueryable();
            return data;

        }

        public bool Update(Movie model)
        {
            try
            {
                // these categoryIds are not selected by users and still present is movieCategory table corresponding to
                // this movieId. So these ids should be removed.

                var categoryToDelete = ctx.MovieCategory.Where(a => a.MovieId == model.Id && !model.Categories.Contains(a.CategoryId)).ToList();
                var subcategoryToDelete = ctx.MovieSubCategory.Where(a => a.MovieId == model.Id && !model.SubCategories.Contains(a.SubCategoryId)).ToList();

                foreach (var nCategory in categoryToDelete)
                {
                    ctx.MovieCategory.Remove(nCategory);
                }

                foreach (var nSubCategory in subcategoryToDelete)
                {
                    ctx.MovieSubCategory.Remove(nSubCategory);
                }

                // after delete process, we should add values to tables

                foreach (int catId in model.Categories)
                {
                    var movieCategory = ctx.MovieCategory.FirstOrDefault(a => a.MovieId == model.Id && a.CategoryId == catId);
                    if (movieCategory == null)
                    {
                        movieCategory = new MovieCategory { CategoryId = catId, MovieId = model.Id };
                        ctx.MovieCategory.Add(movieCategory);
                    }
                }

                foreach (int subcatId in model.SubCategories)
                {
                    var movieSubCategory = ctx.MovieSubCategory.FirstOrDefault(a => a.MovieId == model.Id && a.SubCategoryId == subcatId);
                    if (movieSubCategory == null)
                    {
                        movieSubCategory = new MovieSubCategory { SubCategoryId = subcatId, MovieId = model.Id };
                        ctx.MovieSubCategory.Add(movieSubCategory);
                    }
                }

                ctx.Movie.Update(model);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public List<HomeViewModel> HomeViewModel(string term)
        {
            List<HomeViewModel> homeViewModel = new List<HomeViewModel>();

            var movies = ctx.Movie.ToList();

            foreach (var movie in movies)
            {
                // Category Select
                var movieCategories = ctx.MovieCategory
                    .Where(mc => mc.MovieId == movie.Id)
                    .Select(mc => mc.CategoryId).ToList();

                var categories = ctx.Category.Where(c => movieCategories
                    .Contains(c.Id))
                    .Select(c => c.CategoryName).ToList();

                var category = string.Join(", ", categories);

                // SubCategory Select
                var movieSubCategories = ctx.MovieSubCategory
                    .Where(mc => mc.MovieId == movie.Id)
                    .Select(mc => mc.SubCategoryId).ToList();

                var subCategories = ctx.SubCategory.Where(c => movieSubCategories
                    .Contains(c.Id))
                    .Select(c => c.SubCategoryName).ToList();

                var subCategory = string.Join(", ", subCategories);

                homeViewModel.Add(new HomeViewModel
                {
                    Id = movie.Id,
                    MovieName = movie.MovieName,
                    MovieDescription = movie.MovieDescription,
                    Actors = movie.Actors,
                    PublishYear = movie.PublishYear,
                    MovieImage = movie.MovieImage,
                    Category = category,
                    SubCategory = subCategory,
                });         
            }

            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                homeViewModel = homeViewModel.Where(a => a.MovieName.ToLower().Contains(term) || a.MovieDescription.ToLower().Contains(term)
                || a.Actors.ToLower().Contains(term) || a.Category.ToLower().Contains(term) || a.SubCategory.ToLower().Contains(term)).ToList();
            }


            return homeViewModel;
        }

        public List<MovieCommentViewModel> SearchResultInDetail(string term = "")
        {
            List<MovieCommentViewModel> searchResultInDetail = new List<MovieCommentViewModel>();

            var movies = ctx.Movie.ToList();

            foreach (var movie in movies)
            {
                // Category Select
                var movieCategories = ctx.MovieCategory
                    .Where(mc => mc.MovieId == movie.Id)
                    .Select(mc => mc.CategoryId).ToList();

                var categories = ctx.Category.Where(c => movieCategories
                    .Contains(c.Id))
                    .Select(c => c.CategoryName).ToList();

                var category = string.Join(", ", categories);

                searchResultInDetail.Add(new MovieCommentViewModel
                {
                    MovieId = movie.Id,
                    MovieName = movie.MovieName,
                    MovieDescription = movie.MovieDescription,
                    Actors = movie.Actors,
                    PublishYear = movie.PublishYear,
                    MovieImage = movie.MovieImage,
                    Categories = category,
                });
            }

            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                searchResultInDetail = searchResultInDetail.Where(a => a.MovieName.ToLower().Contains(term) || a.MovieDescription.ToLower().Contains(term)
                || a.Actors.ToLower().Contains(term) || a.Categories.ToLower().Contains(term)).ToList();
            }


            return searchResultInDetail;
        }
    }
}
