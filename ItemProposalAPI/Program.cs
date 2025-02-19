
using FluentValidation;
using ItemProposalAPI.DataAccess;
using ItemProposalAPI.DTOs.Account;
using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.DTOs.ItemParty;
using ItemProposalAPI.DTOs.Party;
using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.DTOs.User;
using ItemProposalAPI.EnumSchemaFilter;
using ItemProposalAPI.Models;
using ItemProposalAPI.Services.Interfaces;
using ItemProposalAPI.Services.Service;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using ItemProposalAPI.UnitOfWorkPattern.UnitOfWork;
using ItemProposalAPI.Validation.Account;
using ItemProposalAPI.Validation.Item;
using ItemProposalAPI.Validation.ItemParty;
using ItemProposalAPI.Validation.Party;
using ItemProposalAPI.Validation.Proposal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace ItemProposalAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(s =>
            {
                s.SchemaFilter<EnumSchemFilter>();

                s.MapType<DateTime>(() => new Microsoft.OpenApi.Models.OpenApiSchema
                {
                    Type = "string",
                    Format = "date-time"
                });
                s.MapType<DateTime?>(() => new Microsoft.OpenApi.Models.OpenApiSchema
                {
                    Type = "string",
                    Format = "date-time"
                });

                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                s.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

            });

            builder.Services.AddDbContext<ApplicationDbContext>(dbContextOptions =>
            {
                dbContextOptions.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 10;

            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddAuthentication(options =>  {
                options.DefaultAuthenticateScheme =
                options.DefaultChallengeScheme =
                options.DefaultForbidScheme =
                options.DefaultScheme =
                options.DefaultSignInScheme =
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
                    )
                };
            });

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IPartyService, PartyService>();
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<IItemPartyService, ItemPartyService>();
            builder.Services.AddScoped<IProposalService, ProposalService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ITokenService, TokenService>();

            builder.Services.AddScoped<IValidator<CreatePartyRequestDto>, AddPartyValidator>();
            builder.Services.AddScoped<IValidator<UpdatePartyRequestDto>, UpdatePartyValidator>();
            builder.Services.AddScoped<IValidator<CreateItemRequestDto>, AddItemValidator>();
            builder.Services.AddScoped<IValidator<UpdateItemRequestDto>, UpdateItemValidator>();  
            builder.Services.AddScoped<IValidator<CreateItemPartyRequestDto>, AddItemPartyValidator>();
            builder.Services.AddScoped<IValidator<CreateProposalRequestDto>, AddProposalValidator>();
            builder.Services.AddScoped<IValidator<CreateCounterProposalRequestDto>, AddCounterProposalValidator>();
            builder.Services.AddScoped<IValidator<UpdateProposalRequestDto>, UpdateProposalValidator>();
            builder.Services.AddScoped<IValidator<RegisterDto>, RegisterAccountValidator>();
            builder.Services.AddScoped<IValidator<LoginDto>, LoginAccountValidator>();
            builder.Services.AddScoped<IValidator<ReviewProposalDto>, ReviewProposalValidator>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                // Seed data if not already seeded
                await ApplicationDbContext.Seed(context, userManager);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(s =>
                {
                    s.DisplayRequestDuration();
                    s.ConfigObject.AdditionalItems["operationsSorter"] = "method";
                });
            }

            app.UseHttpsRedirection();

            app.Use(async (context, next) =>
            {
                if (context.Request.Method == HttpMethods.Post ||
                    context.Request.Method == HttpMethods.Put ||
                    context.Request.Method == HttpMethods.Patch)
                {
                    if (context.Request.ContentLength == null || context.Request.ContentLength == 0)
                    {
                        context.Response.StatusCode = 400; // Bad Request
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync("{\"Errors\": [\"Request body cannot be empty. Please provide valid JSON data.\"]}");
                        return;
                    }
                }

                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
