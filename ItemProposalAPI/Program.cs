
using FluentValidation;
using ItemProposalAPI.DataAccess;
using ItemProposalAPI.DTOs.Item;
using ItemProposalAPI.DTOs.ItemParty;
using ItemProposalAPI.DTOs.Party;
using ItemProposalAPI.DTOs.Proposal;
using ItemProposalAPI.DTOs.User;
using ItemProposalAPI.EnumSchemaFilter;
using ItemProposalAPI.Services.Interfaces;
using ItemProposalAPI.Services.Service;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using ItemProposalAPI.UnitOfWorkPattern.UnitOfWork;
using ItemProposalAPI.Validation.Item;
using ItemProposalAPI.Validation.ItemParty;
using ItemProposalAPI.Validation.Party;
using ItemProposalAPI.Validation.Proposal;
using ItemProposalAPI.Validation.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace ItemProposalAPI
{
    public class Program
    {
        public static void Main(string[] args)
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
            });

            builder.Services.AddDbContext<ApplicationDbContext>(dbContextOptions =>
            {
                dbContextOptions.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IPartyService, PartyService>();
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<IItemPartyService, ItemPartyService>();
            builder.Services.AddScoped<IProposalService, ProposalService>();

            builder.Services.AddScoped<IValidator<CreateUserRequestDto>, UserValidator>();
            builder.Services.AddScoped<IValidator<UpdateUserRequestDto>, UpdateUserValidator>();
            builder.Services.AddScoped<IValidator<CreatePartyRequestDto>, AddPartyValidator>();
            builder.Services.AddScoped<IValidator<UpdatePartyRequestDto>, UpdatePartyValidator>();
            builder.Services.AddScoped<IValidator<CreateItemRequestDto>, AddItemValidator>();
            builder.Services.AddScoped<IValidator<UpdateItemRequestDto>, UpdateItemValidator>();  
            builder.Services.AddScoped<IValidator<CreateItemPartyRequestDto>, AddItemPartyValidator>();
            builder.Services.AddScoped<IValidator<CreateProposalRequestDto>, AddProposalValidator>();
            builder.Services.AddScoped<IValidator<CreateCounterProposalRequestDto>, AddCounterProposalValidator>();
            builder.Services.AddScoped<IValidator<UpdateProposalRequestDto>, UpdateProposalValidator>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                // Seed data if not already seeded
                ApplicationDbContext.Seed(context);
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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
