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

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_context.OrderDetails);
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

        [HttpGet("QuantityProduct/ProductId={productId}")]
        public IActionResult GetQuantityProduct()
        {
            var quantityProduct = _context.OrderDetails.GroupBy(od => od.ProductId).Select(od => new { ProductId = od.Key, Quantity = od.Sum(od => od.Quantity) });
            if (quantityProduct == null)
            {
                return NotFound();
            }

            return Ok(quantityProduct);
        }

        [HttpGet("ProductId={productId}")]
        public IActionResult GetByProductId(int productId)
        {
            var orderDetails = _context.OrderDetails.Where(od => od.ProductId == productId);
            if (orderDetails == null)
            {
                return NotFound();
            }

            return Ok(orderDetails);
        }

        [HttpGet("TotalPrice/OrderId={orderId}")]
        public IActionResult GetTotalPrice(int orderId)
        {
            var totalPrice = _context.OrderDetails.Where(od => od.OrderId == orderId).Sum(od => od.Price * od.Quantity);
            if (totalPrice == 0)
            {
                return NotFound();
            }

            return Ok(totalPrice);
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
        public IActionResult CancelOrder()
        {
            var orderDetailInDb = _context.OrderDetails
                .Where(od => od.OrderId == int.Parse(HttpContext.Request.RouteValues["orderId"].ToString()))
                .FirstOrDefault();

            if (orderDetailInDb == null)
            {
                return NotFound();
            }

            var productQuantityInDb = _context.ProductQuantities.Where(pq => pq.ProductId == orderDetailInDb.ProductId && pq.Color == orderDetailInDb.Color).FirstOrDefault();
            if (productQuantityInDb == null)
            {
                return NotFound();
            }

            productQuantityInDb.Quantity += orderDetailInDb.Quantity;
            productQuantityInDb.Sold -= orderDetailInDb.Quantity;

            _context.SaveChanges();
            return Ok();
        }
    }
}
