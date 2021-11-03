using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.OAuth;
using BookStore.Models.Auth;
using System.Security.Claims;

namespace BookStore.Models.Auth
{
    public class BookStoreAuthorizationServerProvider:OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            ValidationRepository _repo = new ValidationRepository();
            
                var user = _repo.ValidateUser(context.UserName, context.Password);
                var admin = _repo.ValidateAdmin(context.UserName, context.Password);
                if(user == null && admin == null)
                {
                    context.SetError("invalid_grant", "provided username and password is incorrect");
                    return;
                }
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            if (user != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
            }
            else
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
                identity.AddClaim(new Claim(ClaimTypes.Name, admin.Username));
            }         
                context.Validated(identity);
            
            
        }
    }
}