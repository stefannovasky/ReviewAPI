using System.Collections.Generic;

namespace ReviewApi.Application.Models
{
    public class PaginationResponseModel<T>
    {
        public IEnumerable<T> Data { get; set; }
        public string NextPage { get; set; }
        public string PreviousPage { get; set; }
        public int Total { get; set; }
    }
}
