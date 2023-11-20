using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dto
{
    public class ClientEmployeeInfoDto
    {
        public string ClientName { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeLastname { get; set; }
        public string OfficeCity { get; set; }
    }
}