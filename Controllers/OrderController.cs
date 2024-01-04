using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhoneShopManagementBackend.Models;
using System.Globalization;
using System.Security.Cryptography.Xml;

namespace PhoneShopManagementBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly TechShopContext _context;

        public OrderController(TechShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            // Order by OrderDate in descending order
            var orders = _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpGet("Processing")]
        public IActionResult GetProcessing()
        {
            var ordersProcessingCount = _context.Orders.Count(o => o.Status == "processing");
            return Ok(ordersProcessingCount);
        }

        [HttpGet("TodayCompleted")]
        public IActionResult GetTodayCompleted()
        {
            var ordersTodayCompletedCount = _context.Orders.Count(o =>
                o.Status == "Done" && o.CompletedDate != null && o.CompletedDate == DateOnly.FromDateTime(DateTime.Today)
            );
            return Ok(ordersTodayCompletedCount);
        }

        [HttpGet("RevenueToday")]

        public IActionResult GetRevenueToday()
        {
            var ordersToday = _context.Orders.Where(o => o.CompletedDate != null && o.CompletedDate == DateOnly.FromDateTime(DateTime.Today)
            );
            double revenueToday = 0;
            foreach (var order in ordersToday)
            {
                revenueToday += order.TotalPrice;
            }

            return Ok(revenueToday);
        }

        [HttpGet("RevenueThisMonth")]
        public IActionResult GetRevenueThisMonth()
        {
            var ordersThisMonth = _context.Orders.Where(o => o.CompletedDate.Value.Month == DateTime.Today.Month);
            double revenueThisMonth = 0;
            foreach (var order in ordersThisMonth)
            {
                revenueThisMonth += order.TotalPrice;
            }

            return Ok(revenueThisMonth);
        }

        [HttpGet("RevenueEachDayThisMonth")]
        public IActionResult GetRevenueEachDayThisMonth()
        {
            var ordersThisMonth = _context.Orders
                .Where(o => o.CompletedDate.Value.Month == DateTime.Today.Month)
                .ToList(); // ToList để đảm bảo dữ liệu được truy xuất từ database trước khi thực hiện LINQ

            double[] revenueEachDayThisMonth =
                new double[DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)];

            foreach (var order in ordersThisMonth)
            {
                if (order.CompletedDate != null)
                {
                    revenueEachDayThisMonth[order.CompletedDate.Value.Day - 1] += order.TotalPrice;
                }

            }

            var result = revenueEachDayThisMonth
                .Select((revenue, day) => new { day = day + 1, revenue })
                .ToArray();

            return Ok(result);
        }

        [HttpGet("GetLastId")]
        public IActionResult GetLastId()
        {
            var lastId = _context.Orders
                .OrderByDescending(o => o.Id)
                .Select(o => o.Id)
                .FirstOrDefault();
            return Ok(lastId);
        }

        [HttpGet("GetMonthlyRevenue")]
        public IActionResult GetMonthlyRevenue([FromQuery] int startMonth, [FromQuery] int startYear,
            [FromQuery] int endMonth, [FromQuery] int endYear)
        {
            DateOnly startDate = new DateOnly(startYear, startMonth, 1);
            DateOnly endDate = new DateOnly(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));

            // Apply OrderBy to sort by CompletedDate in ascending order
            var result = _context.Orders
                .Where(o => o.CompletedDate >= startDate && o.CompletedDate <= endDate)
                .OrderBy(o => o.CompletedDate) // Add this line for sorting
                .GroupBy(o => new { Date = o.CompletedDate })
                .AsEnumerable()
                .Select(g => new
                {
                    Date = $"{g.Key.Date}",
                    Revenue = g.Sum(o => o.TotalPrice)
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("GetMonthlyRevenueByProduct")]
        public IActionResult GetMonthlyRevenueByProduct([FromQuery] int startMonth, [FromQuery] int startYear,
            [FromQuery] int endMonth, [FromQuery] int endYear, [FromQuery] int? productId)
        {
            DateOnly startDate = new DateOnly(startYear, startMonth, 1);
            DateOnly endDate = new DateOnly(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));

            // Apply OrderBy to sort by CompletedDate in ascending order
            var result = _context.OrderDetails
                .Join(
                                       _context.Orders,
                                                          orderDetail => orderDetail.OrderId,
                                                          order => order.Id,
                                                          (orderDetail, order) => new { OrderDetail = orderDetail, Order = order }
                                                      )
                .Where(o => o.Order.CompletedDate >= startDate && o.Order.CompletedDate <= endDate &&
                                                       (!productId.HasValue || o.OrderDetail.ProductId == productId)) // Added productId filter
                .OrderBy(o => o.Order.CompletedDate) // Add this line for sorting
                .GroupBy(o => new { Date = o.Order.CompletedDate })
                .AsEnumerable()
                .Select(g => new
                {
                    Date = $"{g.Key.Date}",
                    Revenue = g.Sum(o => o.OrderDetail.Quantity * o.OrderDetail.Price)
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("GetMonthlyProductsSold")]
        public IActionResult GetMonthlyProductsSold(
            [FromQuery] int startMonth,
            [FromQuery] int startYear,
            [FromQuery] int endMonth,
            [FromQuery] int endYear,
            [FromQuery] int? productId) // Added productId as a nullable parameter
        {
            DateOnly startDate = new DateOnly(startYear, startMonth, 1);
            DateOnly endDate = new DateOnly(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));

            var result = _context.OrderDetails
                .Join(
                    _context.Orders,
                    orderDetail => orderDetail.OrderId,
                    order => order.Id,
                    (orderDetail, order) => new { OrderDetail = orderDetail, Order = order }
                )
                .Where(o => o.Order.CompletedDate >= startDate && o.Order.CompletedDate <= endDate &&
                            (!productId.HasValue || o.OrderDetail.ProductId == productId)) // Added productId filter
                .GroupBy(o => new { Date = o.Order.CompletedDate })
                .OrderBy(g => g.Key.Date)
                .Select(g => new
                {
                    Date = $"{g.Key.Date:yyyy-MM-dd}",
                    ProductsSold = g.Sum(o => o.OrderDetail.Quantity)
                })
                .ToList();

            return Ok(result);
        }


        [HttpPost]
        public IActionResult AddOrder(Order order)
        {
            try
            {
                order.OrderDate = DateOnly.FromDateTime(DateTime.Today);
                order.CanceledDate = null; // Set to null as it's nullable
                order.CompletedDate = null; // Set to null as it's nullable

                if (order.DiscountId != null)
                {
                    var discount = _context.Discounts.Find(order.DiscountId);
                    discount.Quantity--;
                }

                _context.Orders.Add(order);
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex);
                return StatusCode(500, ex);
            }
        }

        [HttpPut("ChangeStatus/{id}")]
        public IActionResult ChangeStatus(int id, Order order)
        {
            var orderInDb = _context.Orders.Find(id);
            if (orderInDb == null)
            {
                return NotFound();
            }

            orderInDb.Status = order.Status;

            if (order.Status == "Done")
            {
                orderInDb.CompletedDate = DateOnly.FromDateTime(DateTime.Today);

            }
            else if (order.Status == "Cancelled")
            {
                orderInDb.CanceledDate = DateOnly.FromDateTime(DateTime.Today);
            }

            _context.Entry(orderInDb).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }
    }
}