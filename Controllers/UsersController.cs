using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using finalProject.Data;
using finalProject.Models;
using finalProject.Models.ViewModels;
using finalProject.Models.Enums;


namespace finalProject.Controllers;

public class UsersController : Controller
{
    private readonly FinalProjectDbContext _context;

    public UsersController(FinalProjectDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _context.Users.ToListAsync();
        return View(users);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var existingUserByUsername = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);
            if (existingUserByUsername != null)
            {
                ModelState.AddModelError("Username", "Tài khoản đã tồn tại.");
                return View(model);
            }

            var existingUserByEmail = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("Password", "Password và ConfirmPassword không khớp");
                return View(model);
            }

            var user = new User
            {
                Name = model.Name,
                DateofBirth = model.DateofBirth,
                Email = model.Email,
                Username = model.Username,
                Password = model.Password,
                Role = Role.Author // mặc định là Author
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        return View(model);
    }
    
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

        if (user == null)
        {
            ViewBag.LoginError = "Tài khoản hoặc mật khẩu không đúng.";
            return View();
        }

        // Lưu thông tin người dùng vào session
        HttpContext.Session.SetInt32("UserId", user.UserId);
        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetString("Role", user.Role.ToString());

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    
    
    // [HttpPost]
    // public async Task<IActionResult> Create(User user)
    // {
    //     if (ModelState.IsValid)
    //     {
    //         _context.Add(user);
    //         await _context.SaveChangesAsync();
    //         return RedirectToAction(nameof(Index));
    //     }
    //
    //     return View();
    //
    // }

    
            
}