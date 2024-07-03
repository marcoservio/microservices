using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Model;
using GeekShopping.IdentityServer.Model.Context;

using IdentityModel;

using Microsoft.AspNetCore.Identity;

using System.Security.Claims;

namespace GeekShopping.IdentityServer.Initializer;

public class DbInitializer : IDbInitializer
{
    private readonly UserManager<ApplicationUser> _user;
    private readonly RoleManager<IdentityRole> _role;

    public DbInitializer(UserManager<ApplicationUser> user, RoleManager<IdentityRole> role)
    {
        _user = user;
        _role = role;
    }

    public void Initialize()
    {
        if (_role.FindByNameAsync(IdentityConfiguration.Admin).Result != null)
            return;

        _role.CreateAsync(new IdentityRole(IdentityConfiguration.Admin)).GetAwaiter().GetResult();
        _role.CreateAsync(new IdentityRole(IdentityConfiguration.Client)).GetAwaiter().GetResult();

        ApplicationUser admin = new()
        {
            UserName = "marco-admin",
            Email = "marco-admin@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "55+ (31) 12345-6789",
            FirstName = "Marco",
            LastName = "Admin"
        };

        _user.CreateAsync(admin, "Marco@007").GetAwaiter().GetResult();
        _user.AddToRoleAsync(admin, IdentityConfiguration.Admin).GetAwaiter().GetResult();

       _ = _user.AddClaimsAsync(admin, new Claim[]
       {
            new(JwtClaimTypes.Name, $"{admin.FirstName} {admin.LastName}"),
            new(JwtClaimTypes.GivenName, admin.FirstName),
            new(JwtClaimTypes.FamilyName, admin.LastName),
            new(JwtClaimTypes.Role, IdentityConfiguration.Admin),
       }).Result;

        ApplicationUser client = new()
        {
            UserName = "marco-client",
            Email = "marco-client@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "55+ (31) 12345-6789",
            FirstName = "Marco",
            LastName = "Client"
        };

        _user.CreateAsync(client, "Marco@007").GetAwaiter().GetResult();
        _user.AddToRoleAsync(client, IdentityConfiguration.Client).GetAwaiter().GetResult();
        
        _ = _user.AddClaimsAsync(client, new Claim[]
        {
            new(JwtClaimTypes.Name, $"{client.FirstName} {client.LastName}"),
            new(JwtClaimTypes.GivenName, client.FirstName),
            new(JwtClaimTypes.FamilyName, client.LastName),
            new(JwtClaimTypes.Role, IdentityConfiguration.Client),
        }).Result;
    }
}
