using DotNet.Testcontainers.Builders;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Xunit;

namespace StronglyTypedId.Tests
{
	[CollectionDefinition(nameof(ProductsTestsCollection))]
	public class ProductsTestsCollection : ICollectionFixture<DbContainerFixture>
	{
	}

	public class DbContainerFixture : IAsyncLifetime
	{
		private MsSqlContainer _container;
		public MsSqlContainer Container => _container;

		public SqlConnection GetConnection()
		{
			SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_container.GetConnectionString());
			builder.InitialCatalog = "ProductsAPITests";
			builder.PersistSecurityInfo = true; //to allow .EnsureDeleted() to work
			return new SqlConnection(builder.ConnectionString);
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



		public async Task DisposeAsync()
		{
			await _container.StopAsync();
		}

	}
}