using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dto
{
    public class EmployeeWithoutClientDto
    {
        public string Name { get; set; }
        public string LastNameOne { get; set; }
        public string LastNameTwo { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
    }
}