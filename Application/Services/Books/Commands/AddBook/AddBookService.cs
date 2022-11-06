using Monitoring.Presistence.Contexts ;
using Monitoring.Common.Dto;
using System;
using Monitoring.Domain.Entities.Books;

namespace Monitoring.Application.Services.Books.Commands.AddBook
{
    public class AddBookService : IAddBookService
    {
        private readonly IDataBaseContext _context;

        public AddBookService(IDataBaseContext context)
        {
            _context = context;
        }

        public ResultDto<ResultAddBookDto> Execute(RequestAddBookDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return new ResultDto<ResultAddBookDto>()
                    {
                        Data = new ResultAddBookDto()
                        {
                            BookId = 0,
                        },
                        IsSuccess = false,
                        Message = "نام کتاب را وارد کنید",
                    };
                }

                if (request.Number <= 0)
                {
                    return new ResultDto<ResultAddBookDto>()
                    {
                        Data = new ResultAddBookDto()
                        {
                            BookId = 0,
                        },
                        IsSuccess = false,
                        Message = "شماره کتاب صحیح نیست",
                    };
                }               

                Book Book = new Book()
                {
                   Name = request.Name,
                    Number = request.Number,
                };

                _context.Books.Add(Book);
                _context.SaveChanges();

                return new ResultDto<ResultAddBookDto>()
                {
                    Data = new ResultAddBookDto()
                    {
                        BookId = Book.Id,
                    },
                    IsSuccess = true,
                    Message = "ثبت با موفقیت صورت گرفت",
                };
            }
            catch (Exception)
            {
                return new ResultDto<ResultAddBookDto>()
                {
                    Data = new ResultAddBookDto()
                    {
                        BookId = 0,
                    },
                    IsSuccess = false,
                    Message = "ثبت با خطا روبرو شد",
                };
            }
        }
    }
}
