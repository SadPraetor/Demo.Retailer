using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;

namespace API.Models
{
	public record Pagination 
    {

        /// <summary> 
        /// Number of items returned in one request 
        /// </summary>            
        public int Size { get; set; } = 10;
        /// <summary> 
        /// Requested page 
        /// </summary>         
        public int Page { get; set; } = 1;

        public Pagination(int size , int page) {
            Size = size;
            Page = page;
        }

        public Pagination() {

        }

		

        private static readonly string _pageParameterName = nameof(Pagination.Page).ToLower();
        private static readonly string _sizeParameterName = nameof(Pagination.Size).ToLower();

		public static ValueTask<Pagination?> BindAsync(HttpContext context,ParameterInfo parameter)
		{
            StringValues pageValue;
            StringValues sizeValue;
            int page = 0;
            int size = 0;

            if(context.Request.Query.TryGetValue(_pageParameterName, out pageValue))
            {
                if(!int.TryParse(pageValue, out page))
                {
                    throw new FaultyPaginationQueryException("Page must be number and be greater than 0");
                }
            }
			else
			{
                page = 1;
			}

           

			if (context.Request.Query.TryGetValue(_sizeParameterName, out sizeValue))
			{
				if (!int.TryParse(sizeValue, out size))
				{
					throw new FaultyPaginationQueryException("Size must be number and be greater than 0");
				}
			}
            else
            {
                size = 50;
            }


			return ValueTask.FromResult<Pagination?>(new Pagination(size,page));
		}
	}
}
