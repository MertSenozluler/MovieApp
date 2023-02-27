using MovieApp.Models.Domain;
using MovieApp.Models.DTO;

namespace MovieApp.Repositories.Abstract
{
    public interface IUserAdminService
    {
        Task<AdminPageViewModel> AdminPage();

        Task<UserPageViewModel> UserPage(ApplicationUser loggedInUser);
    }
}
