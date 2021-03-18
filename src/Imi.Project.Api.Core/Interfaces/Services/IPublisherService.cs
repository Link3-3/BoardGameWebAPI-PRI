﻿using Imi.Project.Api.Core.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Imi.Project.Api.Core.Interfaces.Services
{
    public interface IPublisherService
    {
        Task<IEnumerable<PublisherResponseDto>> ListAllAsync();
        Task<CategoryResponseDto> GetByIdAsync();
    }
}