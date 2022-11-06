using Monitoring.Presistence.Contexts ;
using System;
using System.Collections.Generic;
using System.Linq;
using Monitoring.Common;
using Monitoring.Common.Dto;

namespace Monitoring.Application.Services.Books.Queries.GetBook
{
    public class GetBookService : IGetBookService
    {
        private readonly IDataBaseContext _context;
        public GetBookService(IDataBaseContext context)
        {
            _context = context;
        }

        public ResultDto<ResultGetBookDto> Execute(RequestGetBookDto request)
        {
            try
            {                
                var Books = _context.Books.AsQueryable();                

                int rowsCount;
                var BooksList = Books.ToPaged(request.PageNumber, request.PageSize, out rowsCount).Select(p => new GetBookDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Number = p.Number,
                }).ToList();

                return new ResultDto<ResultGetBookDto>()
                {
                    Data = new ResultGetBookDto()
                    {
                        Rows = rowsCount,
                        Books = BooksList,
                    },

                    IsSuccess = true,
                    Message = "",
                };
            }
            catch (Exception)
            {
                return new ResultDto<ResultGetBookDto>()
                {
                    Data = new ResultGetBookDto()
                    {
                        Rows = 0,
                        Books = null,
                    },
                    IsSuccess = false,
                    Message = "دریافت لیست با خطا روبرو شد",
                };
            }
        }
    }
}
