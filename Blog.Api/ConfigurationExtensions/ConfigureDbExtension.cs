using Blog.Api.Configurations;
using Blog.Core.Data.Repositories;
using Blog.Data.Repositories;
using Blog.Data.Repositories.MongoRepository;
using Blog.Data.Repositories.Posts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Api.ConfigurationExtensions
{
    public static class ConfigureDbExtension
    {
        public static IServiceCollection ConfigureDb(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoDbSettings = new MongoDbSettings();
            configuration.Bind("MongoDb", mongoDbSettings);
            services.AddSingleton(mongoDbSettings);

            ConfigureRepositories(services);

            return services;
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddScoped<IMongoRepository, MongoRepository>();
            services.AddTransient<IPostsRepository, PostsRepository>();
            services.AddTransient<IAuthorsRepository, AuthorsRepository>();
            services.AddTransient<ICommentsRepository, CommentsRepository>();
        }
    }
}