// Controllers/ArticlesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using finalProject.Data;
using finalProject.Helpers;
using finalProject.Models;
using finalProject.Models.ViewModels;
using finalProject.Models.Enums;

namespace finalProject.Controllers;
public class ArticlesController : Controller
{
    private readonly FinalProjectDbContext _context;

    public ArticlesController(FinalProjectDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Create()
    {
        var viewModel = new CreateArticleViewModel
        {
            Topics = _context.Topics.ToList()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreateArticleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Topics = _context.Topics.ToList();
            return View(model);
        }

        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            TempData["ErrorMessage"] = "Bạn cần đăng nhập để thực hiện chức năng này.";
            return RedirectToAction("Login", "Users");
        }

        // Kiểm tra nếu tiêu đề bài viết đã tồn tại
        var existingArticle = _context.Articles
            .FirstOrDefault(a => a.Title.ToLower() == model.Title.ToLower());

        if (existingArticle != null)
        {
            TempData["ErrorMessage"] = "Tiêu đề bài viết đã tồn tại. Vui lòng chọn tiêu đề khác.";
            model.Topics = _context.Topics.ToList();  // Lấy lại danh sách chủ đề
            return View(model);  // Trả về view và hiển thị thông báo lỗi
        }

        var article = new Article
        {
            SubmissionDate = DateTime.Now,
            Title = model.Title,
            Summary = model.Summary,
            Content = model.Content,
            TopicId = model.TopicId,
            Status = ArticleStatus.Pending,
            AuthorId = userId.Value  // Lấy ID người dùng từ session
        };

        _context.Articles.Add(article);
        _context.SaveChanges();

        return RedirectToAction("MyArticles", "Articles");
    }

    private int GetLoggedInUserId()
    {
        // Ví dụ nếu lưu UserId trong session hoặc claim
        return int.Parse(User.FindFirst("UserId")?.Value ?? "0");
    }
    
    public IActionResult MyArticles()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Users");
        }

        var articles = _context.Articles
            .Where(a => a.AuthorId == userId.Value)
            .ToList();

        return View(articles);
    }
    
    public IActionResult Details(int id)
    {
        var article = _context.Articles
            .Include(a => a.Topic)
            .Include(a => a.Author)
            .FirstOrDefault(a => a.ArticleId == id);

        if (article == null)
            return NotFound();

        return View(article);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cancel(int id)
    {
        var article = _context.Articles.FirstOrDefault(a => a.ArticleId == id);
        if (article == null || article.Status != ArticleStatus.Pending)
            return NotFound();

        article.Status = ArticleStatus.Rejected;
        article.ReviewedAt = DateTime.Now; // Bạn cần có cột này trong Article

        _context.SaveChanges();

        return RedirectToAction("MyArticles"); // Trang danh sách bài viết của người dùng
    }
    
    
    [AuthorizeRole("Admin")]
    public IActionResult PendingArticles()
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
            return RedirectToAction("AccessDenied", "Users");

        var pending = _context.Articles
            .Include(a => a.Author)
            .Where(a => a.Status == ArticleStatus.Pending)
            .ToList();

        return View(pending);
    }
    
    
    [AuthorizeRole("Admin")]
    public async Task<IActionResult> DetailsForAdmin(int id)
    {
        var article = await _context.Articles
            .Include(a => a.Author) // Đảm bảo thông tin về tác giả được lấy
            .Include(a => a.Topic) // Nếu bạn muốn hiển thị thông tin chủ đề bài viết
            .FirstOrDefaultAsync(a => a.ArticleId == id);

        if (article == null)
        {
            return NotFound();
        }

        return View(article);
    }

    [HttpPost]
    // [Authorize(Roles = "Admin")]
    [AuthorizeRole("Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveOrReject(int id, string action)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article == null)
        {
            return NotFound();
        }

        if (action == "approve")
        {
            article.Status = ArticleStatus.Approved;
        }
        else if (action == "reject")
        {
            article.Status = ArticleStatus.Rejected;
        }

        article.ReviewedAt = DateTime.Now; // Lưu thời gian duyệt hoặc từ chối
        _context.Update(article);
        await _context.SaveChangesAsync();

        return RedirectToAction("PendingArticles"); // Quay lại danh sách bài viết chờ duyệt
    }
    
    
    
    // public IActionResult Statistics()
    // {
    //     var stats = new
    //     {
    //         Total = _context.Articles.Count(),
    //         Pending = _context.Articles.Count(a => a.Status == ArticleStatus.Pending),
    //         Approved = _context.Articles.Count(a => a.Status == ArticleStatus.Approved),
    //         Rejected = _context.Articles.Count(a => a.Status == ArticleStatus.Rejected)
    //     };
    //
    //     ViewBag.Stats = stats;
    //     return View();
    // }
    
    [AuthorizeRole("Admin")]
    public IActionResult Statistics()
    {
        // Thống kê số lượng bài viết theo từng tác giả
        var byAuthor = _context.Articles
            .GroupBy(a => a.AuthorId)
            .Select(g => new AuthorStats
            {
                AuthorId = g.Key,
                Username = g.FirstOrDefault().Author.Username,  // Lấy Username từ tác giả
                Count = g.Count()
            })
            .ToList();

        // Thống kê số lượng bài viết theo từng chủ đề
        var byTopic = _context.Articles
            .GroupBy(a => a.Topic.TopicName)
            .Select(g => new TopicStats
            {
                Topic = g.Key,
                Count = g.Count()
            })
            .ToList();

        // Thống kê số lượng bài đã duyệt / chưa duyệt
        var approvedCount = _context.Articles.Count(a => a.Status == ArticleStatus.Approved);
        var pendingCount = _context.Articles.Count(a => a.Status == ArticleStatus.Pending);

        // Gán vào ViewBag để truyền dữ liệu tới View
        ViewBag.ByAuthor = byAuthor;
        ViewBag.ByTopic = byTopic;
        ViewBag.ApprovedCount = approvedCount;
        ViewBag.PendingCount = pendingCount;

        return View();
    }


}