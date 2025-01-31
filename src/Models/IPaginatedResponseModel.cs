using System.Collections.Generic;

namespace API.Models
{
	public interface IPaginatedResponseModel<T>
	{
		int CurrentPage { get; set; }
		IList<T> Data { get; set; }
		IDictionary<LinkType, string> Links { get; set; }
		int PageSize { get; set; }
		int TotalItems { get; set; }
		int TotalPages { get; set; }
	}
}