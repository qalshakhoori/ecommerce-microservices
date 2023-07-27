using AspnetRunBasics.Models;
using System.Threading.Tasks;

namespace AspnetRunBasics.Services
{
    public interface IUserService
    {
        Task<UserInfo> GetUserInfo();
    }
}