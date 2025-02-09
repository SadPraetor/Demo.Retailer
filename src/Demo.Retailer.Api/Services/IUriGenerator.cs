using Demo.Retailer.Api.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Demo.Retailer.Api.Services
{
	public interface IUriGenerator
	{		
		Dictionary<LinkType, string> GeneratePaginationLinks<T>(IPaginatedResponseModel<T> paginationResponseModel, HttpContext httpContext);
	}
}
