using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCAP.Api.DataAccess;
using CCAP.Api.Filters;
using CCAP.Api.Services;
using CCAP.Api.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace CCAP.Api {
    public class Startup {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers(options => {
                options.Filters.Add(new GeneralExceptionHandlerFilter());
            });

            services.AddDbContext<CCAPContext>(options => {
                options.UseNpgsql(_configuration.GetConnectionString(StaticProvider.PostgreSQLConnection));
            });

            services.AddCors(options => {
                options.AddPolicy(StaticProvider.FrontendCorsPolicy, builder => {
                    builder.WithOrigins(
                            _configuration.GetSection(StaticProvider.AllowedOrigins).Get<string[]>()
                            )
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            services.AddScoped<IAuthService, AuthService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.Default.GetBytes(_configuration.GetSection("AuthConfig:ServerSecret").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddAuthorization(options => {
                options.AddPolicy(StaticProvider.UserPolicy, policy => policy.RequireClaim("Roles", "user"));
                options.AddPolicy(StaticProvider.ApproverPolicy, policy => policy.RequireClaim("Roles", "approver"));
                options.AddPolicy(StaticProvider.IssuerPolicy, policy => policy.RequireClaim("Roles", "issuer"));
                options.AddPolicy(StaticProvider.AdminPolicy, policy => policy.RequireClaim("Roles", "admin"));
                options.AddPolicy(StaticProvider.StaffPolicy, policy => policy.RequireClaim("Roles", new string[] {"approver", "issuer"}));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(StaticProvider.FrontendCorsPolicy);

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}