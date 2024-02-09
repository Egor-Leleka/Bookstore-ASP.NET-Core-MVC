using BookstoreWebRazor.Models;
using Microsoft.EntityFrameworkCore;

namespace BookstoreWebRazor.Data
{
	public class Entities: DbContext
	{
		public DbSet<Category> Categories { get; set; }

        public Entities(DbContextOptions<Entities> options): base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Seed data.
			modelBuilder.Entity<Category>().HasData(
				new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
				new Category { Id = 2, Name = "Sci-Fi", DisplayOrder = 2 },
				new Category { Id = 3, Name = "Comics", DisplayOrder = 3 },
				new Category { Id = 4, Name = "Fantasy", DisplayOrder = 4 },
				new Category { Id = 5, Name = "Horror", DisplayOrder = 5 }
				);
		}
	}
}
