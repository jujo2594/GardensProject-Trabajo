using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dto
{
    public class CityEmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalEmployees { get; set; }
    }
}