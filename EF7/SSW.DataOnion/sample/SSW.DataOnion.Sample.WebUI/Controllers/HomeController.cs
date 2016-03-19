using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using SSW.DataOnion.Interfaces;
using SSW.DataOnion.Sample.Data;
using SSW.DataOnion.Sample.Entities;
using SSW.DataOnion.Sample.Entities.Exceptions;
using SSW.DataOnion.Sample.WebUI.Model;
using SSW.DataOnion.Sample.WebUI.Services.Query;

namespace SSW.DataOnion.Sample.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IDbContextScopeFactory dbContextScopeFactory;
        private readonly ISchoolQueryService schoolQueryService;

        public HomeController(IUnitOfWorkFactory unitOfWorkFactory, IDbContextScopeFactory dbContextScopeFactory, ISchoolQueryService schoolQueryService)
        {
            Guard.AgainstNull(unitOfWorkFactory, nameof(unitOfWorkFactory));
            Guard.AgainstNull(dbContextScopeFactory, nameof(dbContextScopeFactory));

            this.unitOfWorkFactory = unitOfWorkFactory;
            this.dbContextScopeFactory = dbContextScopeFactory;
            this.schoolQueryService = schoolQueryService;
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
            using (var unitOfWork = this.unitOfWorkFactory.CreateReadOnly())
            {
                var schools =
                    await unitOfWork.Repository<School>()
                        .Get()
                        .Include(s => s.Address)
                        .Include(s => s.Students)
                        .ToListAsync();

                var student =
                    this.schoolQueryService.GetStudentDetails(
                        schools?.FirstOrDefault()?.Students?.FirstOrDefault()?.Id ?? Guid.NewGuid());
                
                return View(new SchoolsModel { Schools = schools, Student = student });
            }
        }

        public async Task<IActionResult> Schools2()
        {
            using (var dbContextScope = this.dbContextScopeFactory.CreateReadOnly())
            {
                var schools =
                    await dbContextScope.DbContexts.Get<SchoolDbContext>()
                        .Schools
                        .Include(s => s.Address)
                        .Include(s => s.Students)
                        .ToListAsync();

                var student =
                    this.schoolQueryService.GetStudentDetails2(
                        schools?.FirstOrDefault()?.Students?.FirstOrDefault()?.Id ?? Guid.NewGuid());

                return View(new SchoolsModel { Schools = schools, Student = student });
            }
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
