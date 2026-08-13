using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Services.DatabaseException;
using CleanArchitecture.Infrastructure.Persistence.Context;
using CleanArchitecture.Infrastructure.Persistence.Repositories;
using CleanArchitecture.Infrastructure.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Registration of Database Context
            services.AddDbContext<AppDbContext>(option =>
                option.UseSqlServer(configuration.GetConnectionString("context")));

            // Registration of Generic Repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));


            // Registrarion of Repositories
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();


            // Registration of Unit Of Work
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();


            // Registration of Database Exception Service
            services.AddScoped<IDatabaseExceptionService, SqlServerDatabaseExceptionService>();


            return services;
        }
    }
}
