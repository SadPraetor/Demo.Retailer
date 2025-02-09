using Demo.Retailer.Api.DataAccess;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Xunit;

namespace Tests.APITests
{
	[CollectionDefinition(nameof(ProductsTestsCollection))]
	public class ProductsTestsCollection : ICollectionFixture<APITestsFixture>
	{
	}

	public class APITestsFixture : WebApplicationFactory<Program>, IAsyncLifetime
	{
		private MsSqlContainer _container;
		public MsSqlContainer Container => _container;

		public SqlConnection GetConnection()
		{
			return new SqlConnection(GetConnectionString());
		}

		private string GetConnectionString()
		{
			SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_container.GetConnectionString());
			builder.InitialCatalog = "ProductsAPITests";
			builder.PersistSecurityInfo = true; //to allow .EnsureDeleted() to work
			return builder.ConnectionString;
		}

		public async Task InitializeAsync()
		{
			_container = new MsSqlBuilder()
				.WithAutoRemove(true)
				.WithImage("mcr.microsoft.com/mssql/server:2022-CU16-ubuntu-22.04")
				.WithPassword("Password!123")
				.WithPortBinding(1433)
				.WithName("mssql")
				.WithEnvironment("ACCEPT_EULA", "Y")
				.WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
				.WithCleanUp(false)
				.Build();

			await _container.StartAsync();
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				var dbContextDescriptor = services.SingleOrDefault(
					d => d.ServiceType ==
						typeof(DbContextOptions<ProductsDbContext>));

				services.Remove(dbContextDescriptor);

				var dbConnectionDescriptor = services.SingleOrDefault(
					d => d.ServiceType ==
						typeof(DbConnection));

				services.Remove(dbConnectionDescriptor);

				services.AddDbContext<ProductsDbContext>((container, options) =>
				{			
					options.UseSqlServer(GetConnectionString());
				});
			});
		}

		public async override ValueTask DisposeAsync()
		{
			await _container.StopAsync();
			await base.DisposeAsync();
		}

		async Task IAsyncLifetime.DisposeAsync()
		{
			await this.DisposeAsync();
		}
	}
}