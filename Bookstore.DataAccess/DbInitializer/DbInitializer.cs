using Bookstore.DataAccess.Data;
using Bookstore.Models.Models;
using Bookstore.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.DataAccess.DbInitializer
{
	public class DbInitializer : IDbInitializer
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly Entities _entities;

		public DbInitializer(
			UserManager<IdentityUser> userManager,
			RoleManager<IdentityRole> roleManager,
			Entities entities)
		{
			_entities = entities;
			_userManager = userManager;
			_roleManager = roleManager;
		}


		public void Initialize()
		{
			// Migrations if they are not applied.
			try
			{
				if (_entities.Database.GetPendingMigrations().Count() > 0)
					_entities.Database.Migrate();
			}
			catch (Exception ex) { }

			// Create roles if they are not created.
			if (!_roleManager.RoleExistsAsync(StaticDetails.Role_Customer).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Customer)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Employee)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Admin)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Company)).GetAwaiter().GetResult();

				// If roles are not created, then we will create admin user as well
				_userManager.CreateAsync(new ApplicationUser
				{
					UserName = "admin@gmail.com",
					Email = "admin@gmail.com",
					FirstName = "admin",
					LastName = "admin",
					PhoneNumber = "123456789",
					StreetAddress = "admin",
					City = "admin",
					PostalCode = "admin",
				},"Adnin123#").GetAwaiter().GetResult();

				ApplicationUser user = _entities.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@gmail.com");
				_userManager.AddToRoleAsync(user, StaticDetails.Role_Admin).GetAwaiter().GetResult();
			}

			return;
			
		}
	}
}
