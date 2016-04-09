using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SSW.DataOnion.Sample.Entities;

namespace SSW.DataOnion.Sample.WebUI.Models
{
    public class StudentModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        public Guid SchoolId { get; set; }

        public static StudentModel CreateFromEntity(Student student)
        {
            return new StudentModel
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName
            };
        }

        public Student ToStudentEntity()
        {
            return Student.Create(this.FirstName, this.LastName);
        }
    }
}
