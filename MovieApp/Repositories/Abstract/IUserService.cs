using MovieApp.Models.DTO;

namespace MovieApp.Repositories.Abstract
{
    public interface IUserService
    {
        Task<List<ListUsersViewModel>> List(string term = "", bool paging = false, int currentPage = 0);
    }
}
