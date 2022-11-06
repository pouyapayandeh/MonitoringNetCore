using System;
using System.Collections.Generic;
using System.Text;

namespace Monitoring.Application.Services.Books.Commands.AddBook
{
    public class RequestAddBookDto
    {
        public string Name { get; set; }
        public int Number { get; set; }
    }
}
