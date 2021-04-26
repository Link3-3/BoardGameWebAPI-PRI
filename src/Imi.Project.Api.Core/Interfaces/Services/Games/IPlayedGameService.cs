﻿using Imi.Project.Api.Core.Dtos.Games;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Imi.Project.Api.Core.Interfaces.Services.Games
{
    public interface IPlayedGameService
    {
        Task<IEnumerable<PlayedGameResponseDto>> ListAllAsync();
        Task<PlayedGameResponseDto> GetByIdAsync(Guid id);
        Task<IEnumerable<PlayedGameResponseDto>> GetByPlayerIdAsync(Guid id);
        Task<PlayedGameResponseDto> AddAsync(PlayedGameRequestDto playedGameRequestDto);
        Task<PlayedGameResponseDto> UpdateAsync(PlayedGameRequestDto playedGameRequestDto);
        Task DeleteAsync(Guid id);
    }
}
