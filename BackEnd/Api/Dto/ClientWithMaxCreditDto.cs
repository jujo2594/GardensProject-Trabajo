using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dto
{
    public class ClientWithMaxCreditDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public double CreditLimit { get; set; }
    }
}