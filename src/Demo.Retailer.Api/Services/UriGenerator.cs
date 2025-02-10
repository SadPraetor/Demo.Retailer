using Demo.Retailer.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;


namespace Demo.Retailer.Api.Services
{
	public class UriGenerator : IUriGenerator
	{
		private static readonly string _pageQueryParameterName = nameof(Pagination.Page).ToLower();
		private static readonly string _sizeQueryParameterName = nameof(Pagination.Size).ToLower();
		private readonly LinkGenerator _linkGenerator;


		public UriGenerator(LinkGenerator linkGenerator)
		{
			_linkGenerator = linkGenerator;
		}

		public Dictionary<LinkType, string> GeneratePaginationLinks<T>(IPaginatedResponseModel<T> paginationResponseModel, HttpContext httpContext)
		{

			Dictionary<LinkType, string> resourceLinks = null;

			if (paginationResponseModel.TotalPages > paginationResponseModel.CurrentPage)
			{
				resourceLinks ??= new Dictionary<LinkType, string>();
				resourceLinks[LinkType.Next] = _linkGenerator.GetUriByName(httpContext, "paginated_products", new { size = paginationResponseModel.PageSize, page = paginationResponseModel.CurrentPage + 1, });
			}

			if (paginationResponseModel.CurrentPage > 1)
			{
				resourceLinks ??= new Dictionary<LinkType, string>();
				resourceLinks[LinkType.Prev] = _linkGenerator.GetUriByName(httpContext, "paginated_products", new { size = paginationResponseModel.PageSize, page = paginationResponseModel.CurrentPage - 1 });
			}



			return resourceLinks;
		}
	}
}
