using Microsoft.AspNetCore.Identity;

namespace OrderService.Domain.Entities;

public class AppUser : IdentityUser
{
    //username mail, passwordhash zaten var

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
