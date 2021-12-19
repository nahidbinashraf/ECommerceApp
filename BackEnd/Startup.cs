using BackEnd.DataAccess;
using BackEnd.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd
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

            services.AddControllers();
            //Add CORS
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                   builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
                //options.AddPolicy("EnableCORS", builder =>
                //{
                //    builder.SetIsOriginAllowed(_ => true)
                //            .AllowAnyHeader().AllowAnyMethod().AllowCredentials().Build();

                //});
            });

            //DB Connection
            services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            // Identity Provides
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;

            }).AddEntityFrameworkStores<ApplicationDBContext>();

            //Authentication
            var appSettingSection = Configuration.GetSection("appSettings");
            services.Configure<AppSettings>(appSettingSection);

            var appSetting = appSettingSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSetting.Secret);

            services.AddAuthentication(o =>
           {
               o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               o.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
               o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

           }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
           {
               o.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = appSetting.Site,
                   ValidAudience = appSetting.Audience,
                   IssuerSigningKey = new SymmetricSecurityKey(key)
               };
           });

            //Authorization 
            services.AddAuthorization(options => {
                options.AddPolicy("IsLoggedIn", policy => policy.RequireRole("Admin", "Customer").RequireAuthenticatedUser());
                options.AddPolicy("IsAdministrator", policy => policy.RequireRole("Admin").RequireAuthenticatedUser());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();
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
