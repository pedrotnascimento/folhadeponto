using FolhaDePonto.DTO;

namespace FolhaDePonto
{
    public class Authentication : IAuthentication
    {
        public UserDTO GetSignedInUser(string token)
        {
            var mockedUser = new UserDTO
            {
                Id = 1,
                Name = "Usuario Mockado"
            };
            return mockedUser;
        }
    }
}
