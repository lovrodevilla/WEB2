using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        private readonly UsersDbContext _context;

        public HomeController(ILogger<HomeController> logger, UsersDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> User()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public IActionResult CreateUser() {

            return View();
        }

        public async Task<IActionResult> CreateUserForm(CreateUser model)
        {
            User user = new User();
            user.Username = model.Username;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.Password = model.Password;

            if (string.IsNullOrEmpty(model.Username) ||
                string.IsNullOrEmpty(model.FirstName) ||
                string.IsNullOrEmpty(model.LastName) ||
                string.IsNullOrEmpty(model.Email) ||
                string.IsNullOrEmpty(model.Password))
            {
                ViewBag.Message = "FILL ALL FIELDS";
                return View("CreateUser", model);
            }
            if (model.RememberMe == false)
            {
                //checks for a minimum length of 8 characters, the presence of uppercase and lowercase letters,
                //at least one digit, and at least one special character
                if (model.Password.Length < 8 ||
                  !model.Password.Any(char.IsUpper) ||
                  !model.Password.Any(char.IsLower) ||
                  !model.Password.Any(char.IsDigit) ||
                  model.Password.Any(ch => !char.IsLetterOrDigit(ch)))
                {
                    ViewBag.Message = "PASSWORD IS NOT VALID";
                    return View("CreateUser", model);
                }
                else
                {
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> OneUser(string username, string password, bool rememberMe)
        {

            if (rememberMe)
            {
                string query = $"SELECT * FROM public.\"Users\" WHERE \"Username\" = '{username}' AND \"Password\" = '{password}'";
                var modelData = await _context.Users.FromSqlRaw(query).ToListAsync();
                return View(modelData);
            }
            else
            {
                var modelData = await _context.Users
                    .Where(x => x.Username == username && x.Password == password).ToListAsync();
                return View(modelData);
            }
        }

        public IActionResult FindUser() {

            return View();
        }

        public IActionResult FindUserForm(LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return View("FindUser");
            }

            return RedirectToAction("OneUser", new { username = model.Username, password = model.Password, rememberMe = model.RememberMe });
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
