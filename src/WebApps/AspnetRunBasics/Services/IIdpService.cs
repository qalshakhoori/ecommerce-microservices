using AspnetRunBasics.Models;
using System.Threading.Tasks;

namespace AspnetRunBasics.Services
{
    public interface IIdpService
    {
        Task<UserInfo> GetUserInfo();
    }
}
