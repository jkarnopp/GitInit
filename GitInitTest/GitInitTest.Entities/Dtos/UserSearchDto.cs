
namespace GitInitTest.Entities.Dtos
{
    public class UserSearchDto
    {
        public string LastName { get; set; }
        public int Page { get; set; }

        public string SortOrder { get; set; }
        public string CurrentSort { get; set; }

        public string SearchString { get; set; }


    }
}
