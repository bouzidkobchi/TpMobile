using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MobileAppTp.Controllers
{
    [ApiController]
    [Route("Products")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> userManager;
        private readonly IConfiguration _configuration;

        public ProductsController(AppDbContext context, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _context = context;
            this.userManager = userManager;
            _configuration = configuration;
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

        //[Authorize]
        //[HttpPost("favorites")]
        //public async Task<IActionResult> AddProductTofavoritesAsync([FromQuery] string title)
        //{
        //    if(User.Identity != null)
        //    {
        //        var userIdentity = (ClaimsIdentity)User.Identity;
        //        var userId = userIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //        if(userId != null)
        //        {
        //            var user = await userManager.FindByIdAsync(userId);
        //            if(user != null)
        //            {
        //                var product = _context.Products.Find(title);
        //                if(product != null)
        //                {
        //                    user.Favorites.Add(product);
        //                    _context.SaveChanges();
        //                    return Ok(new
        //                    {
        //                        message = "product added to favorites!"
        //                    });
        //                }
        //                return BadRequest(new
        //                {
        //                    message = "product not found!"
        //                });
        //            }
        //            return BadRequest(new
        //            {
        //                message = "user not found!"
        //            });
        //        }
        //        return BadRequest(new
        //        {
        //            message = "userid in this jwt not found!"
        //        });
        //    }

        //    return Unauthorized(new
        //    {
        //        message = "unauthorized!"
        //    });
        //}

        //[Authorize]
        //[HttpPost("favorites")]
        //public async Task<IActionResult> AddProductToFavoritesAsync([FromQuery] string title)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return Unauthorized(new { message = "User ID not found in JWT token." });
        //    }

        //    var user = await userManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        return BadRequest(new { message = "User not found." });
        //    }

        //    var product = _context.Products.FirstOrDefault(p => p.Title == title);
        //    if (product == null)
        //    {
        //        return NotFound(new { message = "Product not found." });
        //    }

        //    user.Favorites.Add(product);
        //    _context.SaveChanges();

        //    return Ok(new { message = "Product added to favorites." });
        //}


        //[Authorize]
        //[HttpGet("favorites")]
        //public IActionResult GetFavorites()
        //{
        //    if (User.Identity != null)
        //    {
        //        var userIdentity = (ClaimsIdentity)User.Identity;
        //        var userId = userIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //        if (userId != null)
        //        {
        //            var user = _context.Users.Include(u => u.Favorites).FirstOrDefault(u => u.Id == userId);

        //            if (user != null)
        //            {
        //                return Ok(new
        //                {
        //                    user.Favorites
        //                });
        //            }
        //            return NotFound(new
        //            {
        //                message = "user with this token not found!"
        //            });
        //        }
        //        return NotFound(new
        //        {
        //            message = "userId not found in the token!"
        //        });
        //    }
        //    return Forbid("can not validate user identity!");
        //}

        //[Authorize]
        //[HttpGet("favorites")]
        //public IActionResult GetFavorites()
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return NotFound(new { message = "User ID not found in the token." });
        //    }

        //    var user = _context.Users.Include(u => u.Favorites).FirstOrDefault(u => u.Id == userId);
        //    if (user == null)
        //    {
        //        return NotFound(new { message = "User not found." });
        //    }

        //    return Ok(new { favorites = user.Favorites });
        //}

        //[Authorize]
        //[HttpDelete("favorites")]
        //public IActionResult DeleteProductFromfavorites([FromQuery] string title)
        //{
        //    if (User.Identity != null)
        //    {
        //        var userIdentity = (ClaimsIdentity)User.Identity;
        //        var userId = userIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //        {
        //            var user = _context.Users.Include(u => u.Favorites).FirstOrDefault(u => u.Id == userId);
        //            if (user != null)
        //            {
        //                var product = _context.Products.Find(title);
        //                if (product != null)
        //                {
        //                    user.Favorites.Remove(product);
        //                    _context.SaveChanges();
        //                    return Ok(new
        //                    {
        //                        message = "product deleted seccufuly from favorites!"
        //                    });
        //                }
        //                return BadRequest(new
        //                {
        //                    message = "product not found!"
        //                });
        //            }
        //            return BadRequest(new
        //            {
        //                message = "user not found!"
        //            });
        //        }
        //        return BadRequest(new
        //        {
        //            message = "userid in this jwt not found!"
        //        });
        //    }

        //    return Unauthorized(new
        //    {
        //        message = "unauthorized!"
        //    });
        //}

        //[Authorize]
        //[HttpDelete("favorites")]
        //public IActionResult DeleteProductFromFavorites([FromQuery] string title)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return Unauthorized(new { message = "User ID not found in the token." });
        //    }

        //    var user = _context.Users.Include(u => u.Favorites).FirstOrDefault(u => u.Id == userId);
        //    if (user == null)
        //    {
        //        return BadRequest(new { message = "User not found." });
        //    }

        //    var product = _context.Products.FirstOrDefault(p => p.Title == title);
        //    if (product == null)
        //    {
        //        return BadRequest(new { message = "Product not found." });
        //    }

        //    user.Favorites.Remove(product);
        //    _context.SaveChanges();

        //    return Ok(new { message = "Product deleted successfully from favorites." });
        //}

        // custom authentication using parameter instead of header , not good , but for testing purpose only
        [HttpGet("favorites")]
        public IActionResult GetFavorites([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Token is required in the query string." });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:key"])),
                ValidateIssuer = false,
                ValidIssuer = _configuration["JWT:audience"],
                ValidateAudience = false,
                ValidAudience = _configuration["JWT:audience"],
                ValidateLifetime = false
            };

            try
            {
                // Validate the token
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                // Retrieve user ID from the validated token
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User ID not found in the token." });
                }

                // Retrieve user from the database
                var user = _context.Users.Include(u => u.Favorites).FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return BadRequest(new { message = "User not found." });
                }

                return Ok(new { favorites = user.Favorites });
            }
            catch (SecurityTokenException)
            {
                return Unauthorized(new { message = "Invalid token." });
            }
        }

        [HttpDelete("favorites")]
        public IActionResult DeleteProductFromFavorites([FromQuery] string title, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Token is required in the query string." });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:key"])),
                ValidateIssuer = false,
                ValidIssuer = _configuration["JWT:audience"],
                ValidateAudience = false,
                ValidAudience = _configuration["JWT:audience"],
                ValidateLifetime = false
            };

            try
            {
                // Validate the token
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                // Retrieve user ID from the validated token
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User ID not found in the token." });
                }

                // Retrieve user from the database
                var user = _context.Users.Include(u => u.Favorites).FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return BadRequest(new { message = "User not found." });
                }

                // Retrieve product from the database
                var product = _context.Products.FirstOrDefault(p => p.Title == title);
                if (product == null)
                {
                    return BadRequest(new { message = "Product not found." });
                }

                // Remove product from favorites
                user.Favorites.Remove(product);
                _context.SaveChanges();

                return Ok(new { message = "Product deleted successfully from favorites." });
            }
            catch (SecurityTokenException)
            {
                return Unauthorized(new { message = "Invalid token." });
            }
        }

        [HttpPost("favorites")]
        public async Task<IActionResult> AddProductToFavoritesAsync([FromQuery] string title, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Token is required in the query string." });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:key"])),
                ValidateIssuer = false,
                ValidIssuer = _configuration["JWT:audience"],
                ValidateAudience = false,
                ValidAudience = _configuration["JWT:audience"],
                ValidateLifetime = false
            };

            try
            {
                // Validate the token
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                // Retrieve user ID from the validated token
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User ID not found in the token." });
                }

                // Retrieve user from the database
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return BadRequest(new { message = "User not found." });
                }

                // Retrieve product from the database
                var product = _context.Products.FirstOrDefault(p => p.Title == title);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found." });
                }

                // Add product to user's favorites
                user.Favorites.Add(product);
                _context.SaveChanges();

                return Ok(new { message = "Product added to favorites." });
            }
            catch (SecurityTokenException)
            {
                return Unauthorized(new { message = "Invalid token." });
            }
        }

    }
}