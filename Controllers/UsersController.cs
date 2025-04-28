using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using finalProject.Data;
using finalProject.Models;


namespace finalProject.Controllers;

public class UsersController : Controller
{
    private readonly FinalProjectDbContext _context;

    public UsersController(FinalProjectDbContext context)
    {
        _context = context;
    }

    
    
}