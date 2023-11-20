using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dto;
using AutoMapper;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Api.Controllers
{
    public class Consult3Controller : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly GardensContext _context;

        public Consult3Controller(IUnitOfWork unitOfWork, IMapper mapper, GardensContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("CityEmployees")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CityEmployeeDto>> GetCityEmployees()
        {
            var results = await (from tcity in _context.Cities
            join tofficeAddress in _context.OfficesAddresses on tcity.Id equals tofficeAddress.IdCityFk
            join toffice in _context.Offices on tofficeAddress.IdOfficeFk equals toffice.Id
            join temployee in _context.Employees on toffice.Id equals temployee.IdOfficeFk
            group new {tcity.Id, tcity.Name} by new {tcity.Id, tcity.Name} into g
            select new CityEmployeeDto
            {
                Id = g.Key.Id,
                Name = g.Key.Name,
                TotalEmployees = g.Count()
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ClientWithEmployeeInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ClientEmployeeInfoDto>>> GetClientWithEmployeeInfo()
        {
            var results = await (from _employee in _context.Employees
            join _office in _context.Offices on _employee.IdOfficeFk equals _office.Id
            join _client in _context.Clients on _employee.Id equals _client.IdEmployeeFk
            join _officeAddress in _context.OfficesAddresses on _office.Id equals _officeAddress.IdOfficeFk
            join _city in _context.Cities on _officeAddress.IdCityFk equals _city.Id
            select new ClientEmployeeInfoDto
            {
                ClientName = _client.Name,
                EmployeeName = _employee.Name,
                EmployeeLastname = _employee.LastNameOne,
                OfficeCity = _city.Name
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ClientWithoutPurchases")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ClientWithoutPurchasesDto>>> GetClientWithoutPurchases()
        {
            var results = await (from _client in _context.Clients
            join _employee in _context.Employees on _client.IdEmployeeFk equals _employee.Id
            join _office in _context.Offices on _employee.IdOfficeFk equals _office.Id
            join _payment in _context.Payments on _client.Id equals _payment.IdClientFk into Payments
            from _payment in Payments.DefaultIfEmpty()
            where _payment == null
            select new ClientWithoutPurchasesDto
            {
                ClientName = _client.Name,
                EmployeeName = _employee.Name,
                EmployeeLastname = _employee.LastNameOne,
                OfficePhone = _office.Phone,
            }
            )
            .ToListAsync();
            return Ok(results);
        }

        [HttpGet("ClientPurchaseDate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ClientPurchaseDateDto>>> GetClientPurchaseDate()
        {
            var results = await (from _client in _context.Clients
                let OrderDate = (from _order in _context.Orders
                                where _client.Id == _order.IdClientFk
                                select _order.OrderDate).FirstOrDefault()
                where OrderDate.Year == 2008
                orderby _client.Name ascending
            select new ClientPurchaseDateDto
            {
                Id = _client.Id,
                ClientName = _client.Name,
                OrderDate = OrderDate,
            }
            )
            .ToListAsync();
            return Ok(results);
        }

        [HttpGet("EmployeeWithoutClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<EmployeeWithoutClientDto>>> GetEmployeeWithoutClient()
        {
            var results = await (from temployee in _context.Employees
            join tpositionEmployee in _context.PositionsEmployees on temployee.IdPositionFk equals tpositionEmployee.Id
            join toffice in _context.Offices on temployee.IdOfficeFk equals toffice.Id
            join tclient in _context.Clients on temployee.Id equals tclient.IdEmployeeFk into Clients
            from tclient in Clients.DefaultIfEmpty()
            where temployee.IdPositionFk == 3 && tclient == null
            select new EmployeeWithoutClientDto
            {
                Name = temployee.Name,
                LastNameOne = temployee.LastNameOne,
                LastNameTwo = temployee.LastNameTwo,
                Position = tpositionEmployee.Name,
                Phone = toffice.Phone
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ClientsWithTotalPayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ClientsTotalPaymentDto>>> GetClientsWithTotalPayments()
        {
            var results = await (from _payment in _context.Payments
            join _client in _context.Clients on _payment.IdClientFk equals _client.Id
            group _payment by new {_client.Id, _client.Name} into g
            select new ClientsTotalPaymentDto
            {
                Id = g.Key.Id,
                Name = g.Key.Name,
                TotalPayment = g.Sum(_payment => _payment.Total)
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ClientTotalOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ClientsTotalPaymentDto>>> GetClientsTotalOrders()
        {
            var results = await (from _order in _context.Orders
            join _client in _context.Clients on _order.IdClientFk equals _client.Id
            group _order by new {_client.Id, _client.Name} into g
            select new ClientsTotalPaymentDto
            {
                Id = g.Key.Id,
                Name = g.Key.Name,
                TotalPayment = g.Count()
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ProductInOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProductsInOrdersDto>>> GetProductInOrder()
        {
            var results = await (from _orderDetail in _context.OrdersDetails
            join _product in _context.Products on _orderDetail.IdProductFk equals _product.Id
            select new ProductsInOrdersDto
            {
                IdProduct = _orderDetail.IdProductFk,
                IdOrder = _orderDetail.IdOrderFk,
                ProductName = _product.Name
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ProductNotInOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProductsNotInOrderDto>>> GetProductNotInOrder()
        {
            var results = await (from _product in _context.Products
            join _orderDetail in _context.OrdersDetails on _product.Id equals _orderDetail.IdProductFk into _orderDetailGroup
            from _orderdetail in _orderDetailGroup.DefaultIfEmpty()
            where !_context.Products.Any(prod => prod.Id == _orderdetail.IdProductFk)
            select new ProductsNotInOrderDto
            {
                IdProduct = _product.Id,
                ProductName = _product.Name
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ClientWithPayments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ClientWithPaymentDto>>> GetClientsWithPayments()
        {
            var results = await (from _client in _context.Clients
                where _context.Payments.Any(p => p.IdClientFk == _client.Id)
                select new {_client.Id, _client.Name}).ToListAsync();
            return  Ok(results);
        }

        [HttpGet("ClientWithoutPayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ClientWithoutPaymentDto>>> GetClientWithoutPayment()
        {
            var results = await (from _client in _context.Clients
            join _payment in _context.Payments on _client.Id equals _payment.IdClientFk into _clientPayment
            from _payment in _clientPayment.DefaultIfEmpty()
            where !_context.Clients.Any(client => client.Id == _payment.IdClientFk)
            select new ClientWithoutPaymentDto
            {
                Id = _client.Id,
                ClientName = _client.Name
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ClientWithOrderNoPayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ClientWithOrderNoPaymentDto>>> GetClientWitOrderNoPayment()
        {
            var results = await (from _client in _context.Clients
            join _order in _context.Orders on _client.Id equals _order.IdClientFk 
            join _payment in _context.Payments on _client.Id equals _payment.IdClientFk into _paymentGroup
            from pg in _paymentGroup.DefaultIfEmpty()
            where _context.Orders.Any(o => o.IdClientFk == _client.Id) && pg == null
            select new ClientWithOrderNoPaymentDto
            {
                Id = _client.Id,
                ClientName = _client.Name
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ClientWithoutSalesAgent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ClientWithoutSalesAgentDto>>> GetClientWithoutSalesAgent()
        {
            var results = await (from _office in _context.Offices
            join _employee in _context.Employees on _office.Id equals _employee.IdOfficeFk into eGroup
            from _employee in eGroup.DefaultIfEmpty() 
            join _positionEmployee in _context.PositionsEmployees on _employee.IdPositionFk equals _positionEmployee.Id into peGroup
            from _positionEmployee in peGroup.DefaultIfEmpty()
            join _client in _context.Clients on _employee.Id equals _client.IdEmployeeFk into cGroup
            from _client in cGroup.DefaultIfEmpty()
            join _order in _context.Orders on _client.Id equals _order.IdClientFk into oGroup
            from _order in oGroup.DefaultIfEmpty()
            join _orderDetail in _context.OrdersDetails on _order.Id equals _orderDetail.IdOrderFk into odGroup
            from _orderDetail in odGroup.DefaultIfEmpty()
            join _product in _context.Products on _orderDetail.IdProductFk equals _product.Id into pGroup
            from _product in pGroup.DefaultIfEmpty()
            join _rangeProduct in _context.RangersProducts on _product.IdRangerFk equals _rangeProduct.Id into rpGroup
            from _rangeProduct in rpGroup.DefaultIfEmpty()
            where _positionEmployee == null || _positionEmployee.Id != 3 
            && _rangeProduct.Id == "RNG001" 
            select new ClientWithoutSalesAgentDto
            {
                Id = _office.Id,
                OfficeName = _office.Name
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("SalesAgentWithoutClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<SalesAgentWithoutClientDto>>> GetSalesAgentWithoutClient()
        {
            var results = await (from e in _context.Employees
            join o in _context.Offices on e.IdOfficeFk equals o.Id
            join pe in _context.PositionsEmployees on e.IdPositionFk equals pe.Id
            join c in _context.Clients on e.Id equals c.IdEmployeeFk into clients
            from c in clients.DefaultIfEmpty()
            where !_context.Clients.Any(c => c.IdEmployeeFk == e.Id)
            select new SalesAgentWithoutClientDto
            {
                Name = e.Name,
                LastName = e.LastNameOne,
                LastNameTwo = e.LastNameTwo,
                Position = pe.Name,
                OfficePhone = o.Phone
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ProductWithoutOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProductoWithoutOrder>>> GetProductWithoutOrder()
        {
            var results = await (from _product in _context.Products
            join _orderDetail in _context.OrdersDetails on _product.Id equals _orderDetail.IdProductFk into _orderDetailGroup
            from _orderdetail in _orderDetailGroup.DefaultIfEmpty()
            where !_context.Products.Any(prod => prod.Id == _orderdetail.IdProductFk)
            select new ProductoWithoutOrder
            {
                IdProduct = _product.Id,
                ProductName = _product.Name
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ClientInPayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ClientInPaymentDto>>> GetClientInPayment()
        {
            var results = await (from _client in _context.Clients
                where _context.Payments.Any(p => p.IdClientFk == _client.Id)
                select new {_client.Id, _client.Name}).ToListAsync();
            return  Ok(results);
        }

        [HttpGet("ClientNotPayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ClientNotPaymentDto>>> GetClientNotPayment()
        {
            var results = await (from _client in _context.Clients
            join _payment in _context.Payments on _client.Id equals _payment.IdClientFk into _clientPayment
            from _payment in _clientPayment.DefaultIfEmpty()
            where !_context.Clients.Any(client => client.Id == _payment.IdClientFk)
            select new ClientWithoutPaymentDto
            {
                Id = _client.Id,
                ClientName = _client.Name
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("EmployeeNoClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<EmployeeNoClientDto>>> GetEmployeeNoClient()
        {
            var results = await (from temployee in _context.Employees
            join tpositionEmployee in _context.PositionsEmployees on temployee.IdPositionFk equals tpositionEmployee.Id
            join toffice in _context.Offices on temployee.IdOfficeFk equals toffice.Id
            join tclient in _context.Clients on temployee.Id equals tclient.IdEmployeeFk into Clients
            from tclient in Clients.DefaultIfEmpty()
            where tclient == null
            select new EmployeeNoClientDto
            {
                Name = temployee.Name,
                LastName = temployee.LastNameOne,
                Position = tpositionEmployee.Name,
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ProductWithLessStock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProductWithLessStockDto>>> GetProductWithLessStock()
        {
            var results = await (from _product in _context.Products
                where _product.Stock == (from p in _context.Products
                        select p.Stock).Min()
            select new ProductWithLessStockDto
            {
                Id = _product.Id,
                ProductName = _product.Name,
                MinStock = _product.Stock,
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ProductWithMaxPrice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProductWithMaxPriceDto>>> GetProductWithMaxPrice()
        {
            var results = await (from _product in _context.Products
                where _product.PriceSale == (from p in _context.Products
                        select p.PriceSale).Max()
            select new ProductWithMaxPriceDto
            {
                Id = _product.Id,
                ProductName = _product.Name,
                PriceSale = _product.PriceSale,
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ClientWithMaxCredit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ClientWithMaxCreditDto>>> GetClientWithMaxCredit()
        {
            var results = await (from _client in _context.Clients
                where _client.CreditLimit == (from c in _context.Clients
                        select c.CreditLimit).Max()
            select new ClientWithMaxCreditDto
            {
                Id = _client.Id,
                ClientName = _client.Name,
                CreditLimit = _client.CreditLimit
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("EmployeeByBoss{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<EmployeeByBossDto>>> GetEmployeeByBoss(string name)
        {
            var results = await (from _employee in _context.Employees
                join _boss in _context.Bosses on _employee.IdBoosFk equals _boss.Id
                where _boss.Name == name.Trim().ToLower()
            select new EmployeeByBossDto
            {
                Id = _employee.Id,
                EmployeeName = _employee.Name,
                EmployeeLastName = _employee.LastNameOne,
                EmployeeEmail = _employee.Email
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ProductMinStock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProductWithLessStockDto>>> GetProductMinStock()
        {
            var results = await (from _product in _context.Products
                where _product.Stock == (from p in _context.Products
                        select p.Stock).Min()
            select new ProductWithLessStockDto
            {
                Id = _product.Id,
                ProductName = _product.Name,
                MinStock = _product.Stock,
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ProductMaxStock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProductMaxStockDto>>> GetProductMaxStock()
        {
            var results = await (from _product in _context.Products
                where _product.Stock == (from p in _context.Products
                        select p.Stock).Max()
            select new ProductMaxStockDto
            {
                Id = _product.Id,
                ProductName = _product.Name,
                MaxStock = _product.Stock,
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ClientWithCreditMorePayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ClientWithCreditMorePaymentDto>>> GetClientWithCreditMorePayment()
        {
            var results = await (from _client in _context.Clients
                where _client.CreditLimit > (from p in _context.Payments
                            where p.IdClientFk == _client.Id
                            select p.Total).Sum()
            select new ClientWithCreditMorePaymentDto
            {
                Id = _client.Id,
                ClientName = _client.Name
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ProductWithMaxSales")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProductWithMaxSalesDto>>> GetProductWithMaxSales()
        {
            var results = await (from od in _context.OrdersDetails
            join p in _context.Products on od.IdProductFk equals p.Id
                where od.Quantity == (from s in (from sub in _context.OrdersDetails
                    group sub by sub.IdProductFk into g
                    select new {IdProductFk = g.Key, TotalQuantity = g.Sum(x => x.Quantity)})
                    select s.TotalQuantity).Max()
            select new ProductWithMaxSalesDto
            {
                Id = od.IdProductFk,
                ProductName = p.Name
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ProductHigherPrice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProductWithMaxPriceDto>>> GetProductHigherPrice()
        {
            var results = await (from _product in _context.Products
                where _product.PriceSale == (from p in _context.Products
                        select p.PriceSale).Max()
            select new ProductWithMaxPriceDto
            {
                Id = _product.Id,
                ProductName = _product.Name,
                PriceSale = _product.PriceSale,
            }).ToListAsync();
            return Ok(results);
        }

        [HttpGet("ClientHigherCredit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ClientWithMaxCreditDto>>> GetClientHigherCredit()
        {
            var results = await (from _client in _context.Clients
                where _client.CreditLimit == (from c in _context.Clients
                        select c.CreditLimit).Max()
            select new ClientWithMaxCreditDto
            {
                Id = _client.Id,
                ClientName = _client.Name,
                CreditLimit = _client.CreditLimit
            }).ToListAsync();
            return Ok(results);
        }
    }
}