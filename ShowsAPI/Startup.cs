using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShowsAPI.Contexts;
using ShowsAPI.Extensions;
using ShowsAPI.Persistence;
using Swashbuckle.AspNetCore.Swagger;

namespace ShowsAPI
{
    public class Startup
    {
        private readonly ILogger _logger;
        public Startup(IConfiguration configuration, ILoggerFactory logerFactory)
        {
            Configuration = configuration;
            _logger = logerFactory.CreateLogger<Startup>();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddLogging();

            services.AddDbContext<ShowsContext>(x => x.UseSqlServer(Configuration.GetConnectionString("ShowsDatabase")));

            services.AddScoped<IShowsRepository, ShowsRepository>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My Shows API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureExceptionHandler(_logger);

            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Shows API V1"); });
        }
    }
}
