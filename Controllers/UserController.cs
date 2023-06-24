using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Monitoring.Presistence.Contexts;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.ComponentModel.DataAnnotations;

namespace MonitoringNetCore.Controllers;
public class NewUser
{


    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }
}
public class UserController : Controller
{
    private readonly DataBaseContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    public UserController(DataBaseContext context,UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    // GET
    public IActionResult Index()
    {
        ViewBag.title = "کاربران";
        ViewBag.menuNavigation = "پیشخوان";
        ViewBag.menuItem = "کاربران";
        ViewBag.activeNavigation = "navigationDashboard";
        ViewBag.activeItem = "itemUsers";
        return View(_userManager.Users.ToList());
    }
    // GET: Video/Create
    public IActionResult Create()
    {
        ViewBag.title = "ایجاد کاربر جدید";
        ViewBag.menuNavigation = "پیشخوان";
        ViewBag.menuItem = "ایجاد کاربر جدید";
        ViewBag.activeNavigation = "navigationDashboard";
        return View();
    }

    // POST: Video/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Email,Password")] NewUser  newUser)
    {
        ViewBag.title = "ایجاد کاربر جدید";
        ViewBag.menuNavigation = "پیشخوان";
        ViewBag.menuItem = "ایجاد کاربر جدید";
        ViewBag.activeNavigation = "navigationDashboard";
    
        if (ModelState.IsValid)
        {
            var user = new IdentityUser(newUser.Email);
            await _userManager.SetUserNameAsync(user, newUser.Email);
            await _userManager.SetEmailAsync(user, newUser.Email);
            await _userManager.CreateAsync(user, newUser.Password);
            
            return RedirectToAction(nameof(Index));
        }
        return View();
    }
    // GET: Video/Delete/5
    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = _userManager.Users.Where(user => user.Id.Equals(id)).First();
        IdentityResult result = await  _userManager.DeleteAsync(user);
        if (! result.Succeeded)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}