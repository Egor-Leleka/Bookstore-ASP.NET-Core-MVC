using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bookstore.Models.Models
{
	public class Product
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Title { get; set; }

        public string Description { get; set; }

		[Required]
		[MaxLength(13)]
		[MinLength(13)]
		public string ISBN { get; set; }

		[Required]
		[MaxLength(50)]
		[MinLength(2)]
		public string Author { get; set; }

		[Required]
		[DisplayName("List Price")]
        public decimal ListPrice { get; set; }

		[Required]
		[DisplayName("Price for 1-50")]
		public decimal Price { get; set; }

		[Required]
		[DisplayName("Price 51-100")]
		public decimal Price50 { get; set; }

		[Required]
		[DisplayName("Price 100+")]
		public decimal Price100 { get; set; }

		[DisplayName("Category")]
		public int CategoryId { get; set; }

		[ForeignKey("CategoryId")]
		[ValidateNever]
		public Category Category { get; set; }

		[ValidateNever]
		public List<ProductImage> ProductImages { get; set; }


    }
}
