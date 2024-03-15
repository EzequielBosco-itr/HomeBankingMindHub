using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using System.Net;

namespace HomeBankingMindHub.Services.Implements
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public IEnumerable<ClientDTO> GetAllClientsDTO()
        {
            var clients = _clientRepository.GetAllClients();
            var clientsDTO = new List<ClientDTO>();

            foreach (Client client in clients)
            {
                ClientDTO newClientDTO = new ClientDTO(client);
                clientsDTO.Add(newClientDTO);
            }

            return clientsDTO;
        }

        public Client GetClientByEmail(string email)
        {
            Client client = _clientRepository.FindByEmail(email);
            if (client == null)
            {
                throw new Exception("Cliente no encontrado por email");
            }
            return client;
        }

        public ClientDTO GetClientDTOByEmail(string email)
        {
            Client client = _clientRepository.FindByEmail(email);
            if (client == null)
            {
                throw new Exception("Cliente no encontrado por email");
            }

            return new ClientDTO(client);
        }

        public ClientDTO GetClientDTOById(long id)
        {
            Client client = _clientRepository.FindById(id);
            if (client == null)
            {
                throw new Exception("Cliente no encontrado por id");
            }

            return new ClientDTO(client);
        }

        public bool ClientExistsByEmail(string email)
        {
            return _clientRepository.ExistsByEmail(email);
        }

        public Client CreateClient(string email, string passwordHash, string firstName, string lastName, bool isAdmin)
        {
            Client newClient = new Client
            {
                Email = email,
                Password = passwordHash,
                FirstName = firstName,
                LastName = lastName,
                IsAdmin = isAdmin,
            };
            return newClient;
        }

        public void SaveClient(Client client)
        {
            _clientRepository.Save(client);
        }
    }
}
