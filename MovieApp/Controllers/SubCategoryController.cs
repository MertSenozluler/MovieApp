using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Models.Domain;
using MovieApp.Repositories.Abstract;

namespace MovieApp.Controllers
{
    [Authorize(Roles ="admin")]
    public class SubCategoryController : Controller
    {
        private readonly ISubCategoryService _subCategoryService;
        private readonly DatabaseContext databaseContext;

        public SubCategoryController(ISubCategoryService subCategoryService, DatabaseContext databaseContext)
        {
            _subCategoryService = subCategoryService;
            this.databaseContext = databaseContext;
        }


        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(SubCategory model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = _subCategoryService.Add(model);
            if (result)
            {
                TempData["msg"] = "SubCategory successfully added";
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
            var data = _subCategoryService.GetById(id);
            return View(data);
        }

        [HttpPost]
        public IActionResult Update(SubCategory model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = _subCategoryService.Update(model);
            if (result)
            {
                TempData["msg"] = "Subcategory successfully updated";
                return RedirectToAction(nameof(Add));
            }
            else
            {
                TempData["msg"] = "Error on the server side...";
                return View(model);
            }
        }

        public IActionResult SubCategoryList()
        {
            var data = _subCategoryService.List().ToList();
            return View(data);
        }

        public IActionResult Delete(int id)
        {
            var result = _subCategoryService.Delete(id);
            TempData["msg"] = "Subcategory successfully deleted";
            return RedirectToAction(nameof(SubCategoryList));
        }
    }
}
