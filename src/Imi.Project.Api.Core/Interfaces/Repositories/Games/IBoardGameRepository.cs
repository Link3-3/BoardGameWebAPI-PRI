using Imi.Project.Api.Core.Entities.Games;
using Imi.Project.Api.Core.Interfaces.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imi.Project.Api.Core.Interfaces.Repositories.Games
{
    public interface IBoardGameRepository : IRepository<BoardGame>
    {
        IQueryable<BoardGame> GetESOAsync();
        Task<BoardGame> GetByIdESOAsync(Guid id);
        Task<IEnumerable<BoardGame>> GetByCategoryIdAsync(Guid id);
        Task<IEnumerable<BoardGame>> GetByArtistIdAsync(Guid id);
        Task<IEnumerable<BoardGame>> SearchByNameAsync(string title);
        Task<IEnumerable<BoardGame>> GetTopBoardGameWithLongestPlayTimeAsync(int totalItems);
        Task<IEnumerable<BoardGame>> GetTopBoardGameWithHighestRatingAsync(int totalItems);
        Task<long> GetTotalBoardGamePlayTimeAsync();
    }
}
