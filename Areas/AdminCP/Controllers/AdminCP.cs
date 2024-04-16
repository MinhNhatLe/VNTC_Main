using dotnetstartermvc.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnetstartermvc.Areas.AdminCP.Controllers
{
    [Area("AdminCP")]
    [Authorize(Roles = $"{RoleName.SuperAdmin},{RoleName.Administrator},{RoleName.Manager},{RoleName.Member}")]
    public class AdminCP : Controller
    {
        [Route("/admincp/")]
        public IActionResult Index() => View();
    }
}