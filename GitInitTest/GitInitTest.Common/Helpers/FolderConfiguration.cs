
namespace GitInitTest.Common.Helpers
{
    public interface IFolderConfiguration
    {
        string BaseFolder { get; set; }
        string DataFolder { get; set; }
        string LogFolder { get; set; }
        string ErrorFolder { get; set; }
        string ArchiveFolder { get; set; }
    }

    public class FolderConfiguration : IFolderConfiguration
    {
        public string BaseFolder { get; set; }
        public string DataFolder { get; set; }
        public string LogFolder { get; set; }
        public string ErrorFolder { get; set; }
        public string ArchiveFolder { get; set; }
    }
}