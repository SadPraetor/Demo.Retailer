using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq
{
	public static class PaginationExtension
	{
		public static async Task<PaginatedResponseModel<TModel>> PaginateAsync<TModel>(
				this IQueryable<TModel> query,
				int page,
				int pageSize,
				CancellationToken cancellationToken)
				where TModel : class
		{

			var paged = new PaginatedResponseModel<TModel>();

			paged.TotalItems = await query.CountAsync(cancellationToken);

			paged.TotalPages = (int)Math.Ceiling(paged.TotalItems / (double)pageSize);

			if (paged.TotalPages < page)
			{
				throw new PageOutOfRangeException();
			}


			paged.CurrentPage = page;
			paged.PageSize = pageSize;


			var skip = (page - 1) * pageSize;
			paged.Data = await query
					   .Skip(skip)
					   .Take(pageSize)
					   .ToListAsync(cancellationToken);

			return paged;
		}
	}
}

