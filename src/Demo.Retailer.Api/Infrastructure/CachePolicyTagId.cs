using Microsoft.AspNetCore.OutputCaching;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Retailer.Api.Infrastructure
{
	public class CachePolicyTagId : IOutputCachePolicy
	{
		public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellation)
		{
			var id = context.HttpContext.Request.RouteValues["id"];

			if (id is null)
			{
				return ValueTask.CompletedTask;
			}

			context.Tags.Add($"tag-id-{id}");

			context.EnableOutputCaching = true;
			context.AllowCacheLookup = true;
			context.AllowCacheStorage = true;
			context.AllowLocking = true;
			context.CacheVaryByRules.RouteValueNames = "id";



			return ValueTask.CompletedTask;
		}

		public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellation)
		{
			return ValueTask.CompletedTask;
		}

		public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellation)
		{
			return ValueTask.CompletedTask;
		}
	}
}
