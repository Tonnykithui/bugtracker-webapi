using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core2.Interfaces;
using Core2.Models;
using Core2.Repository;
using DataStore.EF;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TicketSystemAPI.Interfaces;
using TicketSystemAPI.Repository;

namespace TicketSystemAPI
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

            //ADD DB CONTEXT
            services.AddDbContext<AppDbContext>(options => 
            options.UseSqlServer(Configuration.GetConnectionString("DefConnection"), 
            b => b.MigrationsAssembly("TicketSystemAPI")));

            //CONFIGURE IDENTITY
            services.AddIdentity<ApplicationsUser, IdentityRole>(options =>
            {
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            //ADD AUTHENTICATION
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidAudience = "Users",
                    ValidIssuer = "https://localhost:44346/",
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Thisissigningkey")),
                    ValidateIssuerSigningKey = true
                };
            });

            services.AddCors(options =>
            {
                options.AddPolicy("Tickets",
                    builder =>
                {
                    builder.WithOrigins(Configuration.GetSection("AllowedOrigin").Get<string[]>())
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            //ADD LOGIN SERVICES
            services.AddScoped<ILoginRegister, LoginRegister>();


            services.AddScoped<IProject, ProjectRepository>();

            services.AddScoped<ITicket, TicketRepository>();

            services.AddScoped<IComment, CommentRepository>();

            services.AddScoped<IPeople, UserRepository>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseCors("Tickets");

            app.UseMvc();
            
            CreateRolesAndAdmin(serviceProvider).Wait();
        }


        private async Task CreateRolesAndAdmin(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationsUser>>();
            string[] roleName = { "Admin", "Developer" };
            IdentityResult identityResult;

            foreach(var role in roleName)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    identityResult = await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var powerUser = new ApplicationsUser
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                FirstName = "Admin",
                LastName = "Admin"
            };

            string password = "admin1234";

            var adminExists = await userManager.FindByEmailAsync("admin@gmail.com");

            if(adminExists == null)
            {
                var createPowerUser = await userManager.CreateAsync(powerUser, password);

                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(powerUser, "Admin");
                }
            }

        }
    }
}


