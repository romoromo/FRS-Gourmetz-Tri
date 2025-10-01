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
using AppPermissions = DAL.Core.ApplicationPermissionsTrees;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using FRS.Hubs;
using Microsoft.AspNetCore.Http.Connections;
using DAL.Repositories.Interfaces;
using DAL.Repositories;
using Sieve.Models;
using Sieve.Services;
using DAL.Filters;
using BAL.Mapping;
using BAL.Services.Interfaces;
using BAL.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using AspNetCore.Proxy;
using ServiceModels;
using SoapCore;
using System.ServiceModel;
using BAL.Utilities;
using BAL.Services.MealOrder;
using BAL.Services.Interfaces.MealOrder;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Http.Features;
using BAL.DTO;
using FRS.Middleware;
using Microsoft.AspNetCore.DataProtection;
using System.Threading.Tasks;
using WebSocketOptions = Microsoft.AspNetCore.Builder.WebSocketOptions;
using OpenIddict.Validation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using DAL.Core.Helpers;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace FRS
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IConfiguration StaticConfiguration { get; }
        private string _corsPolicyname = "FullyPermissive";
        
        private readonly ILogger<Startup> _logger;

        public Startup(ILogger<Startup> logger, IConfiguration configuration)
        {
            Configuration = configuration;
            StaticConfiguration = configuration;
            _logger = logger;
        }

        public static IConfiguration StaticConfig { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseLazyLoadingProxies();

                if(Configuration["AppSettings:RETRY_ON_FAILURE"] == "Y")
                {
                    options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"], //b => b.MigrationsAssembly("FRS"),
                    op => {
                        op.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                        );
                        op.MigrationsAssembly("FRS");
                    });
                }
                else
                {
                options.UseSqlServer(
                    Configuration["ConnectionStrings:DefaultConnection"],
                    b =>
                    {
                        b.MigrationsAssembly("FRS");
                        int timeout = Convert.ToInt32(Configuration["AppSettings:DB_COMMAND_TIMEOUT"] ?? "120");
                        b.CommandTimeout(timeout);
                    });
                }
                
                options.UseOpenIddict();
                options.EnableSensitiveDataLogging();
            });


            /* Uncomment when cookie affinity doesn't fix the issue on jwt*/
            services.AddDataProtection()
                .PersistKeysToDbContext<ApplicationDbContext>()
                //.PersistKeysToFileSystem(new DirectoryInfo(@"\\server\share\directory\"))
                .SetApplicationName("FRS");

            /* Uncomment when cookie affinity doesn't fix the issue on jwt
            //services.AddDataProtection()
            //    .PersistKeysToDbContext<ApplicationDbContext>()
            //    //.PersistKeysToFileSystem(new DirectoryInfo(@"\\server\share\directory\"))
            //    .SetApplicationName("FRS");
            */

            // add identity
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<ApplicationUserManager>()
                .AddRoleManager<ApplicationRoleManager>()
                .AddDefaultTokenProviders();

            // Configure Identity options and password complexity here
            services.Configure<IdentityOptions>(options =>
            {
                // User settings
                options.User.RequireUniqueEmail = true;

                string isBypassEmailConfirmation = Configuration["AppSettings:IsBypassEmailConfirmation"];
                options.SignIn.RequireConfirmedEmail = !string.IsNullOrEmpty(isBypassEmailConfirmation) && isBypassEmailConfirmation == "N";

                //    //// Password settings
                string passwordLength = Configuration["AppSettings:PASSWORD_LENGTH"];
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = passwordLength != null ? Int32.Parse(passwordLength) : 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                //    //// Lockout settings
                string maxFailedAttempt = Configuration["AppSettings:MAX_FAILED_ATTEMPT"];
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(365);
                options.Lockout.MaxFailedAccessAttempts = maxFailedAttempt != null ? Int32.Parse(maxFailedAttempt) : 5;

                options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
            });

            services.AddProxies();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            })
            .AddCookie();

            //services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
            //.AddJwtBearer(options =>
            //{
            //    //options.Authority = Configuration.GetSection("Jwt:Authority").Get<string>();
            //    options.Audience = Configuration.GetSection("Jwt:Audience").Get<string>();
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        //ValidateIssuerSigningKey = true,
            //        ValidAudiences = Configuration.GetSection("Jwt:ValidAudiences").Get<string[]>(),
            //        ValidIssuers = Configuration.GetSection("Jwt:ValidIssuers").Get<string[]>()
            //    };

            //    options.Events = new JwtBearerEvents
            //    {
            //        OnChallenge = context =>
            //        {
            //            context.HandleResponse();
            //            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //            context.Response.ContentType = "application/json";

            //            // Ensure we always have an error and error description.
            //            if (string.IsNullOrEmpty(context.Error))
            //                context.Error = "invalid_token";
            //            if (string.IsNullOrEmpty(context.ErrorDescription))
            //                context.ErrorDescription = "This request requires a valid JWT access token to be provided";

            //            // Add some extra context for expired tokens.
            //            if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
            //            {
            //                var authenticationException = context.AuthenticateFailure as SecurityTokenExpiredException;
            //                context.Response.Headers.Add("x-token-expired", authenticationException.Expires.ToString("o"));
            //                context.ErrorDescription = $"The token expired on {authenticationException.Expires.ToString("o")}";
            //            }

            //            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            //            {
            //                error = context.Error,
            //                error_description = context.ErrorDescription
            //            }));
            //        },
            //        OnMessageReceived = context =>
            //        {
            //            var accessToken = context.Request.Query["access_token"];

            //            return Task.CompletedTask;
            //        },
            //        OnAuthenticationFailed = context =>
            //        {
            //            _logger.LogInformation($"Authentication failed because {context.Exception.Message}");
            //            _logger.LogInformation($"Authentication failed because {context.Exception.StackTrace.ToString()}");
            //            return Task.FromException(context.Exception);
            //        }
            //    };
            //});

            // Register the OpenIddict services.
            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore().UseDbContext<ApplicationDbContext>();
                })
                .AddClient(options =>
                {
                    options.AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate();
                })
                //.AddClient(options =>
                //{
                //    options.AddEphemeralEncryptionKey()
                //           .AddEphemeralSigningKey();
                //})
                .AddServer(options =>
                {
                    //var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("RxnO9lDzsCP8vu4NFOvPbpVSkKHBMfzUjyqBv8bg"));
                    //options.AddSigningKey(signingKey);

                    //options.AddEphemeralEncryptionKey()
                    //        .AddEphemeralSigningKey();

                    //options.UseMvc();
                    //options.AllowAuthorizationCodeFlow();
                    options.SetTokenEndpointUris("/connect/token");
                    options.SetIssuer(Configuration["AppSettings:baseUrl"]);
                    options.AllowPasswordFlow();
                    options.AllowRefreshTokenFlow();
                    options.AcceptAnonymousClients();
                    options.AllowClientCredentialsFlow();
                    //options.DisableHttpsRequirement(); // Note: Comment this out in production
                    options.RegisterScopes(
                        OpenIddictConstants.Scopes.OpenId,
                        OpenIddictConstants.Scopes.Email,
                        OpenIddictConstants.Scopes.Phone,
                        OpenIddictConstants.Scopes.Profile,
                        OpenIddictConstants.Scopes.OfflineAccess,
                        OpenIddictConstants.Scopes.Roles);

                    // Register the signing and encryption credentials.
                    options.AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate();

                    // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                    options.UseAspNetCore()
                           .EnableTokenEndpointPassthrough()
                           .DisableTransportSecurityRequirement();

                   // options.UseDataProtection();
                    options.DisableAccessTokenEncryption();
                    //options.UseRollingTokens(); //Uncomment to renew refresh tokens on every refreshToken request
                    // Note: to use JWT access tokens instead of the default encrypted format, the following lines are required:
                    //options.UseJsonWebTokens();
                    options.SetAccessTokenLifetime(TimeSpan.FromDays(1));
                    options.SetRefreshTokenLifetime(TimeSpan.FromDays(30));
                })
                .AddValidation(options =>
                {
                    //var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("RxnO9lDzsCP8vu4NFOvPbpVSkKHBMfzUjyqBv8bg"));
                    //options.AddSigningKey(signingKey);
                    //options.UseDataProtection();
                    options.UseLocalServer();
                    //options.UseAspNetCore();
                    //options.AddAudiences("frs");


                    //options.SetIssuer(Configuration["AppSettings:baseUrl"]);
                    //options.UseSystemNetHttp((config =>
                    //{
                    //    config.ConfigureHttpClientHandler(c =>
                    //    {
                    //        c.ServerCertificateCustomValidationCallback = (HttpRequestMessage requestMessage,
                    //                                           X509Certificate2 certificate,
                    //                                           X509Chain chain,
                    //                                           SslPolicyErrors sslPolicyErrors) => true;
                    //    });

                    //}));
                    options.UseAspNetCore();

                }); //Only compatible with the default token format. For JWT tokens, use the Microsoft JWT bearer handler.
            /* Uncomment if cookie affinity doesn't fix the jwt issue
            .AddValidation(options =>
            {
                options.AddAudiences(Configuration.GetSection("Jwt:Audience").Get<string>());
                options.AddEventHandler<MyEventHandlerA>();
                options.AddEventHandler<MyEventHandlerB>();
                //options.AddEventHandler<OpenIddictValidationEvents.RetrieveToken>(
                //options.AddEventHandler<OpenIddictValidationEvents.RetrieveToken>(ev =>
                //{
                //    ev.Context.Token = ev.Context.Request.Query["access_token"];
                //    return Task.CompletedTask;
                //});
            });
            */

            //services.AddAuthentication(options =>
            //   {
            //       options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            //       options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            //   }).AddCookie();

            // Add cors
            var origins = Configuration["AppSettings:corsOrigin"].Split(";");
            services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy(_corsPolicyname,
                    configurePolicy => configurePolicy
                    .WithOrigins(origins)
                    //.SetIsOriginAllowed((x) => true)
                    //.SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    //.AllowAnyOrigin()
                    ///.DisallowCredentials()


                    );
            });



            // Add framework services.
            //services.AddMvc().AddControllersAsServices()
            //    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            //    .AddJsonOptions(
            //        options => options.SerializerSettings.ReferenceLoopHandling =
            //        Newtonsoft.Json.ReferenceLoopHandling.Ignore
            //    );

            services.AddMvc().AddControllersAsServices();

            services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            // If you still need Newtonsoft.Json:
            services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            //services.AddControllers(options =>
            //{
            //    // Configure MVC options here if needed
            //    options.EnableEndpointRouting = false;
            //});


            //services.AddControllers(options =>
            //{
            //    options.Filters.Add(new CorsAuthorizationFilterFactory("MyCorsPolicy"));
            //});


            //services.Configure<MvcOptions>(options =>
            //{
            //    options.Filters.Add(new CorsAuthorizationFilterFactory(_corsPolicyname));
            //});
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

            services.AddHsts(options =>
            {
                options.Preload = true; // Preload for browser support
                options.IncludeSubDomains = true; // Apply to subdomains
                options.MaxAge = TimeSpan.FromDays(365); // Set max-age to 1 year
                //options.ExcludedHosts.Add("tappeemeals.com"); // Exclude specific hosts
                options.ExcludedHosts.Add("uat.tappeemeals.com");
            });

            string enableSignalR = Configuration["AppSettings:enableSignalR"];
            if (string.IsNullOrEmpty(enableSignalR) || enableSignalR == "Y")
            {
                services.AddSignalR(options =>
                {
                    options.EnableDetailedErrors = Configuration["AppSettings:enableSignalrDetailedError"] == "Y";
                    options.KeepAliveInterval = TimeSpan.FromMinutes(3);
                    //options.HandshakeTimeout = 
                    options.ClientTimeoutInterval = TimeSpan.FromMinutes(6);
                })
                /* Uncomment if cookie affinity doesn't fix the issue
                 .AddSqlServer(o =>
                 {
                     o.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
                     o.AutoEnableServiceBroker = true;
                     o.TableSlugGenerator = hubType => hubType.Name;
                     o.TableCount = 1;
                     o.SchemaName = "SignalRCore";
                 });
                */
                //.AddSqlServer(Configuration["ConnectionStrings:DefaultConnection"]); 
                ;
            }

            string isAppTier = Configuration["AppSettings:isAppTier"];
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";

                //if (string.IsNullOrEmpty(isAppTier) || isAppTier == "N")
                //{
                //    configuration.RootPath = "ClientApp/dist";
                //}
                //else
                //{
                //    configuration.RootPath = "ClientApp";
                //}
            });


            //Todo: ***Using DataAnnotations for validation until Swashbuckle supports FluentValidation***
            //services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());


            //.AddJsonOptions(opts =>
            //{
            //    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //});
            var baseUrl = Configuration["AppSettings:baseUrl"];
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FRS API", Version = "v1" });
                c.OperationFilter<AuthorizeCheckOperationFilter>();
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri($"{baseUrl}/connect/token"),
                        }
                    }
                });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "FRS.xml");
                c.IncludeXmlComments(filePath);
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

                //options.AddPolicy(Authorization.Policies.ViewAllPIBTemplatesPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.ViewUserPhonebooks));
                //options.AddPolicy(Authorization.Policies.ManageAllPIBTemplatesPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.ManageUserPhonebooks));
            });

            services.AddAuthenticationCore().AddOpenIddict();
            services.AddAuthentication();

            // TODO: Uncomment if required. SATS version doesn't have this
            //services.AddAuthentication().AddGoogle(googleOptions =>
            //{
            //    googleOptions.CallbackPath = new PathString("/google-callback");
            //    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
            //    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            //});

            //services.AddAutoMapper(typeof(Startup));
            //services.AddScoped<IMapper, Mapper>();
            //services.AddAutoMapper(typeof(AutoMapperProfile), typeof(EntityToDTOAutoMapperProfile));
            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //services.AddAuthentication()
            //    .AddJwtBearer(options =>
            //    {
            //        // Configure JWT Bearer authentication
            //        options.Authority = baseUrl;
            //        options.Audience = "api";
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = true,
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("frs-api")),
            //            ValidIssuer = "frs-api",
            //            ValidAudience = "frs-api"
            //        };
            //    });

            services.AddAutoMapper(config =>
            {
                config.AddProfile<AutoMapperProfile>();
                config.AddProfile<EntityToDTOAutoMapperProfile>();
                config.AllowNullCollections = true;
                config.AddGlobalIgnore("Item");
            }, AppDomain.CurrentDomain.GetAssemblies());

            //Auto Mapper Configurations
            //var mappingConfig = new MapperConfiguration(config =>
            //{
            //    config.AddProfile(typeof(AutoMapperProfile));
            //    config.AddProfile(typeof(EntityToDTOAutoMapperProfile));
            //    config.AllowNullCollections = true;
            //    config.AddGlobalIgnore("Item");
            //});

            //create and configure mapper
            //IMapper mapper = mappingConfig.CreateMapper();
            //services.AddSingleton(mapper);

            //mapper.Initialize(config =>
            //{
            //    config.AddProfile(typeof(AutoMapperProfile));
            //    config.AddProfile(typeof(EntityToDTOAutoMapperProfile));
            //});


            // Configurations
            services.Configure<SmtpConfig>(Configuration.GetSection("SmtpConfig"));

            services.Configure<SmtpOauth2Config>(Configuration.GetSection("SmtpOauth2Config"));

            // Business Services
            services.AddScoped<IEmailSender, EmailSender>();

            // Repositories
            services.AddScoped<IUnitOfWork, HttpUnitOfWork>();
            services.AddScoped<IAccountManager, AccountManager>();
            services.AddScoped<IContactGroupRepository, ContactGroupRepository>();
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

            services.Configure<SieveOptions>(Configuration.GetSection("Sieve"));
            services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();
            services.AddScoped<ISieveCustomFilterMethods, SieveCustomFilterMethods>();
            services.AddScoped<IApiKeyManager, ApiKeyManager>();

            //Services
            services.AddScoped<IAuthenticationLogService, AuthenticationLogService>();
            services.AddScoped<IUpDownTimeLogService, UpDownTimeLogService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IImageReferenceTypeService, ImageReferenceTypeService>();
            services.AddScoped<IImageReferenceColorService, ImageReferenceColorService>();
            services.AddScoped<ISmartRoomService, SmartRoomService>();

            services.AddScoped<IQueueService, QueueService>();
            services.AddScoped<IAssetService, AssetService>();
            services.AddScoped<IServiceContractService, ServiceContractService>();
            services.AddScoped<IApplicationSettingService, ApplicationSettingService>();
            services.AddScoped<IEmailQueueService, EmailQueueService>();
            services.AddScoped<IConnectionService, ConnectionService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IStudentWalletService, StudentWalletService>();
            services.AddScoped<IStudentPointService, StudentPointService>();
            services.AddScoped<IRewardService, RewardService>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IChannelInfoService, ChannelInfoService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IManagementService, ManagementService>();
            services.AddScoped<IUserActivityRepository, UserActivityRepository>();

            services.Configure<FormOptions>(o =>  // currently all set to max, configure it to your needs!
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = long.MaxValue; // <-- !!! long.MaxValue
                o.MultipartBoundaryLengthLimit = int.MaxValue;
                o.MultipartHeadersCountLimit = int.MaxValue;
                o.MultipartHeadersLengthLimit = int.MaxValue;
            });


            #region MEAL ORDER SYSTEM
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ITokenOrderService, TokenOrderService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped<IRestrictionService, RestrictionService>();
            services.AddScoped<IMealService, MealService>();
            services.AddScoped<IDishService, DishService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped<IOrderPortalService, OrderPortalService>();

            #endregion
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ILogger<Startup> logger, IDatabaseInitializer databaseInitializer, IConfiguration configuration,
             IApplicationLifetime lifetime)
        {
            // Add file logging if using a third-party provider
            loggerFactory.AddFile(Configuration.GetSection("Logging"));

            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            //if (configuration["Logging:Default"] == "Trace")
            //{
            //    loggerFactory.AddDebug(LogLevel.Trace);
            //}
            //else if (configuration["Logging:Default"] == "Debug")
            //{
            //    loggerFactory.AddDebug(LogLevel.Trace);
            //}
            //else if (configuration["Logging:Default"] == "Error")
            //{
            //    loggerFactory.AddDebug(LogLevel.Error);
            //}
            //else if (configuration["Logging:Default"] == "Information")
            //{
            //    loggerFactory.AddDebug(LogLevel.Information);
            //}

            //loggerFactory.AddFile(Configuration.GetSection("Logging"));

            Utilities.ConfigureLogger(loggerFactory, configuration);
            Logger.ConfigureLogger(loggerFactory, configuration);
            ApplicationPermissionsTrees.SetAclPath(configuration["AppSettings:ACL_PATH"]);
            EmailTemplates.Initialize(env);

            bool enableErrorPage = configuration["AppSettings:ENABLE_ERROR_PAGE"] == "Y";
            if (env.IsDevelopment() && !enableErrorPage)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(configuration["AppSettings:ERROR_PAGE"] ?? "/Error");
                app.UseHsts();
            }

            try
            {
                //Having database seeding here rather than in Program.Main() ensures logger is configured before seeding occurs
                string migrationDB = configuration["AppSettings:ENABLE_DB_MIGRATION"];
                databaseInitializer.SeedAsync(string.IsNullOrEmpty(migrationDB) || migrationDB == "Y", false).Wait();

                string resetConnectionAtStartup = configuration["AppSettings:resetConnectionAtStartup"];
                logger.LogInformation(LoggingEvents.CONNECTION_STATUS, string.Format("Connection reset enabled: {0}", resetConnectionAtStartup));

                if (string.IsNullOrEmpty(resetConnectionAtStartup) || resetConnectionAtStartup == "Y")
                {
                    logger.LogInformation(LoggingEvents.INIT_USER_CONNECTION, "Trying to initialise user signalR connections");
                    try
                    {
                        databaseInitializer.RemoveUserConnectionsAsync().Wait();
                    }
                    catch (Exception ex)
                    {
                        logger.LogCritical(LoggingEvents.INIT_USER_CONNECTION, ex, "Error initialising user connections");
                    }

                    logger.LogInformation(LoggingEvents.INIT_DEVICE_CONNECTION, "Trying to initialise device signalR connections");
                    try
                    {
                        databaseInitializer.RemoveDeviceConnectionsAsync().Wait();
                    }
                    catch (Exception ex)
                    {
                        logger.LogCritical(LoggingEvents.INIT_DEVICE_CONNECTION, ex, "Error initialising device connections");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(LoggingEvents.INIT_DATABASE, ex, LoggingEvents.INIT_DATABASE.Name);

                string proxiedAddress = configuration["AppSettings:PROXIED_ADDRESS"];
                if (string.IsNullOrEmpty(proxiedAddress))
                {
                    throw new Exception(LoggingEvents.INIT_DATABASE.Name, ex);
                }
            }

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation(LoggingEvents.APPLICATION_STOPPING, "ApplicationStopping");
                //logger.LogInformation(LoggingEvents.APPLICATION_STOPPING, "Trying to remove signalR connections");
                //try
                //{
                //    databaseInitializer.RemoveUserConnectionsAsync().Wait();
                //}
                //catch (Exception ex)
                //{
                //    logger.LogCritical(LoggingEvents.APPLICATION_STOPPING, ex, "Error removing user connections");
                //}
            });

            lifetime.ApplicationStopped.Register(() =>
            {
                logger.LogInformation(LoggingEvents.APPLICATION_STOPPED, "ApplicationStopped");
                //logger.LogInformation(LoggingEvents.APPLICATION_STOPPED, "Trying to remove signalR connections");
                //try
                //{
                //    databaseInitializer.RemoveUserConnectionsAsync().Wait();
                //}
                //catch (Exception ex)
                //{
                //    logger.LogCritical(LoggingEvents.APPLICATION_STOPPED, ex, "Error removing user connections");
                //}
            });

            //Configure Cors
            app.UseCors(_corsPolicyname);
            //app.UseCors(builder => builder
            //    .AllowAnyOrigin()
            //    .AllowAnyHeader()
            //    .AllowAnyMethod());

            try
            {
                string proxiedAddress = configuration["AppSettings:PROXIED_ADDRESS"];
                if (!string.IsNullOrEmpty(proxiedAddress))
                {
                    app.RunProxy(proxy => proxy.UseHttp(proxiedAddress));
                    //app.UseProxies(proxies =>
                    //{
                    //    // Bare string.
                    //    proxies.Map("echo/post", proxy => proxy.UseHttp("https://postman-echo.com/post"));

                    //    // Computed to task.
                    //    proxies.Map("api/comments/task/{postId}", proxy => proxy.UseHttp((_, args) => new ValueTask<string>($"https://jsonplaceholder.typicode.com/comments/{args["postId"]}")));

                    //    // Computed to string.
                    //    proxies.Map("api/comments/string/{postId}", proxy => proxy.UseHttp((_, args) => $"https://jsonplaceholder.typicode.com/comments/{args["postId"]}"));
                    //});

                    //app.RunProxy(context =>
                    //{
                    //    if (context.WebSockets.IsWebSocketRequest)
                    //        return "wss://mysite.com/ws";

                    //    return "https://mysite.com";
                    //});
                    //.UseWs((context, args) => "ws://mywsserver.com"));
                }
            }
            catch (Exception)
            {
            }

            app.UseHttpsRedirection();

            try
            {
                string csp = configuration["AppSettings:CSP"];
                if (!string.IsNullOrEmpty(csp))
                {
                    app.Use(async (context, next) =>
                    {
                        context.Response.Headers.Add("Content-Security-Policy", csp);
                        await next();
                    });

                }
            }
            catch (Exception)
            {

            }

            app.UseStaticFiles(GetStaticFileOptions());
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
                //FileProvider = new PhysicalFileProvider(Path.Combine(@"\\4-SMVNB-12\test", @"Resources")),
                RequestPath = new PathString("/Resources")
            });
            app.UseSpaStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseFileServer();

            //Sanitize models
            app.UseMiddleware<SanitizeMiddleware<Sanitizeable>>();
            //app.UseMiddleware<SanitizeMiddleware<OrderPortalContentViewModel>>();
            //app.UseMiddleware<SanitizeMiddleware<OrderPortalBannerViewModel>>();
            app.UseMiddleware<AntiXssMiddleware>();

            //Desanitize models after xss checking
            app.UseMiddleware<DesanitizeMiddleware<Sanitizeable>>();
            //app.UseMiddleware<DesanitizeMiddleware<OrderPortalContentViewModel>>();
            //app.UseMiddleware<DesanitizeMiddleware<OrderPortalBannerViewModel>>();

            app.UseMiddleware<ExceptionMiddleware>();
            /* Uncomment if cookie affinity doesn't fix the issue
            app.UseMiddleware<SignalRCookieIdMiddleware>();
            */
            //var names = Assembly.GetExecutingAssembly().GetManifestResourceNames().ToList();
            string swaggerEndpoint = configuration["AppSettings:SWAGGER_ENDPOINT"];
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Swagger UI - FRS";
                c.SwaggerEndpoint(swaggerEndpoint ?? "/swagger/v1/swagger.json", "FRS API V1");
                //c.IndexStream = () => Assembly.GetExecutingAssembly()
                //    .GetManifestResourceStream("FRS.swagger.ui.index.html");
            });

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller}/{action=Index}/{id?}");
            //});

            string enableSignalR = configuration["AppSettings:enableSignalR"];
            logger.LogInformation(LoggingEvents.SIGNALR_STATUS, string.Format("Signalr enabled: {0}", enableSignalR));
            if (string.IsNullOrEmpty(enableSignalR) || enableSignalR == "Y")
            {
                logger.LogInformation(LoggingEvents.SIGNALR_STATUS, string.Format("UseSignalR"));
                
                app.UseWebSockets();
                app.UseEndpoints(routes =>
                {
                    routes.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");

                    HttpTransportType transportType = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
                    routes.MapHub<FRSHub>("/hub/frs", options =>
                    {
                        options.Transports = transportType;
                    });

                    routes.MapHub<UserHub>("/hub/user", options =>
                    {
                        options.Transports = transportType;
                    });

                    routes.MapHub<EMSHub>("/hub/ems", options =>
                    {
                        options.Transports =
                            HttpTransportType.WebSockets
                            | HttpTransportType.LongPolling;
                        options.WebSockets.CloseTimeout = TimeSpan.FromMinutes(6);
                    });

                    routes.MapHub<LocationHub>("/hub/location", options =>
                    {
                        options.Transports =
                            HttpTransportType.WebSockets
                            | HttpTransportType.LongPolling;
                        options.WebSockets.CloseTimeout = TimeSpan.FromMinutes(6);
                    });

                    routes.MapHub<QueueHub>("/hub/queue", options =>
                    {
                        options.Transports =
                            HttpTransportType.WebSockets
                            | HttpTransportType.LongPolling;
                        options.WebSockets.CloseTimeout = TimeSpan.FromMinutes(6);
                    });

                    routes.MapHub<EmployeeScheduleHub>("/hub/employeeschedule", options =>
                    {
                        options.Transports =
                            HttpTransportType.WebSockets
                            | HttpTransportType.LongPolling;
                        options.WebSockets.CloseTimeout = TimeSpan.FromMinutes(6);
                    });

                    routes.MapHub<DeviceHub>("/hub/device", options =>
                    {
                        options.Transports =
                            HttpTransportType.WebSockets
                            | HttpTransportType.LongPolling;
                        options.WebSockets.CloseTimeout = TimeSpan.FromMinutes(6);
                    });

                    routes.MapHub<FRSDeviceHub>("/hub/frsdevice", options =>
                    {
                        options.Transports =
                            HttpTransportType.WebSockets
                            | HttpTransportType.LongPolling;
                        options.WebSockets.CloseTimeout = TimeSpan.FromMinutes(6);
                    });

                    routes.MapHub<PIBDeviceHub>("/hub/pibdevice", options =>
                    {
                        options.Transports =
                            HttpTransportType.WebSockets
                            | HttpTransportType.LongPolling;
                        options.WebSockets.CloseTimeout = TimeSpan.FromMinutes(6);
                    });

                    routes.MapHub<MeetingRoomHub>("/hub/meetingroom", options =>
                    {
                        options.Transports =
                            HttpTransportType.WebSockets
                            | HttpTransportType.LongPolling;
                        options.WebSockets.CloseTimeout = TimeSpan.FromMinutes(6);
                    });

                    //routes.MapHub<ReservationHub>("/hub/reservations", options =>
                    //{
                    //    options.Transports =
                    //        HttpTransportType.WebSockets |
                    //        HttpTransportType.LongPolling;
                    //});

                    //routes.MapHub<DeviceHub>("/hub/devices", options =>
                    //{
                    //    options.Transports =
                    //        HttpTransportType.WebSockets |
                    //        HttpTransportType.LongPolling;
                    //});
                });
            }

            // TODO:
            // Temporarily disblae for SATS version. This is not used
            //app.UseSoapEndpoint<IQueueService>("/Service.asmx", new BasicHttpBinding(), SoapSerializer.XmlSerializer);
            //app.UseEndpoints(endpoints => {
            //            endpoints.UseSoapEndpoint<IQueueService>(opt =>
            //            {
            //                opt.Path = "/Service.asmx";
            //                opt.SoapSerializer = SoapSerializer.DataContractSerializer;
            //            });
            //});

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501
                spa.Options.SourcePath = "ClientApp";

                //if (env.IsDevelopment())
                //{
                //    spa.UseAngularCliServer(npmScript: "start");
                //    spa.Options.StartupTimeout = TimeSpan.FromSeconds(240); // Increase the timeout if angular app is taking longer to startup
                //                                                            //spa.UseProxyToSpaDevelopmentServer("http://localhost:4200"); // Use this instead to use the angular cli server
                //}
            });

        }

        private StaticFileOptions GetStaticFileOptions()
        {
            //var p = new FileExtensionContentTypeProvider();
            //p.Mappings[".exe"] = "application/octet-stream";
            //return new StaticFileOptions { ContentTypeProvider = p };
            var staticFileOptions = new StaticFileOptions
            {
                // Configure options here if needed
                OnPrepareResponse = ctx =>
                {
                    // For example, set caching headers
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=86400");
                },
                ServeUnknownFileTypes = true,
                DefaultContentType = "application/octet-stream"
            };

            return staticFileOptions;
        }
    }

    //public class NamespaceSchemaFilter : ISchemaFilter
    //{
    //    public void Apply(Schema schema, SchemaFilterContext context)
    //    {
    //        if (schema is null)
    //        {
    //            throw new System.ArgumentNullException(nameof(schema));
    //        }

    //        if (context is null)
    //        {
    //            throw new ArgumentNullException(nameof(context));
    //        }

    //        schema.Title = context.SystemType.Name; // To replace the full name with namespace with the class name only
    //    }
    //}

    //public class MyEventHandlerA : IOpenIddictValidationEventHandler<OpenIddictValidationEvents.ValidateToken>
    //{
    //    async public Task<OpenIddictValidationEventState> HandleAsync(OpenIddictValidationEvents.ValidateToken notification)
    //    {
    //        return OpenIddictValidationEventState.Handled;
    //    }
    //}

    //public class MyEventHandlerB : IOpenIddictValidationEventHandler<OpenIddictValidationEvents.RetrieveToken>
    //{
    //    async public Task<OpenIddictValidationEventState> HandleAsync(OpenIddictValidationEvents.RetrieveToken notification)
    //    {
    //        return OpenIddictValidationEventState.Handled;
    //    }
    //}

    public class SignalRCookieIdMiddleware
    {
        private readonly RequestDelegate _next;

        public SignalRCookieIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context)
        {
            if (context.Request != null && context.Request.Path.Value.Contains("/frs/negotiate"))
            {
                if (!context.Request.Cookies.ContainsKey("_frs_cookie"))
                {
                    context.Response.Cookies.Append("_frs_cookie", "1");
                }
            }
            else if (context.Request != null && context.Request.Path.Value.Contains("/user/negotiate"))
            {
                if (!context.Request.Cookies.ContainsKey("_frs_cookie"))
                {
                    context.Response.Cookies.Append("_frs_cookie", "1");
                }
            }
            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
