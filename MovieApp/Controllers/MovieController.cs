using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieApp.Models.Domain;
using MovieApp.Repositories.Abstract;

namespace MovieApp.Controllers
{
    [Authorize(Roles ="admin")]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IFileService _fileService;
        private readonly ICategoryService _catService;
        private readonly ISubCategoryService _subCatService;

        public MovieController(IMovieService movieService, IFileService fileService, ICategoryService catService, ISubCategoryService subCatService)
        {
            this._movieService = movieService;
            _fileService = fileService;
            _catService = catService;
            _subCatService = subCatService;
        }

        public IActionResult Add()
        {
            var model = new Movie();
            model.CategoryList = _catService.List().
                Select(a => new SelectListItem
                {
                    Text = a.CategoryName,
                    Value = a.Id.ToString()
                });
            model.SubCategoryList = _subCatService.List().
                Select(a => new SelectListItem
                {
                    Text = a.SubCategoryName,
                    Value = a.Id.ToString()
                });
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(Movie model)
        {
            model.CategoryList = _catService.List().
                Select(a => new SelectListItem
                {
                    Text = a.CategoryName,
                    Value = a.Id.ToString()
                });
            model.SubCategoryList = _subCatService.List().
                Select(a => new SelectListItem
                {
                    Text = a.SubCategoryName,
                    Value = a.Id.ToString()
                });
            if (model.MovieName.Length < 3 || model.MovieName.Length > 25)
            { 
                TempData["msg"] = "Movie name character length must be between 3 and 25";
                return View(model);
            }
            
            if (!ModelState.IsValid) return View(model);

            if (model.ImageFile != null)
            {
                var fileResult = this._fileService.
                    SaveImage(model.ImageFile);
                if (fileResult.Item1 == 0)
                {
                    TempData["msg"] = "File could not saved";
                    return View(model);
                }
                var imageName = fileResult.Item2;
                model.MovieImage = imageName;
            }
            else
            {
                TempData["msg"] = "Image is required";
                return View(model);
            }

            var result = _movieService.Add(model);
            if (result)
            {
                TempData["msg"] = "Added successfully";
                return RedirectToAction(nameof(Add));
            }
            else
            {
                TempData["msg"] = "Error on the server side...";
                return View(model);
            }

        }

        public IActionResult Edit(int id)
        {
            var model = _movieService.GetById(id);
            var selectedCategories = _movieService.GetCategoryByMovieId(model.Id);
            var selectedSubCategories = _movieService.GetSubCategoryByMovieId(model.Id);
            MultiSelectList multiCategoryList = new MultiSelectList(_catService.List(), "Id", "CategoryName", selectedCategories);
            MultiSelectList multiSubCategoryList = new MultiSelectList(_subCatService.List(), "Id", "SubCategoryName", selectedSubCategories);
            model.MultiCategoryList = multiCategoryList;
            model.MultiSubCategoryList = multiSubCategoryList;
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(Movie model)
        {
            var selectedCategories = _movieService.GetCategoryByMovieId(model.Id);
            var selectedSubCategories = _movieService.GetSubCategoryByMovieId(model.Id);
            MultiSelectList multiCategoryList = new MultiSelectList(_catService.List(), "Id", "CategoryName", selectedCategories);
            MultiSelectList multiSubCategoryList = new MultiSelectList(_subCatService.List(), "Id", "SubCategoryName", selectedSubCategories);
            model.MultiCategoryList = multiCategoryList;
            model.MultiSubCategoryList = multiSubCategoryList;
            if (!ModelState.IsValid) return View(model);
            if (model.ImageFile != null)
            {
                var fileResult = this._fileService.SaveImage(model.ImageFile);
                if (fileResult.Item1 == 0)
                {
                    TempData["msg"] = "File could not saved";
                    return View(model);
                }
                var imageName = fileResult.Item2;
                model.MovieImage = imageName;
            }
            var result = _movieService.Update(model);
            if (result)
            {
                TempData["msg"] = "Successfully Updated";
                return RedirectToAction(nameof(MovieList));
            }
            else
            {
                TempData["msg"] = "Error on the server side...";
                return View();
            }
        }

        public IActionResult MovieList(string term = "", int currentPage = 1)
        {
            var data = this._movieService.List(term, true, currentPage);
            return View(data);
        }

        public IActionResult Delete(int id)
        {
            var result = _movieService.Delete(id);
            TempData["msg"] = "Successfully Deleted";
            return RedirectToAction(nameof(MovieList));
        }

        public IActionResult SearchResult(string term = "")
        {
            if (term != "")
            {
                var result = _movieService.List(term);
                return View(result);
            }
            return View();

        }
    }
}
