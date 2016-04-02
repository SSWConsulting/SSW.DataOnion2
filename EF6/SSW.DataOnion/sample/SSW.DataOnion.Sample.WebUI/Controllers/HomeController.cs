using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SSW.DataOnion.Core;
using SSW.DataOnion.Interfaces;
using SSW.DataOnion.Sample.Data;
using SSW.DataOnion.Sample.Entities;
using SSW.DataOnion.Sample.WebUI.Models;
using SSW.DataOnion.Sample.WebUI.Services.Query;

namespace SSW.DataOnion.Sample.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IDbContextScopeFactory dbContextScopeFactory;
        private readonly ISchoolQueryService schoolQueryService;

        public HomeController(
            IUnitOfWorkFactory unitOfWorkFactory, 
            IDbContextScopeFactory dbContextScopeFactory, 
            ISchoolQueryService schoolQueryService)
        {
            Guard.AgainstNull(unitOfWorkFactory, nameof(unitOfWorkFactory));
            Guard.AgainstNull(dbContextScopeFactory, nameof(dbContextScopeFactory));

            this.unitOfWorkFactory = unitOfWorkFactory;
            this.dbContextScopeFactory = dbContextScopeFactory;
            this.schoolQueryService = schoolQueryService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<ActionResult> Schools()
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

                return this.View(new SchoolsModel { Schools = schools, Student = student });
            }
        }

        public async Task<ActionResult> Schools2()
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

                return this.View(new SchoolsModel { Schools = schools, Student = student });
            }
        }
    }
}