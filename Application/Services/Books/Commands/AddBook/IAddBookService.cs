using Monitoring.Common.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Monitoring.Application.Services.Books.Commands.AddBook
{
    public interface IAddBookService
    {
        ResultDto<ResultAddBookDto> Execute(RequestAddBookDto request);
    }
}
