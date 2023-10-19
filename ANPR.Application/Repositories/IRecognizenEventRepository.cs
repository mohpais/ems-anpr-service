using Microsoft.Lonsum.Services.ANPR.Application.Common.SeedWork;
using Microsoft.Lonsum.Services.ANPR.Domain.Entities;

namespace Microsoft.Lonsum.Services.ANPR.Application.Repositories
{
    public interface IRecognizenEventRepository : IRepository<RecognizenEvent>
    {
        RecognizenEvent Add(RecognizenEvent entity);
        void Update(RecognizenEvent entity);
        Task<List<RecognizenEvent>> GetAll();
    }
}
