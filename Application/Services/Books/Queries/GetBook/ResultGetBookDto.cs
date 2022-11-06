using System.Collections.Generic;

namespace Monitoring.Application.Services.Books.Queries.GetBook
{
    public class ResultGetBookDto
    {
        public List<GetBookDto> Books { get; set; }
        public int Rows { get; set; }
    }
}
