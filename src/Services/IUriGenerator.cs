using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services {
    public interface IUriGenerator {
        Dictionary<LinkType, string> GeneratePaginationLinks<T>( IPaginatedResponseModel<T> paginationResponseModel, string path );
    }
}
