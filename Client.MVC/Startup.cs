using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Client.MVC
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //To host static files (e.g. images, CSS, etc.)
            app.UseStaticFiles();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "cookies",
                AutomaticAuthenticate = true,
                ExpireTimeSpan = TimeSpan.FromMinutes(60)
            });

            //https://leastprivilege.com/2016/08/21/why-does-my-authorize-attribute-not-work/
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                AuthenticationScheme = "oidc",
                SignInScheme = "cookies",

                Authority = "http://localhost:42705",
                RequireHttpsMetadata = false,

                ClientId = "client.mvc",
                ClientSecret = "secret",

                ResponseType = "code id_token", // OpenIdConnect Hybrid flow
                Scope = { "openid", "profile", "api1", "offline_access" },

                SaveTokens = true,
                
                //https://leastprivilege.com/2016/08/21/why-does-my-authorize-attribute-not-work/
                TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    NameClaimType = IdentityModel.JwtClaimTypes.Name,
                    RoleClaimType = IdentityModel.JwtClaimTypes.Role,
                }
            });

            app.UseMvcWithDefaultRoute();
        }
    }
}
