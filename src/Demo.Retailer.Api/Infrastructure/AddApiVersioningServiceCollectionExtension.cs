using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Retailer.Api.Infrastructure
{
	public static class AddApiVersioningServiceCollectionExtension
	{
		public static IServiceCollection AddProjectApiVersions(this IServiceCollection services)
		{
			services.AddApiVersioning(options =>
			{
				options.DefaultApiVersion = new ApiVersion(1);
				options.ReportApiVersions = true;
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.ApiVersionReader = new UrlSegmentApiVersionReader();
			})
			.AddApiExplorer(o =>
			{
				o.GroupNameFormat = "'v'VVV";
				o.SubstituteApiVersionInUrl = true;
			});
			return services;
		}
	}
}
