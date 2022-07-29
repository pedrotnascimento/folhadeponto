using FolhaDePonto.DTO;

namespace FolhaDePonto
{
    public interface IAuthentication
    {
        UserDTO GetSignedInUser(string token);
    }
}
