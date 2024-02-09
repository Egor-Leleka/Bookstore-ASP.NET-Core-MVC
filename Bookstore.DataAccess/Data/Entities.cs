using Bookstore.Models.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.DataAccess.Data
{
    public class Entities : IdentityDbContext
    {
        // Creat tabels in database.
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        public Entities(DbContextOptions<Entities> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data.
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Sci-Fi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Comics", DisplayOrder = 3 },
                new Category { Id = 4, Name = "Fantasy", DisplayOrder = 4 },
                new Category { Id = 5, Name = "Horror", DisplayOrder = 5 }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Harry Potter and the Philosopher's Stone",
                    Description = "Harry Potter has never even heard of Hogwarts when the letters " +
                    "start dropping on the doormat at number four, Privet Drive. Addressed in green " +
                    "ink on yellowish parchment with a purple seal, they are swiftly confiscated by " +
                    "his grisly aunt and uncle. Then, on Harry’s eleventh birthday, a great beetle-eyed " +
                    "giant of a man called Rubeus Hagrid bursts in with some astonishing news: Harry " +
                    "Potter is a wizard, and he has a place at Hogwarts School of Witchcraft and Wizardry. " +
                    "An incredible adventure is about to begin!\r\nJ.K. Rowling’s internationally " +
                    "bestselling Harry Potter books continue to captivate new generations of readers. " +
                    "Harry’s first adventure alongside his friends, Ron and Hermione, will whisk you away to Hogwarts," +
                    " an enchanted, turreted castle filled with disappearing staircases, pearly-white ghosts and magical" +
                    " paintings that flit from frame to frame. This gorgeous paperback edition features a spectacular " +
                    "cover by award-winning artist Jonny Duddle, as well as refreshed bonus material including fun facts" +
                    "exploring the origins of names such as Albus Dumbledore, Hedwig and other favourite characters. " +
                    "This is the perfect starting point for anyone who’s ready to lose themselves in the biggest children’s books of all time.",
                    ISBN = "9781408855652",
                    Author = "J. K. Rowling",
                    ListPrice = 29.99m,
                    Price = 21.99m,
                    Price50 = 18.99m,
                    Price100 = 15.99m,
                    CategoryId = 1
                },

                new Product
                {
                    Id = 2,
                    Title = "Harry Potter and the Chamber of Secrets",
                    Description = "Harry Potter’s summer has included the worst birthday ever, doomy warnings from a house-elf called Dobby and " +
                    "rescue from the Dursleys by his friend Ron Weasley in a magical flying car! Back at Hogwarts School of Witchcraft and Wizardry" +
                    " for his second year, Harry hears strange whispers echo through empty corridors – and then the attacks start. " +
                    "Students are found as though turned to stone … Dobby’s sinister predictions seem to be coming true.\r\nJ.K. Rowling’s " +
                    "internationally bestselling Harry Potter books continue to captivate new generations of readers. Harry’s second adventure " +
                    "alongside his friends, Ron and Hermione, invites you to explore even more of the wizarding world; from the waving, walloping " +
                    "branches of the Whomping Willow to the thrills of a rain-streaked Quidditch pitch. This gorgeous hardback edition features " +
                    "a spectacular cover by award-winning artist Jonny Duddle, plus refreshed bonus material, allowing readers to learn about wand " +
                    "woods and get to know the many members of the Weasley family. Perfect for anyone who’s ready to lose themselves in the biggest " +
                    "children’s books of all time.",
                    ISBN = "9781408855904",
                    Author = "J. K. Rowling",
                    ListPrice = 29.99m,
                    Price = 21.99m,
                    Price50 = 18.99m,
                    Price100 = 15.99m,
                    CategoryId = 3
                },

                new Product
                {
                    Id = 3,
                    Title = "Harry Potter and the Prisoner of Azkaban",
                    Description = "Harry Potter, along with his best friends, Ron and Hermione, is about to start his third year at Hogwarts School of " +
                    "Witchcraft and Wizardry. Harary can't wait to get back to school after the summer holidays. (Who wouldn't if they lived with the " +
                    "horrible Dursleys?) But when Harry gets to Hogwarts, the atmosphere is tense. There's an escaped mass murderer on the loose, and the " +
                    "sinister prison guards of Azkaban have been called in to guard the school. 'The most eagerly awaited children's book for years." +
                    "' - \"The Evening Standard\". 'The Harry Potter books are that rare thing, a series of stories adored by by parents and children alike'." +
                    " - \"The Daily Telegraph\". All the Harry Potter books are now available in large print.",
                    ISBN = "9780747560777",
                    Author = "J. K. Rowling",
                    ListPrice = 29.99m,
                    Price = 21.99m,
                    Price50 = 18.99m,
                    Price100 = 15.99m,
                    CategoryId = 1
                },

                new Product
                {
                    Id = 4,
                    Title = "Harry Potter and the Goblet of Fire",
                    Description = "The Triwizard Tournament is to be held at Hogwarts. Only wizards who are over seventeen are allowed to enter – but " +
                    "that doesn’t stop Harry dreaming that he will win the competition. Then at Hallowe’en, when the Goblet of Fire makes its selection, " +
                    "Harry is amazed to find his name is one of those that the magical cup picks out. He will face death-defying tasks, dragons and Dark wizards, " +
                    "but with the help of his best friends, Ron and Hermione, he might just make it through – alive!\r\nJ.K. Rowling’s internationally " +
                    "bestselling Harry Potter books continue to captivate new generations of readers. Harry’s fourth adventure invites you to explore even more " +
                    "of the wizarding world; from the foggy, frozen depths of the Great Lake to the silvery secrets of the Pensieve. This gorgeous paperback " +
                    "edition features a spectacular cover by award-winning artist Jonny Duddle, plus refreshed bonus material, allowing readers to learn more " +
                    "about the different breeds of dragon. Perfect for anyone who’s ready to lose themselves in the biggest children’s books of all time.",
                    ISBN = "9781408855683",
                    Author = "J. K. Rowling",
                    ListPrice = 29.99m,
                    Price = 21.99m,
                    Price50 = 18.99m,
                    Price100 = 15.99m,
                    CategoryId = 2
                }
            );

            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    Id = 1,
                    Name = "Penguin Random House",
                    StreetAddress = "67 Apollo Drive Rosedale",
                    City = "Auckland",
                    State = "New Zealand",
                    PostalCode = "0632",
                    PhoneNumber = "+64-9-442-7400"
                },

                new Company
                {
                    Id = 2,
                    Name = "Harper Collins",
                    StreetAddress = "PO Box 1, Shortland Street",
                    City = "Auckland",
                    State = "New Zealand",
                    PostalCode = "1140",
                    PhoneNumber = ""
                },

                new Company
                {
                    Id = 3,
                    Name = "Simon and Schuster",
                    StreetAddress = "Suite 19a, Level 1, Building C, 450 Miller Street",
                    City = "Sydney",
                    State = "Australia",
                    PostalCode = "2062",
                    PhoneNumber = "+61-2-9983-6600"
                });
        }
    }
}
