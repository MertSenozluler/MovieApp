namespace MovieApp.Models.DTO
{
    public class CategoryListViewModel
    {
        public List<int> CategoryIds { get; set; }
        public List<string> CategoryName { get; set; }
        public List<int> SubCategoryIds { get; set; }
        public List<string> SubCategoryName { get; set; }     
        public int SelectedCategoryId { get; set; }
        public int SelectedSubCategoryId { get; set; }
    }
}
