using MovieApp.Models.DTO;

namespace MovieApp.Repositories.Abstract
{
    public interface IUserAdminService
    {
        Task<AdminPageViewModel> List();
    }
}
