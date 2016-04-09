using System;
using System.Collections.Generic;
using SSW.DataOnion.Sample.Entities.Exceptions;

namespace SSW.DataOnion.Sample.Entities
{
    public class School : AggregateRoot
    {
        protected School()
        {
        }

        private School(string name, Address address)
        {
            Guard.AgainstNull(address, nameof(address));
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Address = address;
            this.Students = new List<Student>();
        }

        public static School Create(string name, Address address)
        {
            return new School(name, address);
        }

        public void EnrollStudent(Student student)
        {
            Guard.AgainstNull(student, nameof(student));
            this.Students.Add(student);
        }

        public void KickStudentOut(Student student)
        {
            Guard.AgainstNull(student, nameof(student));
            this.Students.Remove(student);
        }

        public string Name { get; protected set; }

        public Address Address { get; protected set; }

        public ICollection<Student> Students { get; protected set; }

        public override string ToString()
        {
            return
                $"{nameof(this.Name)}: {this.Name}; {nameof(this.Address)}: {this.Address}; {nameof(this.Students)}: {this.Students.Count};";
        }
    }
}
