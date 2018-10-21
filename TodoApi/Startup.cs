using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.Data;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TodoApi.Helpers;
using System;


namespace TodoApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
            JwtSecurityKey.SetSecret("a secret that needs to be at least 16 characters long");
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add JWT based authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    //ValidAudience = "TodoApiClient",
                    ValidateIssuer = false,
                    //ValidIssuer = "TodoApi",
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = JwtSecurityKey.Key,
                    ValidateLifetime = true, //validate the expiration and not before values in the token
                    ClockSkew = TimeSpan.FromMinutes(5) //5 minute tolerance for the expiration date
                };
            });

            // Add CORS
            services.AddCors();

            if (Environment.IsDevelopment())
            {
                // In-memory database:
                services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
            }
            else
            {
                // Azure SQL database:
                services.AddDbContext<TodoContext>(opt =>
                         opt.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));
            }

            services.AddScoped<IRepository<TodoItem>, TodoItemRepository>();
            services.AddScoped<IRepository<User>, UserRepository>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // For convenience, I want detailed exception information always. However, this statement should
            // be removed, when the application is released.
            app.UseDeveloperExceptionPage();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // Enable CORS (must precede app.UseMvc()):
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            // Use authentication
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
