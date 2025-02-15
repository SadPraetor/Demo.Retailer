using Demo.Retailer.MigrationService.DevDataSeed;
using FluentAssertions;
using Xunit;

namespace Demo.Retailer.Tests.Demo.Retailer.MigrationService.Tests
{
	public class FakerGeneratorsTests
	{
		[Fact]
		public void LineItemFaker_ShouldPickFromProvidedLists()
		{
			var sut = new LineItemFaker();

			int[] orderIds = [199, 215, 777, 819];
			int[] productIds = [222, 1415, 7777, 1919];

			var items = sut.GetFakeLineItems(orderIds, productIds, 20);

			items.Should()
				.AllSatisfy(lineItem =>
				{
					lineItem.ProductId.Should().BeOneOf(productIds);
					lineItem.OrderId.Should().BeOneOf(orderIds);
				});


		}
	}
}
