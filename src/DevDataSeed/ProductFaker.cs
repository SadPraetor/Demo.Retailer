using API.Models;
using Bogus;
using System;
using System.Collections.Generic;

namespace API.DevDataSeed
{
	public class ProductFaker
	{

		private Faker<Product> _faker;

		public ProductFaker(int useSeed = 999)
		{
			_faker = new Faker<Product>().UseSeed(useSeed)
				.RuleFor(p => p.Name, f => f.Commerce.ProductName())
				.RuleFor(p => p.ImgUri, (f) => f.Image.PlaceholderUrl(width: 300, height: 300, text: null, backColor: "products", textColor: f.Random.Guid().ToString(), format: "png"))
				.RuleFor(p => p.Price, f => f.Finance.Amount(1, 1000, 2))
				.RuleFor(p => p.Description, f => f.Commerce.ProductDescription().OrNull(f, .05f));
		}

		public List<Product> GetFakeProducts(int count = 1)
		{
			return _faker.Generate(count);
		}
	}
}
