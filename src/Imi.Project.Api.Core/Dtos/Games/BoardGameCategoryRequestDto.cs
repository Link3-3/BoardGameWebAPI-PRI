using System;
using System.Collections.Generic;
using System.Text;

namespace Imi.Project.Api.Core.Dtos.Games
{
    public class BoardGameCategoryRequestDto
    {
        public Guid BoardGameId { get; set; }
        public Guid CategoryId { get; set; }
    }
}
