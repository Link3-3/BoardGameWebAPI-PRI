﻿using System;
using System.Collections.Generic;
using Imi.Project.Api.Core.Entities.Base;

namespace Imi.Project.Api.Core.Entities
{
    public class Country : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public ICollection<BoardGame> BoardGames { get; set; }

    }
}
