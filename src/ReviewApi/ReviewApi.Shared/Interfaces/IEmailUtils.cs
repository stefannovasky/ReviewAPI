using System.Threading.Tasks;

namespace ReviewApi.Shared.Interfaces
{
    public interface IEmailUtils
    {
        Task SendEmail(string to, string subject, string body);
    }
}
