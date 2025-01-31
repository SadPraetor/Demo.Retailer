using API.Models;
using System.Collections.Generic;

namespace API.Services
{
	public interface IUriGenerator
	{
		Dictionary<LinkType, string> GeneratePaginationLinks<T>(IPaginatedResponseModel<T> paginationResponseModel, string path);
	}
}
