using Monitoring.Common.Dto;
using System.Collections.Generic;
using System.Text;

namespace Monitoring.Application.Services.Books.Queries.GetBook
{
    public interface IGetBookService
    {
        ResultDto<ResultGetBookDto> Execute(RequestGetBookDto request);
    }
}
