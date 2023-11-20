using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dto;

public class ClientWithoutPurchasesDto
{
    public string ClientName { get; set; }
    public string EmployeeName { get; set; }
    public string EmployeeLastname { get; set; }
    public string OfficePhone { get; set; }
}
