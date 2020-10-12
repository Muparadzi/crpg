using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Crpg.Application.Characters.Models;
using Crpg.Application.Common.Exceptions;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Characters.Queries
{
    public class GetUserCharacterQuery : IMediatorRequest<CharacterViewModel>
    {
        public int CharacterId { get; set; }
        public int UserId { get; set; }

        public class Handler : IMediatorRequestHandler<GetUserCharacterQuery, CharacterViewModel>
        {
            private readonly ICrpgDbContext _db;
            private readonly IMapper _mapper;

            public Handler(ICrpgDbContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<CharacterViewModel>> Handle(GetUserCharacterQuery req, CancellationToken cancellationToken)
            {
                var character = await _db.Characters
                    .AsNoTracking()
                    .Include(c => c.Items.HeadItem)
                    .Include(c => c.Items.CapeItem)
                    .Include(c => c.Items.BodyItem)
                    .Include(c => c.Items.HandItem)
                    .Include(c => c.Items.LegItem)
                    .Include(c => c.Items.HorseHarnessItem)
                    .Include(c => c.Items.HorseItem)
                    .Include(c => c.Items.Weapon1Item)
                    .Include(c => c.Items.Weapon2Item)
                    .Include(c => c.Items.Weapon3Item)
                    .Include(c => c.Items.Weapon4Item)
                    .FirstOrDefaultAsync(c => c.Id == req.CharacterId && c.UserId == req.UserId, cancellationToken);

                return character == null
                    ? new Result<CharacterViewModel>(CommonErrors.CharacterNotFound(req.CharacterId, req.UserId))
                    : new Result<CharacterViewModel>(_mapper.Map<CharacterViewModel>(character)); // can't use ProjectTo https://github.com/dotnet/efcore/issues/20729
            }
        }
    }
}
