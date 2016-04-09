using System.Collections.Generic;
using SSW.DataOnion.Sample.Entities;

namespace SSW.DataOnion.Sample.WebUI.Models
{
    public class StudentsModel
    {
        public List<Student> Students { get; set; }

        public Student CurrentStudent { get; set; }

        public School School { get; set; }
    }
}
