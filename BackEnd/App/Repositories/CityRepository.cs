using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

namespace App.Repositories;
public class CityRepository : GenericRepository<City>, ICity
{
    private readonly GardensContext _context;

    public CityRepository(GardensContext context) : base(context)
    {
        _context = context;
    }
}