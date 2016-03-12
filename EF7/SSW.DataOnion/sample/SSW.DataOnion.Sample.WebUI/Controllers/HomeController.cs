using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using SSW.DataOnion.Interfaces;
using SSW.DataOnion.Sample.Data;
using SSW.DataOnion.Sample.Entities;

namespace SSW.DataOnion.Sample.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly Func<IReadOnlyUnitOfWork> unitOfWorkFactory;
        private readonly Func<IDbContextReadOnlyScope> dbContextScopeFactory;

        public HomeController(Func<IReadOnlyUnitOfWork> unitOfWorkFactory, Func<IDbContextReadOnlyScope> dbContextScopeFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.dbContextScopeFactory = dbContextScopeFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public async Task<IActionResult> Schools()
        {
            using (var unitOfWork = this.unitOfWorkFactory())
            {
                var schools =
                    await unitOfWork.Repository<School>()
                        .Get()
                        .Include(s => s.Address)
                        .Include(s => s.Students)
                        .ToListAsync();

                return View(schools);
            }
        }

        public async Task<IActionResult> Schools2()
        {
            using (var dbContextScope = this.dbContextScopeFactory())
            {
                var schools =
                    await dbContextScope.DbContexts.Get<SchoolDbContext>()
                        .Schools
                        .Include(s => s.Address)
                        .Include(s => s.Students)
                        .ToListAsync();

                return View(schools);
            }
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
