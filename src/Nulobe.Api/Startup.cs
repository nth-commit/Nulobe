﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nulobe.DocumentDb.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Nulobe.Framework;
using Nulobe.Api.Services;
using AutoMapper;
using Nulobe.Api.Middleware;
using System.Diagnostics;
using Microsoft.Azure.Documents.Client;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.AspNetCore.Rewrite;

namespace Nulobe.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;

            var builder = new ConfigurationBuilder()
                .AddConfigurationSources<Startup>(hostingEnvironment)
                .AddCountriesJsonFile();
            _configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Configure<MvcOptions>(options =>
            {
                if (!_hostingEnvironment.IsDevelopment())
                {
                    options.Filters.Add(new RequireHttpsAttribute());
                }
            });

            services.AddAutoMapper(conf =>
            {
                conf.AddCoreApiMapperConfigurations();
            });

            services.ConfigureAuth0(_configuration);
            services.ConfigureQuizlet(_configuration);
            services.AddDocumentDb(_configuration);
            services.ConfigureStorage(_configuration);
            services.ConfigureCountries(_configuration);

            services.AddCoreApiServices(_configuration);
            services.AddQuizletApiServices();

            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IClaimsPrincipalAccessor, HttpClaimsPrincipalAccessor>();
            services.AddScoped<IRemoteIpAddressAccessor, HttpRemoteIpAddressAccessor>();
            services.AddScoped<IAccessTokenAccessor, HttpBearerAccessTokenAccessor>();
            services.AddScoped<Auditor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IDocumentClientFactory documentClientFactory,
            IOptions<Auth0Options> auth0Options)
        {
            loggerFactory.AddConsole();

            using (var client = documentClientFactory.Create())
            {
                client.EnsureFactCollectionAsync().Wait();
            }

            if (!env.IsDevelopment())
            {
                app.UseRewriter(new RewriteOptions().AddRedirectToHttpsPermanent());
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandlingMiddleware();

            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin() // Can make this more secure
                .AllowCredentials()
                .WithExposedHeaders("X-Continuation-Token"));

            app.MapWhen(
                context =>
                {
                    var skipAuthentication = context.Request.Path.StartsWithSegments(new PathString("/quizlet"), StringComparison.InvariantCultureIgnoreCase);
                    return !skipAuthentication;
                },
                innerApp =>
                {
                    innerApp.UseJwtBearerAuthentication(new JwtBearerOptions()
                    {
                        Audience = auth0Options.Value.ClientId,
                        Authority = auth0Options.Value.GetAuthority()
                    });

                    innerApp.UseMvc();
                });

            app.UseMvc();
        }
    }
}
