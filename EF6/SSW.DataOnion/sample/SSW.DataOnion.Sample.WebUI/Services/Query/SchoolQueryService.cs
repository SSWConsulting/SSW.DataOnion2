using System;
using System.Linq;
using SSW.DataOnion.Interfaces;
using SSW.DataOnion.Sample.Data;
using SSW.DataOnion.Sample.Entities;
using Guard = SSW.DataOnion.Sample.Entities.Exceptions.Guard;

namespace SSW.DataOnion.Sample.WebUI.Services.Query
{
    public interface ISchoolQueryService
    {
        Student GetStudentDetails(Guid studentId);
        Student GetStudentDetails2(Guid studentId);
    }

    public class SchoolQueryService : ISchoolQueryService
    {
        private readonly IAmbientDbContextLocator dbContextLocator;
        private readonly IRepositoryLocator repositoryLocator;

        public SchoolQueryService(IAmbientDbContextLocator dbContextLocator, IRepositoryLocator repositoryLocator)
        {
            Guard.AgainstNull(dbContextLocator, nameof(dbContextLocator));
            Guard.AgainstNull(repositoryLocator, nameof(repositoryLocator));

            this.dbContextLocator = dbContextLocator;
            this.repositoryLocator = repositoryLocator;
        }

        public Student GetStudentDetails(Guid studentId)
        {
            Guard.AgainstNullOrEmptyGuid(studentId, nameof(studentId));
            var student = this.repositoryLocator.GetRepository<Student>().Get().SingleOrDefault(s => s.Id == studentId);
            return student;
        }

        public Student GetStudentDetails2(Guid studentId)
        {
            Guard.AgainstNullOrEmptyGuid(studentId, nameof(studentId));
            var student = this.dbContextLocator.Get<SchoolDbContext>().Students.SingleOrDefault(s => s.Id == studentId);
            return student;
        }
    }
}
