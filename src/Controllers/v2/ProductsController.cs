using API.DataAccess;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace RetailerInterviewAPITask.Controllers {
   
    public partial class ProductsController : ControllerBase {

        /// <summary>
        /// Returns paginated model with list of products as member
        /// </summary>
        /// <response code="200">Products found and returned</response> 
        /// <response code="400">Bad Request, most likely wrong pagination query</response>
        /// <response code="404">Requested page not found</response>
        [HttpGet(Name =nameof(GetAllAsync20))]
        [MapToApiVersion( "2.0" )]
        [Produces( "application/json" )]
        [ProducesResponseType( StatusCodes.Status200OK, Type = typeof( PaginatedResponseModel<Product> ) )]
        [ProducesResponseType( StatusCodes.Status404NotFound, Type = typeof( ExceptionDto ) )]
        [ProducesResponseType( StatusCodes.Status400BadRequest, Type = typeof( ExceptionDto ) )]
        public async Task<ActionResult<PaginatedResponseModel<Product>>> GetAllAsync20( 
            [FromQuery] Pagination paginationQuery, CancellationToken cancellationToken ) 
        {
            
            try {

                var paginationFilter = new PaginationFilter( paginationQuery );

                var paginatedModel =   await _productsDbContext
                    .Products
                    .AsNoTracking()
                    .OrderBy(x=>x.Id)
                    .PaginateAsync<Product>( paginationFilter.Page, paginationFilter.Size, cancellationToken ) ;

                var path = Url.RouteUrl( nameof( GetAllAsync20 ) );

                paginatedModel.Links = _uriGenerator.GeneratePaginationLinks<Product>(paginatedModel, path );

                return Ok(paginatedModel);

            }
            catch ( PageOutOfRangeException exception) {
                return NotFound( new ExceptionDto( exception ) );
            }
            catch ( FaultyPaginationQueryException exception ) {              
                return BadRequest( new ExceptionDto(exception) );
            }
            catch(Exception exception ) {                
                return StatusCode( StatusCodes.Status500InternalServerError,new ExceptionDto(exception) );
            }
        }       
    }
}
