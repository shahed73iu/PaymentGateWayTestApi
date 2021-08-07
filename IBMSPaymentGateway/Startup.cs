using System;
using System.IO;
using System.Text;
using AspNetCoreRateLimit;
using Elastic.Apm.NetCoreAll;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using IBMSPaymentGateway.DbContexts;
using IBMSPaymentGateway.ErrorHandling;
using IBMSPaymentGateway.Helper;
using IBMSPaymentGateway.middleware;

namespace IBMSPaymentGateway
{
    public class Startup
    {
        private readonly IHostEnvironment _env;
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpContextAccessor();
            //Added Auto Mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //Added Auto Mapper
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            if (_env.IsProduction())
            {
                string connection = Environment.GetEnvironmentVariable("ConnectionString");

                services.AddDbContext<ReadDbContext>(options => options.UseSqlServer(connection), ServiceLifetime.Transient);
                services.AddDbContext<WriteDbContext>(options => options.UseSqlServer(connection), ServiceLifetime.Transient);
                Connection.iBOSDDD_VAT = connection;

            }

            else 
            {

                services.AddDbContext<ReadDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Development")), ServiceLifetime.Transient);
                services.AddDbContext<WriteDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Development")), ServiceLifetime.Transient);

                Connection.iBOSDDD_VAT = Configuration.GetConnectionString("Development");

            }

            services.AddControllers(opts =>
            {
                if (_env.IsDevelopment())
                {
                    opts.Filters.Add<AllowAnonymousFilter>();
                }
                else if (_env.IsStaging())
                {
                    opts.Filters.Add<AllowAnonymousFilter>();
                }
                else
                {
                    var authenticatedUserPolicy = new AuthorizationPolicyBuilder()
                              .RequireAuthenticatedUser()
                              .Build();
                    opts.Filters.Add(new AuthorizeFilter(authenticatedUserPolicy));
                }
                //var authenticatedUserPolicy = new AuthorizationPolicyBuilder()
                //             .RequireAuthenticatedUser()
                //             .Build();
                //opts.Filters.Add(new AuthorizeFilter(authenticatedUserPolicy));
            });

            services.AddScoped<AesModel>();

            JwtConfiguration(services);
            services.AddMediatR(typeof(Startup));
            RegisterServices(services);

            #region === serilog in elastic search

            var elasticUri = "http://apilog.akij.net:9200/";//Configuration["ElasticConfiguration:Uri"];

            Log.Logger = new LoggerConfiguration()
               .Enrich.FromLogContext()
               .Enrich.WithExceptionDetails()
               .Enrich.WithMachineName()
               .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
               {
                   AutoRegisterTemplate = true,
               })
            .CreateLogger();

            #endregion ================== close

            #region === Swagger generator

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "iBOS Microservice Service"
                });

                c.EnableAnnotations();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "Enter the request header in the following box to add Jwt To grant authorization Token: Bearer Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
            #endregion ======================= close

            #region ==== Rate limit ======

            services.AddMemoryCache();

            //load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            //load ip rules from appsettings.json
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            // inject counter and rules stores
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            // Add framework services.
            services.AddMvc();

            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            #endregion === Close ========

            //services.AddMvc(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

        }


        private void JwtConfiguration(IServiceCollection services)
        {
            var audienceConfig = Configuration.GetSection("Audience");
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(audienceConfig["Secret"]));
            //var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_env.IsProduction() ? Configuration.GetSection("REACT_APP_SECRET_NAME").Value.Trim() : audienceConfig["Secret"]));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Iss"],
                ValidateAudience = true,
                ValidAudience = audienceConfig["Aud"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = "AuthScheme";
            })
            .AddJwtBearer("AuthScheme", x =>
            {
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = tokenValidationParameters;
            });
        }

        private void RegisterServices(IServiceCollection services)
        {
            DependencyContainer.RegisterServices(services);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
           // app.UseIpRateLimiting(); 
            loggerFactory.AddSerilog();
            app.UseAllElasticApm(Configuration);

             

            app.UseCors(x => x
                     .AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader());
           // app.UseCors();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<UserInfoMiddleware>();
            // app.UseAntiXssMiddleware();
            //app.UseMiddleware<DosAttackMiddleware>();
          
          

            app.ConfigureCustomExceptionMiddleware();

            if (!env.IsProduction())
            {
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "IBMSPaymentGateway/swagger/{documentName}/swagger.json";
                });
 
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/IBMSPaymentGateway/swagger/v1/swagger.json", "IBMSPaymentGateway");
                    c.RoutePrefix = "IBMSPaymentGateway/swagger";
                });
            } 
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
