using Microsoft.AspNetCore.Identity;

public class AppUser : IdentityUser<string>
{
    public List<Product> Favorites { get; set; } = new List<Product>();
}
