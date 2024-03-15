using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IClientService
    {
        IEnumerable<ClientDTO> GetAllClientsDTO();
        Client GetClientByEmail(string email);
        ClientDTO GetClientDTOByEmail(string email);
        ClientDTO GetClientDTOById(long id);
        bool ClientExistsByEmail(string email);
        Client CreateClient(string email, string passwordHash, string firstname, string lastname, bool isAdmin);
        void SaveClient(Client client);

    }
}
