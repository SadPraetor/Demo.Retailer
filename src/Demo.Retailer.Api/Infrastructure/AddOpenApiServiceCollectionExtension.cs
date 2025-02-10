using Demo.Retailer.Api.Models;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Retailer.Api.Infrastructure
{
	public static class AddOpenApiServiceCollectionExtension
	{
		public static IServiceCollection RegisterOpenApi(this IServiceCollection services)
		{
			services.AddOpenApi("v1");
			services.AddOpenApi("v2", options =>
			{
				options.AddOperationTransformer((operation, context, cancellationToken) =>
				{
					if (context.Description.ActionDescriptor.EndpointMetadata
					.OfType<IParameterBindingMetadata>()
					.Any(x => x.ParameterInfo.ParameterType == typeof(Pagination)))
					{
						operation.Parameters ??= [];
						operation.Parameters.Add(new OpenApiParameter()
						{
							Name = nameof(Pagination.Page).ToLower(),
							In = ParameterLocation.Query,
							Required = false,
							Schema = new OpenApiSchema() { Type = "integer", Minimum = 1, Format = "int32" }

						});
						operation.Parameters.Add(new OpenApiParameter()
						{
							Name = nameof(Pagination.Size).ToLower(),
							In = ParameterLocation.Query,
							Required = false,
							Schema = new OpenApiSchema() { Type = "integer", Format = "int32", Description = "Items to retrieve. Limited to maximum 100", Maximum = 100, Minimum = 1 }
						});
					}
					return Task.CompletedTask;
				});
			});

			return services;
		}
	}
}
