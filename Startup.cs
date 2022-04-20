using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json.Serialization;

namespace MalVirDetector_CLI_API
{
	public class Startup
	{
		public IConfigurationRoot Configuration;
		public static string ConnectionString { get; private set; }
		public Startup(IHostEnvironment env, IConfiguration configuration)
		{
			Configuration = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			//jwt auth
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
		   .AddJwtBearer(options =>
		   {
			   options.TokenValidationParameters = new TokenValidationParameters
			   {
				   ValidateIssuer = true,
				   ValidateAudience = true,
				   ValidateLifetime = true,
				   ValidateIssuerSigningKey = true,
				   ValidIssuer = Configuration["Jwt:Issuer"],
				   ValidAudience = Configuration["Jwt:Issuer"],
				   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
			   };
		   });

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


			//add cors
			services.AddCors(options =>
			{
				options.AddPolicy("AllowAll",
					builder => builder
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowAnyOrigin()
					.WithOrigins("http://localhost:4200", "https://MalVirDetector.com")
					.AllowCredentials()
					);
			});
			services.AddSignalR().AddJsonProtocol(opt => { opt.PayloadSerializerOptions.PropertyNamingPolicy = null; });

			services.AddMvc(options => options.Filters.Add(new AuthorizeFilter()));
			services.AddControllersWithViews().SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
			.AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseDefaultFiles();
			app.UseStaticFiles();
			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Documents")),
				RequestPath = new PathString("/Documents")
			});

			app.UseRouting();
			app.UseCors("AllowAll");
			app.UseCookiePolicy();

			app.UseAuthentication();
			app.UseAuthorization();


			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});

			ConnectionString = Configuration.GetConnectionString("conn");
		}
	}
}
