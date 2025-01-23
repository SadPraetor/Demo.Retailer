using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Diagnostics.CodeAnalysis;

namespace API.Models
{
	public record Pagination : IParsable<Pagination>
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

		public static Pagination Parse(string s, IFormatProvider provider)
		{
			if(TryParse(s, provider, out var pagination))
            {
                return pagination;
            }
            throw new FormatException("Query string conditions for pagination not valid");
		}

		public static bool TryParse([NotNullWhen(true)] string s, IFormatProvider provider, [MaybeNullWhen(false)] out Pagination result)
		{
			var queryParameters = QueryHelpers.ParseQuery(s);

          
            var found =  queryParameters.TryGetValue(nameof(Pagination.Page),out var page) &
                queryParameters.TryGetValue(nameof(Pagination.Size), out var size);

            if(!found)
            {
                result = null;
                return false;
            }

            var canParse = int.TryParse(page, out var pageParsed) &
                int.TryParse(size, out var sizeParsed);

            if(!canParse)
            {
                result = null;
                return false;
            }

            result =  new Pagination(sizeParsed,pageParsed);
            return true;
		}
	}
}
