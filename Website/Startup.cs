using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using DataAccess.Models;
using Website.Repositories;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Services;

namespace Website
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
            // Add NicheShackContext to the context pool so we can use it for dependency injection
            services.AddDbContextPool<NicheShackContext>(options =>
            {
                // Set the connection string to the niche shack database
                options.UseSqlServer(Configuration.GetConnectionString("NicheShackDBConnection"));
            });



            // Configure the identity options
            services.AddIdentity<Customer, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;

            })
                .AddEntityFrameworkStores<NicheShackContext>()
                .AddDefaultTokenProviders();

            // Add authentication using JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    // Set what we will be validating in the token
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = Configuration["TokenValidation:Site"],
                        ValidIssuer = Configuration["TokenValidation:Site"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenValidation:SigningKey"]))
                    };
                });

            // This is used so only customers with an account can access account pages
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Account Policy", policy => policy.RequireClaim("acc", "customer", "admin"));
            });


            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<EmailService>();
            services.AddSingleton<SearchSuggestionsService>();
            services.AddScoped<QueryService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "images")),
                RequestPath = "/images"
            });

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
