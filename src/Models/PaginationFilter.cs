namespace API.Models
{
	public class PaginationFilter {

        public int Size { get; init; } = 50;
        public int Page { get; init; } = 1;
        public PaginationFilter() 
        {
            
        }

        public PaginationFilter(int pageSize, int page)
		{
			TestValues(pageSize, page);
			Size = pageSize > 100 ? 100 : pageSize;
			Page = page;
		}

		private void TestValues(int size, int page)
		{
			if (page < 1)
			{
				throw new FaultyPaginationQueryException("Page number must be greater than 0");
			}

			if (size < 1)
			{
				throw new FaultyPaginationQueryException("Page size must be greater than 0");
			}

		}

		public PaginationFilter(Pagination pagination)  
        {
            if ( pagination == null ) 
            {
                return;
            } 
			TestValues(pagination.Size, pagination.Page);	
			Size = pagination.Size > 100 ? 100 : pagination.Size;
			Page = pagination.Page;
            
        }
    }
}
