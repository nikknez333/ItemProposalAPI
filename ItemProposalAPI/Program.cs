
using ItemProposalAPI.DataAccess;
using ItemProposalAPI.EnumSchemaFilter;
using ItemProposalAPI.UnitOfWorkPattern.Interface;
using ItemProposalAPI.UnitOfWorkPattern.UnitOfWork;
using Microsoft.EntityFrameworkCore;
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
            });

            builder.Services.AddDbContext<ApplicationDbContext>(dbContextOptions =>
            {
                dbContextOptions.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
