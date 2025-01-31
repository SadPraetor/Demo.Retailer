using API.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;

namespace API.Services
{
	public class UriGenerator : IUriGenerator
	{
		private static readonly string _pageQueryParameterName = nameof(Pagination.Page).ToLower();
		private static readonly string _sizeQueryParameterName = nameof(Pagination.Size).ToLower();

		public Dictionary<LinkType, string> GeneratePaginationLinks<T>(IPaginatedResponseModel<T> paginationResponseModel, string path)
		{



			//.net core 3.1, system.text.json does not support serializing of Ttype enum as dictionary key. Shame
			//need to use ToString, with .net 5 Dictionary<LinkType,string> is good to be used
			Dictionary<LinkType, string> resourceLinks = null;

			if (paginationResponseModel.TotalPages > paginationResponseModel.CurrentPage)
			{
				resourceLinks ??= new Dictionary<LinkType, string>();
				resourceLinks[LinkType.Next] = QueryHelpers.AddQueryString(
					path, new Dictionary<string, string>() {
						{_sizeQueryParameterName, paginationResponseModel.PageSize.ToString() },
						{ _pageQueryParameterName, (paginationResponseModel.CurrentPage + 1).ToString() }
					});

			}

			if (paginationResponseModel.CurrentPage > 1)
			{
				resourceLinks ??= new Dictionary<LinkType, string>();
				resourceLinks[LinkType.Prev] = QueryHelpers.AddQueryString(
					path, new Dictionary<string, string>() {
						{  _sizeQueryParameterName, paginationResponseModel.PageSize.ToString() },
						{ _pageQueryParameterName, (paginationResponseModel.CurrentPage - 1).ToString() }
					});
			}



			return resourceLinks;
		}
	}
}
