using EventTick.Model.Models;

namespace projectDemo.Repository.Ipml
{
    public interface IUserLoginRepository
    {
        Task InsertAsync(UserLogin userLogin);
        UserLogin Update(UserLogin userLogin);
        string DeleteAsync(UserLogin userLogin);
        Task<UserLogin> getbyid(int user);
        Task<UserLogin> GetByProviderUserIdAsync(string providerUserId, string Provider);
    }
}
