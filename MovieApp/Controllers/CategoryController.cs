using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using MovieApp.Models.Domain;
using MovieApp.Repositories.Abstract;
using System.Data;

namespace MovieApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly DatabaseContext databaseContext;

        public CategoryController(ICategoryService categoryService, DatabaseContext databaseContext)
        {
            _categoryService = categoryService;
            this.databaseContext = databaseContext;
        }

        
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Category model) 
        {
            if (!ModelState.IsValid) return View(model);
            var result = _categoryService.Add(model);
            if (result)
            {
                TempData["msg"] = "Category successfully added";
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
            var data = _categoryService.GetById(id);
            return View(data);
        }

        [HttpPost]
        public IActionResult Update(Category model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = _categoryService.Update(model);
            if (result)
            {
                TempData["msg"] = "Category successfully updated";
                return RedirectToAction(nameof(Add));
            }
            else
            {
                TempData["msg"] = "Error on the server side...";
                return View(model);
            }
        }

        public IActionResult CategoryList()
        {
            var data = _categoryService.List().ToList();
            return View(data);
        }

        public IActionResult Delete(int id)
        {
            var result = _categoryService.Delete(id);
            TempData["msg"] = "Category successfully deleted";
            return RedirectToAction(nameof(CategoryList));
        }

       
    }
}
