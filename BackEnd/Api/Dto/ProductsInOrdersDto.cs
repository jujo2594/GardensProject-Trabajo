using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dto
{
    public class ProductsInOrdersDto
    {
        public string IdProduct { get; set; }
        public int IdOrder { get; set; }
        public string ProductName { get; set; }
    }
}