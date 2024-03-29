using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class State : BaseEntityInt
    {
        [Required]
        public string Name { get; set; }


        [Required]
        public int IdCountryFk { get; set; }
        public Country Countries { get; set; }

        public ICollection<City> Cities { get; set; }
    }
}