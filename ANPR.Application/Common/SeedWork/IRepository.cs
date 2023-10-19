using Microsoft.Lonsum.Services.ANPR.Domain.Common;

namespace Microsoft.Lonsum.Services.ANPR.Application.Common.SeedWork
{
    public interface IRepository<T> where T : BaseEntity
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
