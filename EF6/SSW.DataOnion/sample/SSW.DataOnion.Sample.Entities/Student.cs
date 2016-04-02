using System;
using SSW.DataOnion.Sample.Entities.Exceptions;

namespace SSW.DataOnion.Sample.Entities
{
    public class Student : AggregateRoot
    {
        protected Student()
        {
        }

        private Student(string firstName, string lastName)
        {
            Guard.AgainstNullOrEmptyString(firstName, nameof(firstName));
            Guard.AgainstNullOrEmptyString(lastName, nameof(lastName));

            this.Id = Guid.NewGuid();
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public static Student Create(string firstName, string lastName)
        {
            return new Student(firstName, lastName);
        }

        public string FirstName { get; protected set; }

        public string LastName { get; protected set; }

        public string FullName => $"{this.FirstName} {this.LastName}";

        public override string ToString()
        {
            return
                $"{nameof(this.Id)}: {this.Id}; {nameof(this.FirstName)}: {this.FirstName}; {nameof(this.LastName)}: {this.LastName};";
        }
    }
}
