using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    // GET: /Account/Register
    public IActionResult Register()
    {
        return View();
    }

    // GET: /Account/Login
    public IActionResult Login()
    {
        return View();
    }
}
