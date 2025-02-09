using System.Collections.Generic;

namespace Demo.Retailer.Api.Models
{
	public class PaginatedResponseModel<T> : IPaginatedResponseModel<T>
	{
		const int MaxPageSize = 100;
		private int _pageSize;
		public int PageSize
		{
			get => _pageSize;
			set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
		}

		public int CurrentPage { get; set; }
		public int TotalItems { get; set; }
		public int TotalPages { get; set; }

		public IDictionary<LinkType, string> Links { get; set; }

		public IList<T> Data { get; set; }

		public PaginatedResponseModel()
		{
			Data = new List<T>();
		}
	}
}
