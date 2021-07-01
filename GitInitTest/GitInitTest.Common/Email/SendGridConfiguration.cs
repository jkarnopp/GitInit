
namespace GitInitTest.Common.Email
{
    public interface ISendGridConfiguration
    {
        string SendGridApiKey { get; set; }
    }

    public class SendGridConfiguration : ISendGridConfiguration
    {
        public string SendGridApiKey { get; set; }
    }
}
