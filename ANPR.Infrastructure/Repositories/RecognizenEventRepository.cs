using Microsoft.EntityFrameworkCore;
using Microsoft.Lonsum.Services.ANPR.Application.Common.SeedWork;
using Microsoft.Lonsum.Services.ANPR.Application.Repositories;
using Microsoft.Lonsum.Services.ANPR.Domain.Entities;
using Microsoft.Lonsum.Services.ANPR.Infrastructure.Data;

namespace Microsoft.Lonsum.Services.ANPR.Infrastructure.Repositories
{
    public class RecognizenEventRepository : IRecognizenEventRepository
    {
        private readonly ANPRContext _context;
        public RecognizenEventRepository(ANPRContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        readonly List<RecognizenEvent> recognizenEvents = new()
        {
            new RecognizenEvent("ANPR", "B808DJO", "B808DJO", "Black", "vehicle", "Green", "", "", "Dolok", "50144438", "Devin", DateTime.Now),
            new RecognizenEvent("ANPR", "E1790PFS", "B1790RFS", "White", "vehicle", "Black", "", "", "Begerpang", "50144438", "Devin", DateTime.Now)
        };

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public RecognizenEvent Add(RecognizenEvent entity)
        {
            return _context.RecognizenEvents.Add(entity).Entity;
            //recognizenEvents.Add(entity);
            //return entity;
        }

        public async Task<List<RecognizenEvent>> GetAll()
        {
            var data = await _context.RecognizenEvents.ToListAsync();

            if (data == null)
            {
                data = _context.RecognizenEvents.Local
                    .ToList();
            }
            return data;
            //return recognizenEvents;
        }

        public void Update(RecognizenEvent entity)
        {
            throw new NotImplementedException();
        }
    }
}
