using AspNet.Security.OpenIdConnect.Primitives;
using AutoMapper;
using DAL;
using DAL.Core;
using DAL.Core.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using FRS.Authorization;
using FRS.Helpers;
using FRS.ViewModels;
using Swashbuckle.AspNetCore.Swagger;
using System;
using Microsoft.EntityFrameworkCore.Proxies;
using AppPermissions = DAL.Core.ApplicationPermissionsTrees;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using FRS.Jobs;
using FRS.Hubs;
using BAL.DTO;
using BAL.Services.Interfaces;
using BAL.Services;

namespace FRS
{
    public class StartupProd
    {
        public IConfiguration Configuration { get; }


        public StartupProd(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"], b => b.MigrationsAssembly("FRS"));
                options.UseOpenIddict();
            });

            // add identity
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configure Identity options and password complexity here
            services.Configure<IdentityOptions>(options =>
            {
                // User settings
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;

                //    //// Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;

                //    //// Lockout settings
                //    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                //    //options.Lockout.MaxFailedAccessAttempts = 10;

                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });


            // Register the OpenIddict services.
            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore().UseDbContext<ApplicationDbContext>();
                })
                .AddServer(options =>
                {
                    options.UseMvc();
                    options.EnableTokenEndpoint("/connect/token");
                    options.AllowPasswordFlow();
                    options.AllowRefreshTokenFlow();
                    options.AcceptAnonymousClients();
                    options.DisableHttpsRequirement(); // Note: Comment this out in production
                    options.RegisterScopes(
                        OpenIdConnectConstants.Scopes.OpenId,
                        OpenIdConnectConstants.Scopes.Email,
                        OpenIdConnectConstants.Scopes.Phone,
                        OpenIdConnectConstants.Scopes.Profile,
                        OpenIdConnectConstants.Scopes.OfflineAccess,
                        OpenIddictConstants.Scopes.Roles);

                    //options.UseRollingTokens(); //Uncomment to renew refresh tokens on every refreshToken request
                    // Note: to use JWT access tokens instead of the default encrypted format, the following lines are required:
                    // options.UseJsonWebTokens();
                    //options.SetAccessTokenLifetime(TimeSpan.FromMinutes(1));
                    //options.SetRefreshTokenLifetime(TimeSpan.FromMinutes(1));
                })
                .AddValidation(); //Only compatible with the default token format. For JWT tokens, use the Microsoft JWT bearer handler.

            // Add cors
            //services.AddCors(corsOptions =>
            //{
            //    corsOptions.AddPolicy("fully permissive", configurePolicy => configurePolicy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());
            //});

            // Add framework services.
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //// Add cors
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy",
            //        builder => builder.WithOrigins(Configuration["AppSettings:baseUrl"])
            //        .AllowAnyMethod()
            //        .AllowAnyHeader()
            //        .AllowCredentials());
            //});

            //// Add framework services.
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSignalR();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });


            //Todo: ***Using DataAnnotations for validation until Swashbuckle supports FluentValidation***
            //services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());


            //.AddJsonOptions(opts =>
            //{
            //    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //});



            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "FRS API", Version = "v1" });
                c.OperationFilter<AuthorizeCheckOperationFilter>();
                c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "password",
                    TokenUrl = "/connect/token",
                    Description = "Note: Leave client_id and client_secret blank"
                });
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Authorization.Policies.ViewAllUsersPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.SSUserMenu));
                options.AddPolicy(Authorization.Policies.ManageAllUsersPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.SSManageUserMenu));

                options.AddPolicy(Authorization.Policies.ViewAllRolesPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.SSRoleMenu));
                options.AddPolicy(Authorization.Policies.ViewRoleByRoleNamePolicy, policy => policy.Requirements.Add(new ViewRoleAuthorizationRequirement()));
                options.AddPolicy(Authorization.Policies.ManageAllRolesPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.SSManageRoleMenu));

                options.AddPolicy(Authorization.Policies.AssignAllowedRolesPolicy, policy => policy.Requirements.Add(new AssignRolesAuthorizationRequirement()));

                options.AddPolicy(Authorization.Policies.ViewAllFacilitiesPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.RBFacilitiesMenu));
                options.AddPolicy(Authorization.Policies.ViewFacilityByFacilityNamePolicy, policy => policy.Requirements.Add(new ViewFacilityAuthorizationRequirement()));
                options.AddPolicy(Authorization.Policies.ManageAllFacilitiesPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.RBManageFacilities));

                options.AddPolicy(Authorization.Policies.ViewAllFacilityTypesPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.RBFacilityTypesMenu));
                options.AddPolicy(Authorization.Policies.ViewFacilityTypeByFacilityTypeNamePolicy, policy => policy.Requirements.Add(new ViewFacilityTypeAuthorizationRequirement()));
                options.AddPolicy(Authorization.Policies.ManageAllFacilityTypesPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.RBManageFacilityTypes));

                options.AddPolicy(Authorization.Policies.ManageAllReservationsPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.RBCalendarMenu));
                options.AddPolicy(Authorization.Policies.AddReservationPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.RBCalendarMenu));

                options.AddPolicy(Authorization.Policies.ViewAllDevicesPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.DMDevicesMenu));
                options.AddPolicy(Authorization.Policies.ManageAllDevicesPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.DMDevicesMenu));

                options.AddPolicy(Authorization.Policies.ViewAllLocationsPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.SSLocationTreeMenu));
                options.AddPolicy(Authorization.Policies.ManageAllLocationsPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.SSLocationTreeManage));

                options.AddPolicy(Authorization.Policies.ViewAllInstitutionsPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.SSInstitutionMenu));
                options.AddPolicy(Authorization.Policies.ManageAllInstitutionsPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.SSInstitutionManage));

                options.AddPolicy(Authorization.Policies.ViewAllDepartmentsPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.SSDepartmentMenu));
                options.AddPolicy(Authorization.Policies.ManageAllDepartmentsPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.SSDepartmentManage));

                options.AddPolicy(Authorization.Policies.ViewAllContactGroupsPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.ACContactGroupMenu));
                options.AddPolicy(Authorization.Policies.ManageAllContactGroupsPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.ACContactGroupManage));

                options.AddPolicy(Authorization.Policies.ViewAllUserPhonebooksPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.ACUserPhonebookMenu));
                options.AddPolicy(Authorization.Policies.ManageAllUserPhonebooksPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.ACUserPhonebookManage));

            });

            //services.AddAuthentication().AddGoogle(googleOptions =>
            //{
            //    googleOptions.CallbackPath = new PathString("/google-callback");
            //    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
            //    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            //});

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });


            // Configurations
            services.Configure<SmtpConfig>(Configuration.GetSection("SmtpConfig"));

            // Business Services
            services.AddScoped<IEmailSender, EmailSender>();

            // Repositories
            services.AddScoped<IUnitOfWork, HttpUnitOfWork>();
            services.AddScoped<IAccountManager, AccountManager>();

            // Auth Handlers
            services.AddSingleton<IAuthorizationHandler, ViewUserAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, ManageUserAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, ViewRoleAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, AssignRolesAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, ViewFacilityAuthorizationHandler>();

            // DB Creation and Seeding
            services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();

            //services.AddScoped<IScopedJobProcessingService, ScopedJobProcessingService>();
            //services.AddHostedService<ReservationJob>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ILogger<Startup> logger, IDatabaseInitializer databaseInitializer, IConfiguration configuration)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug(LogLevel.Warning);
            loggerFactory.AddFile(Configuration.GetSection("Logging"));

            Utilities.ConfigureLogger(loggerFactory, configuration);
            EmailTemplates.Initialize(env);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            try
            {
                //Having database seeding here rather than in Program.Main() ensures logger is configured before seeding occurs
                databaseInitializer.SeedAsync().Wait();
            }
            catch (Exception ex)
            {
                logger.LogCritical(LoggingEvents.INIT_DATABASE, ex, LoggingEvents.INIT_DATABASE.Name);
                throw new Exception(LoggingEvents.INIT_DATABASE.Name, ex);
            }


            //Configure Cors
            //app.UseCors("fully permissive");
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
                RequestPath = new PathString("/Resources")
            });
            app.UseSpaStaticFiles();
            app.UseAuthentication();


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Swagger UI - FRS";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FRS API V1");
            });


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<ReservationHub>("/reservations");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    //spa.UseAngularCliServer(npmScript: "start");
                    //spa.Options.StartupTimeout = TimeSpan.FromSeconds(120); // Increase the timeout if angular app is taking longer to startup
                    //spa.UseProxyToSpaDevelopmentServer("http://localhost:4200"); // Use this instead to use the angular cli server
                }
            });
        }
    }
}
