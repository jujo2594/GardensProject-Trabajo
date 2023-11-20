using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dto
{
    public class ClientPurchaseDateDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public DateOnly OrderDate { get; set; }
    }
}