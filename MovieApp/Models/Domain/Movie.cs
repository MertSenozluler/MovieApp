using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MovieApp.Models.Domain
{
    public class Movie
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public string MovieDescription { get; set;}
        public string Actors { get; set;}
        public string PublishYear { get; set;}

      
        public string? MovieImage { get; set; }  // stores movie image name with extension (eg, image0001.jpg)

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [NotMapped]
        [Required]
        public List<int>? Categories { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? CategoryList { get; set; }

        [NotMapped]
        public string? CategoryNames { get; set; }

        [NotMapped]
        public MultiSelectList? MultiCategoryList { get; set; }

        [NotMapped]
        [Required]
        public List<int>? SubCategories { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? SubCategoryList { get; set; }

        [NotMapped]
        public string? SubCategoryNames { get; set; }

        [NotMapped]
        public MultiSelectList? MultiSubCategoryList { get; set; }
        [NotMapped]
        public int PageSize { get; set; }

        [NotMapped]
        public int CurrentPage { get; set; }

        [NotMapped]
        public int TotalPages { get; set; }

        [NotMapped]
        public string? Term { get; set; }


    }
}
