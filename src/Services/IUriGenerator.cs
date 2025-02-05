using API.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace API.Services
{
	public interface IUriGenerator
	{		
		Dictionary<LinkType, string> GeneratePaginationLinks<T>(IPaginatedResponseModel<T> paginationResponseModel, HttpContext httpContext);
	}
}
