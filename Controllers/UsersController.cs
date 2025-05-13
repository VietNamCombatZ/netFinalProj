using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using finalProject.Data;
using finalProject.Models;
using finalProject.Models.ViewModels;
using finalProject.Models.Enums;
using finalProject.Controllers.Filter;
using finalProject.Helpers;
using Microsoft.AspNetCore.Authorization;


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
    
    
    public async Task<IActionResult> Profile()
    {
        // Giả sử bạn lưu thông tin người dùng trong session hoặc cookie sau khi đăng nhập
        var userId = HttpContext.Session.GetInt32("UserId");; // Thay bằng cách lấy UserId của người dùng từ session hoặc claim
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // Chỉnh sửa thông tin cá nhân
    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var userId = HttpContext.Session.GetInt32("UserId");; // Thay bằng cách lấy UserId của người dùng từ session hoặc claim
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(User user)
    {
        if (ModelState.IsValid)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var existingUser = await _context.Users.FindAsync(userId.Value);

            if (existingUser == null)
            {
                return NotFound();
            }

            // Kiểm tra xem giá trị có thực sự thay đổi không
            if (existingUser.Name != user.Name)
            {
                existingUser.Name = user.Name;
            }

            if (existingUser.DateofBirth != user.DateofBirth)
            {
                existingUser.DateofBirth = user.DateofBirth;
            }

            try
            {
                // Cập nhật đối tượng
                _context.Update(existingUser);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi lưu vào cơ sở dữ liệu
                ModelState.AddModelError(string.Empty, "Có lỗi khi cập nhật thông tin.");
                return View(user);
            }

            return RedirectToAction(nameof(Profile));
        }

        return View(user);
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

            // Không cần dòng này nữa vì đã có [Compare] trong ViewModel
            // if (model.Password != model.ConfirmPassword)
            // {
            //     ModelState.AddModelError("Password", "Password và ConfirmPassword không khớp");
            //     return View(model);
            // }

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
            TempData["ErrorMessage"] = null;
            return RedirectToAction("Login");
        }

        return View(model);
    }
    
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

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
        
        
        //Lưu thông tin cần truy xuất nhiều vào cookie
        CookieOptions options = new CookieOptions
        {
            Expires = DateTimeOffset.Now.AddDays(7),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };

        Response.Cookies.Append("UserId", user.UserId.ToString(), options);
        Response.Cookies.Append("UserRole", user.Role.ToString(), options);


        // Điều hướng theo role
        if (user.Role == Role.Admin)
        {
            return RedirectToAction("AdminDashboard", "Users");
        }
        else if (user.Role == Role.Author)
        {
            return RedirectToAction("Dashboard", "Users");
        }

        // ✅ Trường hợp không xác định role
        return RedirectToAction("Login", "Account");
        // return RedirectToAction("Dashboard", "Users");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
    
    // [Authorize(Roles = "Admin")]
    [AuthorizeRole("Admin")]
    
    [HttpGet]
    public IActionResult AdminDashboard()
    {
        return View();
    }
    
    // [Authorize(Roles = "Author")]
    [AuthorizeRole("Author")]
    [AuthorizeLogin]
    [HttpGet]
    public IActionResult Dashboard()
    {
        return View();
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        // return View("~/Views/Home/AccessDenied.cshtml");
        return View();
    }
    
    
    
    

    [AuthorizeRole("Admin")]
    public IActionResult AdminList()
    {
        var admins = _context.Users
            .Where(u => u.Role == Role.Admin)
            .ToList();

        return View(admins);
    }
    
    // GET: Users/CreateAdmin
    [AuthorizeRole("Admin")]
    [HttpGet]
    public IActionResult CreateAdmin()
    {
        return View();
    }

    // POST: Users/CreateAdmin
    [HttpPost]
    [AuthorizeRole("Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAdmin(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            ModelState.AddModelError("", "Vui lòng nhập tên đăng nhập.");
            return View();
        }

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (existingUser == null)
        {
            ModelState.AddModelError("", "Không tìm thấy người dùng có username này.");
            return View();
        }

        if (existingUser.Role == Role.Admin)
        {
            ModelState.AddModelError("", "Người dùng này đã là Admin.");
            return View();
        }

        // Cập nhật role thành Admin
        existingUser.Role = Role.Admin;
        _context.Users.Update(existingUser);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã cấp quyền Admin thành công.";
        return RedirectToAction("AdminList");
    }

    [AuthorizeRole("Admin")]
    [HttpPost]
    
    public IActionResult DeleteAdmin(int id)
    {
        var admin = _context.Users.FirstOrDefault(u => u.UserId == id && u.Role == Role.Admin);

        var totalAdmins = _context.Users.Count(u => u.Role == Role.Admin);

        if (admin != null && totalAdmins <= 1)
        {
            TempData["ErrorMessage"] = "Không thể hủy quyền Admin vì hệ thống chỉ còn một tài khoản Admin duy nhất.";
            return RedirectToAction("AdminList");
        }

        if (admin != null)
        {
            // Lưu trạng thái trước khi thay đổi
            var currentUserId = HttpContext.Session.GetInt32("UserId");

            // Nếu người dùng đang xóa chính tài khoản của mình
            if (admin.UserId == currentUserId)
            {
                admin.Role = Role.Author;
                _context.Users.Update(admin);
                _context.SaveChanges();

                // Đăng xuất và chuyển hướng về Dashboard của Author
                HttpContext.Session.Clear(); // Xóa session của người dùng
                return RedirectToAction("Dashboard", "Users"); // Chuyển hướng đến Dashboard cho Author
            }

            // Nếu không phải là người dùng hiện tại, chỉ cần cập nhật role
            admin.Role = Role.Author;
            _context.Users.Update(admin);
            _context.SaveChanges();
        }

        return RedirectToAction("AdminList");
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