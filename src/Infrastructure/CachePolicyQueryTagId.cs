using Microsoft.AspNetCore.OutputCaching;
using System.Threading;
using System.Threading.Tasks;

namespace API.Infrastructure
{
	public class CachePolicyQueryTagId : IOutputCachePolicy
	{
		public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellation)
		{
			context.EnableOutputCaching = true;
			context.AllowCacheLookup = true;
			context.AllowCacheStorage = true;
			context.AllowLocking = true;
			context.CacheVaryByRules.QueryKeys = new Microsoft.Extensions.Primitives.StringValues(new[] { "page", "size" });
			return ValueTask.CompletedTask;
		}

		public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellation)
		{
			return ValueTask.CompletedTask;
		}

		public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellation)
		{
			if (context.HttpContext.Items.TryGetValue("cache-tag-ids", out var obj) && obj is string[] tags)
			{
				// Assign these tags to the cache metadata.
				foreach(var tag in tags)
				{
					context.Tags.Add(tag);
				}
			}
			return ValueTask.CompletedTask;
		}
	}
}
