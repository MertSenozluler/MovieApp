using Microsoft.AspNetCore.Mvc;
using MovieApp.Models.Domain;
using MovieApp.Models.DTO;

namespace MovieApp.Views.Shared.ViewComponents
{
    public class NavbarCategoryListViewComponent : ViewComponent
    {
        private readonly DatabaseContext _context;

        public NavbarCategoryListViewComponent(DatabaseContext context)
        {
            _context = context;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {

            var Categories = _context.Category.ToList();
            var SubCategories = _context.SubCategory.ToList();
            int? selectedCategoryId = TempData["SelectedCategoryId"] as int?;
            int? selectedSubCategoryId = TempData["SelectedSubCategoryId"] as int?;

            var viewModel = new CategoryListViewModel
            {
                CategoryIds = Categories.Select(x => x.Id).ToList(),
                CategoryName = Categories.Select(x => x.CategoryName).ToList(),
                SubCategoryIds = SubCategories.Select(x => x.Id).ToList(),
                SubCategoryName = SubCategories.Select(x => x.SubCategoryName).ToList(),
                SelectedCategoryId = selectedCategoryId ?? 0,
                SelectedSubCategoryId = selectedSubCategoryId ?? 0
            };

            return View(viewModel);


        }
    }
}
