using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhoneShopManagementBackend.Models;

namespace PhoneShopManagementBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderDetailController : ControllerBase
    {
        private readonly TechShopContext _context;

        public OrderDetailController(TechShopContext context)
        {
            _context = context;
        }

        [HttpGet("OrderId={orderId}")]
        public IActionResult Get(int orderId)
        {
            var orderDetails = _context.OrderDetails.Where(od => od.OrderId == orderId);
            if (orderDetails == null)
            {
                return NotFound();
            }

            return Ok(orderDetails);
        }

        [HttpGet("GetLastId")]
        public IActionResult GetLastId()
        {
            var lastOrderDetail = _context.OrderDetails.Find(_context.OrderDetails.Count());
            if (lastOrderDetail == null)
            {
                return NotFound();
            }

            return Ok(lastOrderDetail);
        }

        [HttpPost]
        public IActionResult Post(OrderDetail orderDetail)
        {
            var orderId = orderDetail.OrderId;
            var productId = orderDetail.ProductId;
            var color = orderDetail.Color;
            var quantity = orderDetail.Quantity;
            var price = orderDetail.Price;

            OrderDetail newOrderDetail = new OrderDetail
            {
                OrderId = orderId,
                ProductId = productId,
                Color = color,
                Quantity = quantity,
                Price = price
            };

            var productQuantityInDb = _context.ProductQuantities.Where(pq => pq.ProductId == productId && pq.Color == color).FirstOrDefault();
            if (productQuantityInDb == null)
            {
                return NotFound();
            }

            productQuantityInDb.Quantity -= quantity;
            productQuantityInDb.Sold += quantity;

            _context.OrderDetails.Add(newOrderDetail);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPut("CancelOrder/{orderId}")]
        public IActionResult CancelOrder(int orderId)
        {
            try
            {
                // Get order details
                var orderDetails = _context.OrderDetails
                    .Where(od => od.OrderId == orderId)
                    .ToList();

                if (orderDetails == null || !orderDetails.Any())
                {
                    return NotFound(new { status = 404, message = "Order details not found" });
                }

                // Process each order detail
                foreach (var orderDetail in orderDetails)
                {
                    var productId = orderDetail.ProductId;
                    var color = orderDetail.Color;
                    var quantity = orderDetail.Quantity;

                    // Get product quantity in DB
                    var productQuantityInDb = _context.ProductQuantities
                        .FirstOrDefault(pq => pq.ProductId == productId && pq.Color == color);

                    if (productQuantityInDb != null)
                    {
                        // Update product quantity
                        productQuantityInDb.Quantity += quantity;
                        productQuantityInDb.Sold -= quantity;

                        // Save changes
                        _context.ProductQuantities.Update(productQuantityInDb);
                    }
                }

                _context.SaveChanges();

                return Ok(new { status = 200, message = "Order canceled successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex);
                return StatusCode(500, new { status = 500, message = "Internal server error" });
            }
        }

    }
}
