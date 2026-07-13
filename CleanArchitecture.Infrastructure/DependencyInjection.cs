using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Services.Identity;
using CleanArchitecture.Application.Common.Services.Localization;
using CleanArchitecture.Infrastructure.Identity;
using CleanArchitecture.Infrastructure.Identity.Services;
using CleanArchitecture.Infrastructure.Localization;
using CleanArchitecture.Infrastructure.Persistence.Context;
using CleanArchitecture.Infrastructure.Persistence.Repositories;
using CleanArchitecture.Infrastructure.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Registration of Database Context
            services.AddDbContext<AppDbContext>(option => 
                option.UseSqlServer(configuration.GetConnectionString("context")));

            // Registration of Identity
            services
            .AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // Registration of Identity Service
            services.AddScoped<IIdentityService, IdentityService>();

            // Registration of Localization Service
            services.AddScoped<ILocalizationService, LocalizationService>();

            // Registration of Generic Repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Registrarion of Repositories
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

            // Registration of Unit Of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
