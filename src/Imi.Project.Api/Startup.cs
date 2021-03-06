using Imi.Project.Api.Core.Entities.Base;
using Imi.Project.Api.Core.Entities.Users;
using Imi.Project.Api.Core.Interfaces.Repositories;
using Imi.Project.Api.Core.Interfaces.Repositories.Games;
using Imi.Project.Api.Core.Interfaces.Repositories.Users;
using Imi.Project.Api.Core.Interfaces.Services;
using Imi.Project.Api.Core.Interfaces.Services.Games;
using Imi.Project.Api.Core.Interfaces.Services.Statistics;
using Imi.Project.Api.Core.Interfaces.Services.Users;
using Imi.Project.Api.Core.Services;
using Imi.Project.Api.Core.Services.Games;
using Imi.Project.Api.Core.Services.Statistics;
using Imi.Project.Api.Core.Services.Users;
using Imi.Project.Api.Infrastructure.Data;
using Imi.Project.Api.Infrastructure.Repositories;
using Imi.Project.Api.Infrastructure.Repositories.Games;
using Imi.Project.Api.Infrastructure.Repositories.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imi.Project.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection")));
            services.AddIdentity<Player, ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            #region IdentityConfiguration
            services.Configure<IdentityOptions>(opt =>
                {
                    opt.Password.RequiredLength = 8;
                    opt.Password.RequireLowercase = true;
                    opt.Password.RequireUppercase = true;
                    opt.Password.RequireDigit = true;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.User.RequireUniqueEmail = true;
                    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    opt.Lockout.MaxFailedAccessAttempts = 3;

                });
            #endregion

            #region DI Repositories
            // Repositories
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IArtistRepository, ArtistRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IPlayedGameRepository, PlayedGameRepository>();
            services.AddScoped<IBoardGameRepository, BoardGameRepository>();
            services.AddScoped<IBoardGameCategoryRepository, BoardGameCategoryRepository>();
            services.AddScoped<IBoardGameArtistRepository, BoardGameArtistRepository>();
            services.AddScoped<IGameScoreRepository, GameScoreRepository>();
            #endregion

            #region DI Services
            // Services
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IArtistService, ArtistService>();
            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<IPlayedGameService, PlayedGameService>();
            services.AddScoped<IBoardGameService, BoardGameService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            #endregion

            #region AutoMapper, AuthenticationOptions, Swagger
            // Shizzle to make programmer's and teacher's life easier
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(jwtOptions =>
                {
                    jwtOptions.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateActor = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = Configuration["JWTConfiguration:Issuer"],
                        ValidAudience = Configuration["JWTConfiguration:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTConfiguration:SigningKey"]))
                    };
                });
            //services.AddAuthorization(opt => opt.AddPolicy("AccesFor8And5Users", policy => { policy.RequireClaim("firstnamechar", new string[] { "B", "S" }); }));
            services.AddAuthorization(options => { 
                options.AddPolicy("AccesFor8And5Users", policy => { policy.RequireClaim("firstnamechar", new string[] { "B", "S", "b", "s" }); }); 
                options.AddPolicy("Administrators", policy => policy.RequireRole(new string[] { "Admin" }));
                options.AddPolicy("BoardGameEditors", policy => policy.RequireRole(new string[] { "BoardGameEditor", "Admin" }));
                options.AddPolicy("ArtistEditors", policy => { policy.RequireRole(new string[] { "ArtistEditor", "Admin" }); });
                options.AddPolicy("OnlyUserAccess", policy => { policy.RequireRole(new string[] { "Player", "ArtistEditor", "BoardGameEditor", "Admin" }); });
                options.AddPolicy("OlderThen50", policy =>
                {
                    policy.RequireAssertion(context => 
                    {
                        var userClaimValue = context.User.Claims.SingleOrDefault(c=>c.Type == "dob")?.Value;
                        if (DateTime.TryParseExact(userClaimValue, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dob))
                            return dob.AddYears(50) < DateTime.UtcNow;
                        return false;
                    });
                });
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BoardGame WebAPI", Version = "v1" });
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below! <br>Example: 12345abcdef",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        jwtSecurityScheme, Array.Empty<string>()
                    }
                });
            }); 
            #endregion

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "BoardGame WebAPI"); });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
