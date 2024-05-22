// Program.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MyContactFormApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

// Startup.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using MyContactFormApp.Data;

namespace MyContactFormApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MyConnectionString")));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Contact}/{action=Index}/{id?}");
            });
        }
    }
}

// Models/ContactForm.cs
namespace MyContactFormApp.Models
{
    public class ContactForm
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string Message { get; set; }
    }
}

// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using MyContactFormApp.Models;

namespace MyContactFormApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ContactForm> ContactForms { get; set; }
    }
}

// Controllers/ContactController.cs
using Microsoft.AspNetCore.Mvc;
using MyContactFormApp.Data;
using MyContactFormApp.Models;
using System.Threading.Tasks;

namespace MyContactFormApp.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ContactForm contactForm)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contactForm);
                await _context.SaveChangesAsync();
                return RedirectToAction("ThankYou");
            }
            return View(contactForm);
        }

        public IActionResult ThankYou()
        {
            return View();
        }
    }
}

// Views/Contact/Index.cshtml
@model MyContactFormApp.Models.ContactForm

@{
    ViewData["Title"] = "Contact Us";
}

<h2>Contact Us</h2>

<form asp-action="Index" method="post">
    <div>
        <label asp-for="FullName"></label>
        <input asp-for="FullName" />
    </div>
    <div>
        <label asp-for="Phone"></label>
        <input asp-for="Phone" />
    </div>
    <div>
        <label asp-for="Email"></label>
        <input asp-for="Email" />
    </div>
    <div>
        <label asp-for="Department"></label>
        <select asp-for="Department">
            <option value="Accounting">Accounting</option>
            <option value="Technical Support">Technical Support</option>
            <option value="Human Resources">Human Resources</option>
        </select>
    </div>
    <div>
        <label asp-for="Message"></label>
        <textarea asp-for="Message"></textarea>
    </div>
    <div>
        <button type="submit">Submit</button>
    </div>
</form>

// Views/Contact/ThankYou.cshtml
@{
    ViewData["Title"] = "Thank You";
}

<h2>Thank You</h2>

<p>Your message has been sent successfully!</p>

// appsettings.json
{
  "ConnectionStrings": {
    "MyConnectionString": "Server=(localdb)\\mssqllocaldb;Database=MyContactFormDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "AllowedHosts": "*"
}