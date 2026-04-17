using EventTick.Model.Models;
using projectDemo.DTO.Response;

namespace projectDemo.Repository.Ipml
{
    public interface IUserReposiotry
    {
        Task<List<string>> GetRoleByUser(Guid Userid);
        Task<User> GetUserByid(Guid id);
        Task<(User? user, List<Event> events, int status, string messager)> GetListEventByUserID(
            Guid userID
        );
        Task<User> Create(User user);
        User Update(User user);
        string Delete(User user);


        Task<(List<User>, int)> GetAll(int pageIndex, int pageSize, string key, string role);
    }
}
