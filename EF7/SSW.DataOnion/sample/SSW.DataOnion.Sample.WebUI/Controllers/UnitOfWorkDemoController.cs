using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSW.DataOnion.Interfaces;
using SSW.DataOnion.Sample.Entities;
using SSW.DataOnion.Sample.Entities.Exceptions;
using SSW.DataOnion.Sample.WebUI.Model;
using SSW.DataOnion.Sample.WebUI.Services.Query;

namespace SSW.DataOnion.Sample.WebUI.Controllers
{
    public class UnitOfWorkDemoController : Controller
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly ISchoolQueryService schoolQueryService;

        public UnitOfWorkDemoController(IUnitOfWorkFactory unitOfWorkFactory, ISchoolQueryService schoolQueryService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.schoolQueryService = schoolQueryService;
        }

        public async Task<ActionResult> Index()
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

                return this.View(new SchoolsModel {Schools = schools, Student = student});
            }
        }

        public async Task<ActionResult> Students(Guid schoolId)
        {
            Guard.AgainstNullOrEmptyGuid(schoolId, nameof(schoolId));

            using (var unitOfWork = this.unitOfWorkFactory.CreateReadOnly())
            {
                var existingSchool =
                    await
                        unitOfWork.Repository<School>()
                            .Get()
                            .Include(s => s.Students)
                            .SingleOrDefaultAsync(s => s.Id == schoolId);
                if (existingSchool == null)
                {
                    throw new Entities.Exceptions.ApplicationException($"Could not find school with id '{schoolId}'");
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

            using (var unitOfWork = this.unitOfWorkFactory.CreateReadOnly())
            {
                var existingStudent =
                    await
                        unitOfWork.Repository<Student>().Get().SingleOrDefaultAsync(s => s.Id == studentId.Value);
                if (existingStudent == null)
                {
                    throw new Entities.Exceptions.ApplicationException($"Could not find student with id {studentId}");
                }

                var model = StudentModel.CreateFromEntity(existingStudent);
                model.SchoolId = schoolId;
                return this.PartialView("StudentDetails", model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmDelete(Guid studentId, Guid schoolId)
        {
            using (var unitOfWork = this.unitOfWorkFactory.CreateReadOnly())
            {
                var existingStudent =
                    await
                        unitOfWork.Repository<Student>().Get().SingleOrDefaultAsync(s => s.Id == studentId);
                if (existingStudent == null)
                {
                    throw new Entities.Exceptions.ApplicationException($"Could not find student with id {studentId}");
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

            using (var unitOfWork = this.unitOfWorkFactory.Create())
            {
                if (!model.Id.HasValue || model.Id.Value == Guid.Empty)
                {
                    var existingSchool =
                        await unitOfWork.Repository<School>().Get().Include(s => s.Students).SingleOrDefaultAsync(s => s.Id == model.SchoolId);

                    if (existingSchool == null)
                    {
                        throw new Entities.Exceptions.ApplicationException($"Could not find school with id {model.SchoolId}");
                    }

                    var newStudent = model.ToStudentEntity();
                    unitOfWork.Repository<Student>().Add(newStudent);
                    existingSchool.EnrollStudent(newStudent);
                }
                else
                {
                    var existingStudent =
                        await
                            unitOfWork.Repository<Student>().Get().SingleOrDefaultAsync(s => s.Id == model.Id);
                    if (existingStudent == null)
                    {
                        throw new Entities.Exceptions.ApplicationException($"Could not find student with id {model.Id}");
                    }

                    existingStudent.UpdateName(model.FirstName, model.LastName);
                }

                await unitOfWork.SaveChangesAsync();

                return this.Json(new { Result = "ok" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteStudent(StudentModel model)
        {
            using (var unitOfWork = this.unitOfWorkFactory.Create())
            {
                var existingSchool =
                        await unitOfWork.Repository<School>().Get().Include(s => s.Students).SingleOrDefaultAsync(s => s.Id == model.SchoolId);

                if (existingSchool == null)
                {
                    throw new Entities.Exceptions.ApplicationException($"Could not find school with id {model.SchoolId}");
                }

                var existingStudent =
                    await
                        unitOfWork.Repository<Student>().Get().SingleOrDefaultAsync(s => s.Id == model.Id);
                if (existingStudent == null)
                {
                    throw new Entities.Exceptions.ApplicationException($"Could not find student with id {model.Id}");
                }

                existingSchool.KickStudentOut(existingStudent);
                unitOfWork.Repository<Student>().Delete(existingStudent);

                await unitOfWork.SaveChangesAsync();

                return this.Json(new { Result = "ok" });
            }
        }
    }
}