using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dto
{
    public class ProductMaxStockDto
    {
        public string Id { get; set; }
        public string ProductName { get; set; }
        public int MaxStock { get; set; }
    }
}