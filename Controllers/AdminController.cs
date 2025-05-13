using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using finalProject.Data;
using finalProject.Models;
using finalProject.Models.ViewModels;
using finalProject.Models.Enums;


namespace finalProject.Controllers;

public class AdminController : Controller
{
    private readonly FinalProjectDbContext _context;

    public AdminController(FinalProjectDbContext context)
    {
        _context = context;
    }

    public IActionResult Dashboard()
    {
        var role = HttpContext.Session.GetString("Role");
        if (role != "Admin")
            return RedirectToAction("AccessDenied", "Account");

        return View();
    }
}
