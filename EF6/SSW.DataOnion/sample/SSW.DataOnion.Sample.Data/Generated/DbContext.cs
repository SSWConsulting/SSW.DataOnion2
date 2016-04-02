
using System.Data.Entity;

namespace SSW.DataOnion.Sample.Data
{
	using SSW.DataOnion.Sample.Entities;

	public partial class SchoolDbContext
	{
		public IDbSet<Address> Addresss { get; set; }

		public IDbSet<School> Schools { get; set; }

		public IDbSet<Student> Students { get; set; }

	}
}

