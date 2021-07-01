
namespace GitInitTest.Entities.Dtos
{
    public class PageListActionDto
    {
        public string SortOrder { get; set; }
        public string CurrentSort { get; set; }
        public string CurrentFilter { get; set; }
        public string SearchString { get; set; }
        public string IncludedProperties { get; set; }
        public int? Page { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

    }
}

