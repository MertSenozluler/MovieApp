using MovieApp.Models.Domain;
using MovieApp.Models.DTO;

namespace MovieApp.Repositories.Abstract
{
    public interface IMovieService
    {
        bool Add(Movie model);
        bool Update(Movie model);
        Movie GetById(int id);
        bool Delete(int id);
        MovieListViewModel List(string term = "", bool paging = false, int currentPage = 0);
        List<int> GetCategoryByMovieId(int movieId);
        List<int> GetSubCategoryByMovieId(int movieId);
        List<Movie> GetMovieByCategoryId(int categoryId, string term = "", bool paging = false, int currentPage = 0);
        List<Movie> GetMovieBySubcategoryId (int subcategoryId, string term = "", bool paging = false, int currentPage = 0);
        List<HomeViewModel> HomeViewModel(string term="");
        List<MovieCommentViewModel> SearchResultInDetail(string term = "");
    }
}
