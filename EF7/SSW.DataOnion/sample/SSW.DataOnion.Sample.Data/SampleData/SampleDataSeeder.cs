using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using SSW.DataOnion.Interfaces;
using SSW.DataOnion.Sample.Entities;

namespace SSW.DataOnion.Sample.Data.SampleData
{
	public class SampleDataSeeder : IDataSeeder
	{
	    public void Seed<TDbContext>(TDbContext dbContext) where TDbContext : DbContext
	    {
            this.InsertTestData(dbContext);
	        dbContext.SaveChanges();
	    }

	    public async Task SeedAsync<TDbContext>(
	        TDbContext dbContext,
	        CancellationToken cancellationToken = new CancellationToken()) where TDbContext : DbContext
	    {
	        this.InsertTestData(dbContext);
	        await dbContext.SaveChangesAsync(cancellationToken);
	    }

	    private void InsertTestData<TDbContext>(TDbContext dbContext) where TDbContext : DbContext
	    {
	        this.AddOrUpdate(dbContext, p => p.Id, Students);
            this.AddOrUpdate(dbContext, p => p.Id, Addresses);
            this.AddOrUpdate(dbContext, p => p.Id, Schools);
        }

	    private void AddOrUpdate<TDbContext, TEntity>(
	        TDbContext dbContext,
	        Func<TEntity, object> propertyToMatch,
	        IEnumerable<TEntity> entities) where TEntity : class where TDbContext : DbContext
	    {
	        // Query in a separate context so that we can attach existing entities as modified
	        var existingData = dbContext.Set<TEntity>().ToList();

	        foreach (var item in entities)
	        {
	            dbContext.Entry(item).State = existingData.Any(g => propertyToMatch(g).Equals(propertyToMatch(item)))
	                ? EntityState.Modified
	                : EntityState.Added;
	        }
	    }

        private static List<School> schools;
        public static List<School> Schools
        {
            get
            {
                if (schools == null)
                {
                    schools = new List<School>
                    {
                        School.Create("Qld State School", Addresses[0]),
                        School.Create("Sydney Private School", Addresses[1])
                    };

                    schools[0].EnrollStudent(students[0]);
                    schools[0].EnrollStudent(students[1]);
                    schools[0].EnrollStudent(students[2]);
                    schools[0].EnrollStudent(students[3]);
                    schools[0].EnrollStudent(students[4]);
                    schools[0].EnrollStudent(students[5]);
                    schools[0].EnrollStudent(students[6]);
                    schools[1].EnrollStudent(students[7]);
                    schools[1].EnrollStudent(students[8]);
                    schools[1].EnrollStudent(students[9]);
                    schools[1].EnrollStudent(students[10]);
                }

                return schools;
            }
        }

        private static List<Student> students;
        public static List<Student> Students
        {
            get
            {
                if (students == null)
                {
                    students = new List<Student>
                    {
                        Student.Create("Jessica", "Alba"),
                        Student.Create("Angelina", "Jolie"),
                        Student.Create("Brad", "Pitt"),
                        Student.Create("Tom", "Cruise"),
                        Student.Create("Jackie", "Chan"),
                        Student.Create("Sean", "Penn"),
                        Student.Create("Johny", "Depp"),
                        Student.Create("Leonardo", "DiCaprio"),
                        Student.Create("Tom", "Hanks"),
                        Student.Create("Sean", "Connery"),
                        Student.Create("Al", "Pacino")
                    };
                }

                return students;
            }
        }

        private static List<Address> addresses;
		public static List<Address> Addresses
        {
			get
			{
			    if (addresses == null)
			    {
			        addresses = new List<Address>
			        {
			            Address.Create("12 Test Street", string.Empty, "Brisbane", "4000", "Qld"),
			            Address.Create("42 Test Street", string.Empty, "Sydney", "2000", "Qld"),
			            Address.Create("123 Test Street", string.Empty, "Melbourne", "3000", "Qld")
			        };
			    }

			    return addresses;
			}
		}
	}
}