using System.Collections.Generic;

namespace SSW.DataOnion.CodeGenerator.Tests.Unit.TestEntities
{
    public class School
    {
        public string Name { get; set; }

        public List<Student> Students { get; set; }
    }
}
