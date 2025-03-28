using dotnetstartermvc.Models;
using Microsoft.AspNetCore.Identity;

namespace dotnetstartermvc.Areas.Identity.Models.UserViewModels
{
    public class AddUserRoleModel
    {
        public AppUser user { get; set; }

        public string[] RoleNames { get; set; }

        public List<IdentityRoleClaim<string>> claimsInRole { get; set; }
        public List<IdentityUserClaim<string>> claimsInUserClaim { get; set; }
    }
}