using BOG.CollectorEndPoint.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.IO;

namespace BOG.CollectorEndPoint
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IAssemblyVersion, AssemblyVersion>();

			services.AddControllers();
			services.AddRouting();
			services.AddHttpContextAccessor();

			// Register the Swagger generator, defining one or more Swagger documents
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
				{
					Version = $"v{this.GetType().Assembly.GetName().Version}",
					Title = "BOG.CollectorEndPoint API",
					Description = "An event catcher for a Rest API call, to example the request.  Always returns 200",
					Contact = new Microsoft.OpenApi.Models.OpenApiContact { Name = "John J Schultz", Email = "", Url = new Uri("https://github.com/rambotech") },
					License = new Microsoft.OpenApi.Models.OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
				});
				// Set the comments path for the Swagger JSON and UI.
				var xmlPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "BOG.CollectorEndPoint.xml");
				c.IncludeXmlComments(xmlPath);
			});

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();
			app.UseAuthorization();
			app.UseCookiePolicy();
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "BOG.CollectiorEndPoint API v1");
			});

			app.Use((context, next) =>
			{
				context.Response.Headers.Add("X-Server-App", $"BOG.CollectiorEndPoint v{new AssemblyVersion().Version}");
				return next();
			});


			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
