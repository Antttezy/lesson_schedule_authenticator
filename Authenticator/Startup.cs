using AuthenticationService.Data;
using AuthenticationService.Services.Access;
using AuthenticationService.Services.Refresh;
using AuthenticationService.Services.Repositories;
using AuthenticationService.Services.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace AuthenticationService
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<JwtAccessTokenOptions>();
            services.AddSingleton<JwtRefreshTokenOptions>();
            services.AddTransient<IAccessTokenGenerator, JWTAccessTokenGenerator>();
            services.AddTransient<IRefreshTokenGenerator, JWTRefreshTokenGenerator>();
            services.AddTransient<IRefreshTokenValidator, RefreshTokenValidator>();

            services.AddDbContext<AuthenticationContext>(opts => opts.UseSqlServer(configuration.GetConnectionString("AuthenticationContext")));
            services.AddScoped<IUserRepository, EFUserRepository>();
            services.AddScoped<ITokenRepository, EFTokenRepository>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
            {
                JwtAccessTokenOptions options = new(configuration);

                o.TokenValidationParameters = new()
                {
                    IssuerSigningKey = options.GetAsymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = options.Issuer,
                    ValidateIssuer = true,
                    ValidAudience = options.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddControllers();
            services.AddSwaggerGen();
            services.AddCors(o =>
            {
                o.AddDefaultPolicy(builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
            }

            app.UseCors();
            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            if (env.IsDevelopment())
            {
                app.UseSwaggerUI(o =>
                {
                    o.SwaggerEndpoint("/swagger/v1/swagger.json", "Authentication server API v1");
                    o.RoutePrefix = "";
                });
            }
        }
    }
}
