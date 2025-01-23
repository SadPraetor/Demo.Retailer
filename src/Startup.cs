using API.DataAccess;
using API.Services;
using API.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RetailerInterviewAPITask.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;

namespace RetailerInterviewAPITask {
    public class Startup {
        public Startup( IConfiguration configuration ) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices( IServiceCollection services ) {

            services.AddDbContext<ProductsDbContext>( options =>
                 options.UseSqlServer( Configuration.GetConnectionString( "ProductsDb" ) ) 
            );

            services.AddControllers( options => options.InputFormatters.Insert( 0, new RawRequestBodyFormatter() ) );
               

            services.AddApiVersioning( options => {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion( 1, 0 );
                options.ReportApiVersions = true;
                options.ApiVersionReader = new HeaderApiVersionReader( "X-Api-Version" );
            }
           );

            services.AddVersionedApiExplorer( options => {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";


            } );
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen( options => {
                options.OperationFilter<SwaggerDefaultValues>();
                var filePath = Path.Combine( System.AppContext.BaseDirectory, "Api.xml" );
                options.IncludeXmlComments( filePath );
            } 
            );


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //might need to be adjusted in scenario of proxy (X-Forwarded-For, X-Forwarded-Path)
            //services.AddSingleton<IUriGenerator,UriGenerator>(provider => {
            //    var accessor = provider.GetRequiredService<IHttpContextAccessor>();
            //    var request = accessor.HttpContext.Request;
            //    var absoluteUri = $"{request.Scheme}://{request.Host.ToUriComponent()}";
            //    return new UriGenerator( absoluteUri );
            //} );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider ) {
            if ( env.IsDevelopment() ) {
                app.UseDeveloperExceptionPage();
                
            }

            app.UseSwagger( options => options.RouteTemplate = "swagger/{documentName}/swagger.json" );
            app.UseSwaggerUI(
            options => {
                // build a swagger endpoint for each discovered API version
                foreach ( var description in provider.ApiVersionDescriptions ) {
                    options.SwaggerEndpoint( $"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant() );
                }
            } );

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints( endpoints => {
                endpoints.MapControllers();
            } );
        }
    }
}
