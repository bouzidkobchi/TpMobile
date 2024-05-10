using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MobileAppTp.Controllers
{
    [ApiController]
    [Route("Products")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> userManager;

        public ProductsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Products.ToList();
            return Ok(new
            {
                products,
                products.Count
            });
        }

        [HttpPost]
        public IActionResult CreateProduct(Product product)
        {
            if (product == null)
            {
                return BadRequest(new { message = "Product cannot be null" });
            }

            if (_context.Products.Any(p => p.Title == product.Title))
            {
                return Conflict(new { message = "Product with this title already exists" });
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            return Ok(new { message = "Product created successfully" });
        }

        [HttpGet("{title}")]
        public IActionResult GetProductByTitle(string title)
        {
            var product = _context.Products.FirstOrDefault(p => p.Title == title);
            if (product != null)
            {
                return Ok(new { product });
            }
            return NotFound(new { message = $"There are no products with a title equal to {title}" });
        }

        [HttpPut("{title}")]
        public IActionResult UpdateProduct(string title, Product updatedProduct)
        {
            var existingProduct = _context.Products.FirstOrDefault(p => p.Title == title);
            if (existingProduct == null)
            {
                return NotFound(new { message = $"Product with title {title} not found" });
            }

            existingProduct.Title = updatedProduct.Title;
            existingProduct.Description = updatedProduct.Description;

            _context.SaveChanges();

            return Ok(new { message = "Product updated successfully" });
        }

        [HttpDelete]
        public IActionResult Delete(string title)
        {
            var product = _context.Products.FirstOrDefault(x => x.Title == title);
            if (product == null)
            {
                return NotFound(new
                {
                    message = "product not found!"
                });
            }
            _context.Remove(product);
            _context.SaveChanges(true);
            return Ok(new
            {
                message = "product deleted seccufuly!"
            });
        }

        [Authorize]
        [HttpPost("favorites")]
        public async Task<IActionResult> AddProductTofavoritesAsync([FromQuery] string title)
        {
            if(User.Identity != null)
            {
                var userIdentity = (ClaimsIdentity)User.Identity;
                var userId = userIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if(userId != null)
                {
                    var user = await userManager.FindByIdAsync(userId);
                    if(user != null)
                    {
                        var product = _context.Products.Find(title);
                        if(product != null)
                        {
                            user.Favorites.Add(product);
                            _context.SaveChanges();
                            return Ok(new
                            {
                                message = "product added to favorites!"
                            });
                        }
                        return BadRequest(new
                        {
                            message = "product not found!"
                        });
                    }
                    return BadRequest(new
                    {
                        message = "user not found!"
                    });
                }
                return BadRequest(new
                {
                    message = "userid in this jwt not found!"
                });
            }

            return Unauthorized(new
            {
                message = "unauthorized!"
            });
        }

        [Authorize]
        [HttpGet("favorites")]
        public IActionResult GetFavorites()
        {
            if (User.Identity != null)
            {
                var userIdentity = (ClaimsIdentity)User.Identity;
                var userId = userIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId != null)
                {
                    var user = _context.Users.Include(u => u.Favorites).FirstOrDefault(u => u.Id == userId);

                    if (user != null)
                    {
                        return Ok(new
                        {
                            user.Favorites
                        });
                    }
                    return NotFound(new
                    {
                        message = "user with this token not found!"
                    });
                }
                return NotFound(new
                {
                    message = "userId not found in the token!"
                });
            }
            return Forbid("can not validate user identity!");
        }

        [Authorize]
        [HttpDelete("favorites")]
        public IActionResult DeleteProductFromfavorites([FromQuery] string title)
        {
            if (User.Identity != null)
            {
                var userIdentity = (ClaimsIdentity)User.Identity;
                var userId = userIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                {
                    var user = _context.Users.Include(u => u.Favorites).FirstOrDefault(u => u.Id == userId);
                    if (user != null)
                    {
                        var product = _context.Products.Find(title);
                        if (product != null)
                        {
                            user.Favorites.Remove(product);
                            _context.SaveChanges();
                            return Ok(new
                            {
                                message = "product deleted seccufuly from favorites!"
                            });
                        }
                        return BadRequest(new
                        {
                            message = "product not found!"
                        });
                    }
                    return BadRequest(new
                    {
                        message = "user not found!"
                    });
                }
                return BadRequest(new
                {
                    message = "userid in this jwt not found!"
                });
            }

            return Unauthorized(new
            {
                message = "unauthorized!"
            });
        }

    }
}