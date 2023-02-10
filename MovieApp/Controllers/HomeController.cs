using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Models;
using MovieApp.Models.Domain;
using MovieApp.Models.DTO;
using MovieApp.Repositories.Abstract;
using MovieApp.Repositories.Implementation;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace MovieApp.Controllers
{
    public class HomeController : Controller
    {
  
        private readonly IMovieService movieService;
        private readonly ICommentService commentservice;

        public HomeController(IMovieService movieService, ICommentService commentservice)
        {
            this.movieService = movieService;
            this.commentservice = commentservice;
        }

        public IActionResult Index(string term="")
        {
            var homeViewModel = movieService.HomeViewModel(term);
            if (string.IsNullOrEmpty(term))
            {
                return View(homeViewModel);
            }
            
            else
            {
                if (homeViewModel.Count == 0)   
                    TempData["msg"] = "No Result.";
              
                return View("SearchResult", homeViewModel);
            }
        }

      
        public IActionResult SearchResult(string term="")
        {
            var searchResult = movieService.HomeViewModel(term);
            if (searchResult.Count == 0)
                TempData["msg"] = "No Result.";
   
            return View(searchResult);
        }

        public IActionResult MovieByCategory(int categoryId, string term = "", int currentPage = 1)
        {
            TempData["SelectedCategoryId"] = categoryId;
            List<Movie> movieList = movieService.GetMovieByCategoryId(categoryId, term, true, currentPage);

            if (term != "")
            {
                var searchResult = movieService.HomeViewModel(term);
                if (searchResult.Count == 0) TempData["msg"] = "No Result";
                return View("SearchResultByCategory", searchResult);
            }
            return View(movieList);
        }

        public IActionResult MovieBySubCategory(int subcategoryId, string term = "", int currentPage = 1)
        {
            
            TempData["SelectedSubCategoryId"] = subcategoryId;
            List<Movie> movieList = movieService.GetMovieBySubcategoryId(subcategoryId, term, true, currentPage);

            if (term != "")
            {
                var searchResult = movieService.HomeViewModel(term);
                if (searchResult.Count == 0) TempData["msg"] = "No Result";
                return View("SearchResultByCategory", searchResult);
            }
            return View(movieList);

        }

        public IActionResult SearchResultByCategory(List<Movie> searchResult)
        {
            if (searchResult.Count == 0) TempData["msg"] = "No Result";
            return View(searchResult);
        }


        
        public async Task<IActionResult> MovieDetail(int MovieId, string term = "")
        {

            if (!string.IsNullOrEmpty(term))
            {
                TempData["term"] = term;
                return RedirectToAction("SearchResultInDetail");
            }
            var model = await commentservice.List(MovieId);

            if (model == null)
            {
                TempData["msg"] = "Movie can't find";
                return RedirectToAction("Index");
            }
            return View(model);

        }

        public IActionResult SearchResultInDetail()
        {
            string term = TempData["term"].ToString();
            var searchResultInDetail = movieService.SearchResultInDetail(term);
            if (searchResultInDetail.Count == 0)
                TempData["msg"] = "No Result.";

            return View(searchResultInDetail);
           
        }

        public IActionResult TermsBuy(string term="")
        {
            if (!string.IsNullOrEmpty(term))
            {
                TempData["term"] = term;
                return RedirectToAction("SearchResultInDetail");
            }
            return View();
        }

        public IActionResult ContactUs(string term = "")
        {
            if (!string.IsNullOrEmpty(term))
            {
                TempData["term"] = term;
                return RedirectToAction("SearchResultInDetail");
            }
            return View();
        }
    }
}