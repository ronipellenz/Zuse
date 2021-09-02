using Microsoft.Azure.Cosmos;
using Persistance.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistance
{
    public class DbContext : IDbContext
    {
        private readonly Container _container;

        public DbContext(CosmosClient dbClient, string databaseName, string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<Domain.Entities.User> SaveUserAsync(Domain.Entities.User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            var result = await _container.CreateItemAsync(user, new PartitionKey(user.Id));

            return result;
        }

        public async Task<Domain.Entities.User> GetByIdAsync(string id)
        {
            try
            {
                var result = await _container.ReadItemAsync<Domain.Entities.User>(id, new PartitionKey(id));

                return result;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Domain.Entities.User>> GetUsersListAsync()
        {
            var query = _container.GetItemQueryIterator<Domain.Entities.User>(new QueryDefinition("Select * from Users"));
            var results = new List<Domain.Entities.User>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public Task<Domain.Entities.User> UpdateAsync(Domain.Entities.User user, string partitionKey)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Domain.Entities.User> DeleteUserAsync(string id)
        {
            try
            {
                var result = await _container.DeleteItemAsync<Domain.Entities.User>(id, new PartitionKey(id));

                return result;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }  
    }
}
