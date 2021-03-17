
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NoteApp_UserManagement_Api.Services;
using NoteApp_UserManagement_Api.Models;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace NoteApp_UserManagement_Api
{
    public class Startup
    {

        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        }




        #region snippet_ConfigureServices
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<UserDatabaseSettings>(
                _configuration.GetSection(nameof(UserDatabaseSettings)));
            services.AddSingleton<IConfiguration>(_configuration);

            services.AddSingleton<IUserDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<UserDatabaseSettings>>().Value);

            services.AddSingleton<UserService>();

            services.AddControllers()
                .AddNewtonsoftJson(options => options.UseMemberCasing());
            /*services.AddAuthentication()
                .AddGoogle(googleOptions => {  })    
                .AddTwitter(twitterOptions => {  })
                .AddFacebook(facebookOptions => {  });*/
            var secret = _configuration["JWTToken:Secret"];
            var key = Encoding.ASCII.GetBytes(secret);
            services.AddApiVersioning(apiVerConfig =>
                {
                    apiVerConfig.AssumeDefaultVersionWhenUnspecified = true;
                    apiVerConfig.DefaultApiVersion = new ApiVersion(new DateTime(2021, 1, 1));
                });
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<UserService>();
                        var userId = context.Principal.Identity.Name;
                        var user = userService.Get(userId);
                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        }
        #endregion

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseCors(policy => policy.AllowAnyMethod());
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
