using AutoMapper;
using Imi.Project.Api.Core.Dtos;
using Imi.Project.Api.Core.Dtos.Games;
using Imi.Project.Api.Core.Dtos.IdentityManagment;
using Imi.Project.Api.Core.Dtos.Users;
using Imi.Project.Api.Core.Entities;
using Imi.Project.Api.Core.Entities.Games;
using Imi.Project.Api.Core.Entities.Users;
using Imi.Project.Api.Core.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imi.Project.Api.Core.Mapping
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            #region Player
            CreateMap<Player, PlayerResponseDto>()
                .ForMember(dest => dest.PlayedGameCount,
                    opt => opt.MapFrom(src => src.GameScores.Count == 0 ? 0 : src.GameScores
                    .Select(gs => gs.PlayerId == src.Id).Count()))
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(p => p.Dob
                    .ConvertToStringDateNotation()))
                .ForMember(dest => dest.PlayTimeTotal,
                    opt => opt.MapFrom(src => src.GameScores
                    .Where(gs => gs.PlayerId == src.Id)
                    .Sum(gs => gs.PlayedGame.PlayTime)
                    .ConvertToStringDuration()))
                .ForMember(dest => dest.MostPlayedGame,
                    opt => opt.MapFrom(src => src.GameScores.Count == 0 ? "Player has not played any games yet.." : src.GameScores
                    .GroupBy(i => i.PlayedGame.BoardGame.Title)
                    .OrderByDescending(grp => grp.Count())
                    .Select(grp => grp.Key).First()));
            CreateMap<PlayerRequestDto, Player>();
            CreateMap<RegisterPlayerRequestDto, Player>()
                .ForMember(dest => dest.NormalizedEmail, opt => 
                    opt.MapFrom(src => src.Email.ToUpper()))
                .ForMember(dest => dest.NormalizedUserName, opt => 
                    opt.MapFrom(src => src.Username.ToUpper()));
            CreateMap<Player, RolePlayerResponseDto>();
            #endregion

            #region Category
            CreateMap<Category, CategoryResponseDto>();
            CreateMap<CategoryRequestDto, Category>();
            #endregion

            #region Artist
            CreateMap<Artist, ArtistResponseDto>()
                .ForMember(dest => dest.NumberOfArtwork,
                    opt => opt.MapFrom(src => src.Artwork
                      .Where(art => art.ArtistId == src.Id).Count()))
                .ForMember(dest => dest.Dob,
                    opt => opt.MapFrom(src => src.Dob
                    .ConvertToStringDateNotation()));
            CreateMap<ArtistRequestDto, Artist>();
            #endregion

            #region BoardGame
            CreateMap<BoardGame, BoardGameResponseDto>()
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.Categories
                    .Select(bc => new CategoryResponseDto
                    {
                        Id = bc.CategoryId,
                        Name = bc.Category.Name
                    })))
                .ForMember(dest => dest.NumberOfCategories,
                    opt => opt.MapFrom(src => src.Categories.Count == 0 ? 0 : src.Categories
                    .Select(bc => bc.CategoryId == src.Id).Count()))
                .ForMember(dest => dest.NumberofArtists,
                    opt => opt.MapFrom(src => src.Artists.Count == 0 ? 0 : src.Artists
                    .Select(ba => ba.BoardGameId == src.Id).Count()))
                .ForMember(dest => dest.PlayTime,
                    opt => opt.MapFrom(src => src.PlayTime
                    .ConvertToStringDuration()));
            CreateMap<BoardGameRequestDto, BoardGame>();
            #endregion

            #region BoardGameCategory
            CreateMap<BoardGameCategoryRequestDto, BoardGameCategory>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));
            #endregion

            #region BoardGameArtist
            CreateMap<BoardGameArtistRequestDto, BoardGameArtist>()
                .ForMember(dest => dest.ArtistId, opt => opt.MapFrom(src => src.ArtistId));
            #endregion

            #region PlayedGame
            CreateMap<PlayedGame, PlayedGameResponseDto>()
                .ForMember(dest => dest.PlayTime,
                    opt => opt.MapFrom(src => src.PlayTime
                      .ConvertToStringDuration()))
                .ForMember(dest => dest.PlayerCount,
                    opt => opt.MapFrom(src => src.GameScores
                      .Where(gs => gs.PlayedGameId == src.Id).Count()))
                .ForMember(dest => dest.GameScores,
                    opt => opt.MapFrom(src => src.GameScores
                      .Where(gs => gs.PlayedGameId == src.Id)
                      .Select(gs => new GameScoreResponseDto
                      {
                          PlayerName = gs.Player.Name,
                          Score = gs.Score
                      }).OrderByDescending(gs=>gs.Score)));
            CreateMap<PlayedGameRequestDto, PlayedGame>();
            #endregion

            #region GameScore
            CreateMap<GameScore, GameScoreResponseDto>()
                .ForMember(dest => dest.PlayerName,
                    opt => opt.MapFrom(src => src.Player.Name))
                .ForMember(dest => dest.Score,
                    opt => opt.MapFrom(src => src.Score));
            CreateMap<GameScoreRequestDto, GameScore>()
                .ForMember(dest => dest.PlayerId, opt => opt.MapFrom(src => src.PlayerId))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score));
            #endregion

            #region ApplicationRoles
            CreateMap<ApplicationRole, RoleResponseDto>();
            CreateMap<RoleRequestDto, ApplicationRole>();
            #endregion

            // expansion for Country and Publisher
            #region Country

            #endregion

            #region Publisher

            #endregion
        }
    }
}
