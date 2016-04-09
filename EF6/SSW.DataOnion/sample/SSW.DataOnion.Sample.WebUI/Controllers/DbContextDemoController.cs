using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SSW.DataOnion.Core;
using SSW.DataOnion.Interfaces;
using SSW.DataOnion.Sample.Data;
using SSW.DataOnion.Sample.WebUI.Models;
using SSW.DataOnion.Sample.WebUI.Services.Query;

namespace SSW.DataOnion.Sample.WebUI.Controllers
{
    public class DbContextDemoController : Controller
    {
        private readonly IDbContextScopeFactory dbContextScopeFactory;
        private readonly ISchoolQueryService schoolQueryService;

        public DbContextDemoController(
            IDbContextScopeFactory dbContextScopeFactory,
            ISchoolQueryService schoolQueryService)
        {
            this.dbContextScopeFactory = dbContextScopeFactory;
            this.schoolQueryService = schoolQueryService;
        }

        public async Task<ActionResult> Index()
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

        public async Task<ActionResult> Students(Guid schoolId)
        {
            Guard.AgainstNullOrEmptyGuid(schoolId, nameof(schoolId));

            using (var dbContextScope = this.dbContextScopeFactory.CreateReadOnly())
            {
                var existingSchool =
                    await
                        dbContextScope.DbContexts.Get<SchoolDbContext>()
                            .Schools
                            .Include(s => s.Students)
                            .SingleOrDefaultAsync(s => s.Id == schoolId);
                if (existingSchool == null)
                {
                    throw new ApplicationException($"Could not find school with id '{schoolId}'");
                }

                return
                    this.View(new StudentsModel
                    {
                        School = existingSchool,
                        Students = existingSchool.Students.ToList()
                    });
            }
        }

        [HttpGet] 
        public async Task<ActionResult> EditStudent(Guid? studentId, Guid schoolId)
        {
            if (!studentId.HasValue || studentId == Guid.Empty)
            {
                return this.PartialView("StudentDetails", new StudentModel { SchoolId = schoolId });
            }

            using (var dbContextScope = this.dbContextScopeFactory.CreateReadOnly())
            {
                var existingStudent =
                    await
                        dbContextScope.DbContexts.Get<SchoolDbContext>()
                            .Students.SingleOrDefaultAsync(s => s.Id == studentId.Value);
                if (existingStudent == null)
                {
                    throw new ApplicationException($"Could not find student with id {studentId}");
                }

                var model = StudentModel.CreateFromEntity(existingStudent);
                model.SchoolId = schoolId;
                return this.PartialView("StudentDetails", model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmDelete(Guid studentId, Guid schoolId)
        {
            using (var dbContextScope = this.dbContextScopeFactory.CreateReadOnly())
            {
                var existingStudent =
                    await
                        dbContextScope.DbContexts.Get<SchoolDbContext>()
                            .Students.SingleOrDefaultAsync(s => s.Id == studentId);
                if (existingStudent == null)
                {
                    throw new ApplicationException($"Could not find student with id {studentId}");
                }

                var model = StudentModel.CreateFromEntity(existingStudent);
                model.SchoolId = schoolId;
                return this.PartialView("ConfirmDelete", model);
            }
        }

        [HttpPost] 
        public async Task<ActionResult> EditStudent(StudentModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.PartialView("StudentDetails", model);
            }

            using (var dbContextScope = this.dbContextScopeFactory.Create())
            {
                if (!model.Id.HasValue || model.Id.Value == Guid.Empty)
                {
                    var existingSchool =
                        await dbContextScope.DbContexts.Get<SchoolDbContext>()
                            .Schools.Include(s => s.Students).SingleOrDefaultAsync(s => s.Id == model.SchoolId);

                    if (existingSchool == null)
                    {
                        throw new ApplicationException($"Could not find school with id {model.SchoolId}");
                    }

                    var newStudent = model.ToStudentEntity();
                    dbContextScope.DbContexts.Get<SchoolDbContext>().Students.Add(newStudent);
                    existingSchool.EnrollStudent(newStudent);
                }
                else
                {
                    var existingStudent =
                        await
                            dbContextScope.DbContexts.Get<SchoolDbContext>()
                                .Students.SingleOrDefaultAsync(s => s.Id == model.Id);
                    if (existingStudent == null)
                    {
                        throw new ApplicationException($"Could not find student with id {model.Id}");
                    }

                    existingStudent.UpdateName(model.FirstName, model.LastName);
                }

                await dbContextScope.SaveChangesAsync();

                return this.Json(new { Result = "ok" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteStudent(StudentModel model)
        {
            using (var dbContextScope = this.dbContextScopeFactory.Create())
            {
                var existingSchool =
                        await dbContextScope.DbContexts.Get<SchoolDbContext>()
                            .Schools.Include(s => s.Students).SingleOrDefaultAsync(s => s.Id == model.SchoolId);

                if (existingSchool == null)
                {
                    throw new ApplicationException($"Could not find school with id {model.SchoolId}");
                }

                var existingStudent =
                    await
                        dbContextScope.DbContexts.Get<SchoolDbContext>()
                            .Students.SingleOrDefaultAsync(s => s.Id == model.Id);
                if (existingStudent == null)
                {
                    throw new ApplicationException($"Could not find student with id {model.Id}");
                }

                existingSchool.KickStudentOut(existingStudent);
                dbContextScope.DbContexts.Get<SchoolDbContext>().Students.Remove(existingStudent);

                await dbContextScope.SaveChangesAsync();

                return this.Json(new { Result = "ok" });
            }
        }
    }
}