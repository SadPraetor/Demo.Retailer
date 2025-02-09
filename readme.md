# Retailer Interview - API <!-- omit in toc -->

> Comment 2025-01-21  
I decided to update the project. Original interview was conducted in Sep 2021, with instruciton to use latest LTS .NET version (3.1 at the time).  
Project is currently only a demo in my portfolio, and not all the requirements from the task need to be met. 
I disregard some, or explore in other areas.
Original version can be found on branches `original/master` and `original/repository_pattern`.  
> - I'm updating this to .NET 9 version
> - Decided to completely disregard repository pattern

> ### Current TODO list
> - [X] Minimal API
> - [X] Response caching
> - [X] Verify snapshot testing
> - [X] Implement route building adjusted for proxy scenario
> - [X] Cleanup tests

> - [ ] Aspire implementation
> - [ ] Postgres
> - [ ] Expand data model
> - [ ] Migration project
> - [ ] Image deploy to container
> - [ ] reverse proxy, horizontal scaling
> - [ ] Grafana, Loki, Prometheus stack
> - [ ] OData demo endpoints
> - [ ] expand tests
> - [ ] Logs, open telemetry, metrics
> - [ ] GraphQL demo endpoints
> - [ ] OneOf pattern to remove throwing exceptions (performance)
> - [ ] K6 load test
> - [ ] distrubuted response caching



> :heavy_exclamation_mark: readme is not yet updated, some of the work can still be on develop branch

## Table of contents <!-- omit in toc -->
- [1. Project description](#1-project-description)
- [2. Technology description](#2-technology-description)
- [3. Project dependencies](#3-project-dependencies)
  - [3.1. API project](#31-api-project)
  - [3.2. Testing Project](#32-testing-project)
- [4. Local project setup](#4-local-project-setup)
- [5. DEV Data Seed](#5-dev-data-seed)
- [6. Code](#6-code)
  - [6.1. Api Versioning](#61-api-versioning)
  - [6.2. Update endpoint](#62-update-endpoint)
  - [6.3. Repository pattern vs. EF Core DbContext](#63-repository-pattern-vs-ef-core-dbcontext)
- [7. Unit Testing](#7-unit-testing)
- [8. Possible improvements](#8-possible-improvements)
- [9. Known-Issues](#9-known-issues)

<br/>

<a name="1-project-description"></a>

## 1. Project description
This project was given as interview task by a retailer company. The task is to create REST API, with few basic endpoints, API versioning, data seed, unit tests and swagger documentation. <br/>
Full description of the task can be found in the file [task.txt](./task.txt) within the repository.

<br>

<a name="2-technology-description"></a>

## 2. Technology description
Solution is based on <span>ASP.</span>NET Core 3.1 -> this is as of day of writing the code long term supported .NET version. This was one of the requirements given. <br/>
Backend database implemented in the code is MSSQL, and access to data is managed by EF Core. <br/>
Unit tests are written in xUnit
<br/>
<br/>
Solution was written in Visual Studio 2019.

<br/>

<a name="3-project-dependencies"></a>

## 3. Project dependencies
### 3.1. API project
```
Bogus Version=33.1.1
Microsoft.AspNetCore.Mvc.Versioning - Version=5.0.0 
Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer - Version=5.0.0
Microsoft.EntityFrameworkCore - Version=5.0.9
Microsoft.EntityFrameworkCore.SqlServer - Version=5.0.9
Microsoft.EntityFrameworkCore.Tools - Version=5.0.9
```
<br/>

### 3.2. Testing Project
```
Microsoft.EntityFrameworkCore.InMemory - Version=5.0.9
Moq - Version=4.16.1
xunit - Version=2.4.1
```

<br/>
<a name="4-local-project-setup"></a>

## 4. Local project setup
1. Clone the project from the repo
2. Adjust `appsettings.Development.json` connection string to database of your choosing. It is set by default to MSSQL Express (Visual Studio), so assuming database is running. Read [chapter 5](#5-dev-data-seed)
3. Run command `dotnet run` from `./src/` folder -> this should start project listening on `https://localhost:5001` and `http://localhost:5000`.<br/>
Starting project in Visual Studio should also produce similar result. Hosting on IIS Express is set to be on ports `http://localhost:7410` and `https://localhost:44309`.


<br/>
<a name="5-dev-data-seed"></a>

## 5. DEV Data Seed
If the projects runs on Development environment, on startup it will try to migrate the database and seed test data into it.

1. Adjust configuration file `appsettings.Development.json` connection string
    ```
    "ConnectionStrings": {
    "ProductsDb": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CommerceDb;Integrated Security=True;ApplicationIntent=ReadOnly"}
    ```

    Your user requires corresponding roles to execute the migration, something in line of

    ```
    GRANT ALTER, INSERT, SELECT, DELETE, UPDATE, REFERENCES ON SCHEMA::schema_name TO migration_user
    GRANT CREATE TABLE TO migration_user
    ```

2. On startup part of code that runs migration and data seed is run
    ```
    public static void Main( string[] args ) {
        var hostBuilder = CreateHostBuilder( args ).Build();

        if ( Environment.GetEnvironmentVariable( "ASPNETCORE_ENVIRONMENT" ) == Environments.Development ) {
            hostBuilder
                .MigrateDatabase()
                .SeedDatabaseIfEmpty(300);
        };

        hostBuilder.Run();
    }
    ```
    `.MigrateDatabase` -> runs the migration, creates table `Products` <br/>
    `.SeedDatabaseIfEmpty(int count)` -> seeds table with data, `count` value specifies how many records you want to add


<br/>

<a name="6-code"></a>

## 6. Code
<br/>

### 6.1. Api Versioning
API Versioning approach chosen was to use Header value. Header used is `X-Api-Version`.  If not specified defaults to v1.0.<br/>

> URL versioning was considered.


<br/>

### 6.2. Update endpoint
Update endpoint path is defined as `api/{id}/description`, although `description` is a property of resource and not a resource. Together with method  `PATCH` it should clearly indicate to user that this endpoint only updates a description. Input is `text/plain`, a new description on product resource. <br/>
<span>ASP</span>.NET Core does not support `text/plain` reading from body out of the box. A custom `InputFormatter` was implemented in solution

<br/>

### 6.3. Repository pattern vs. EF Core DbContext
There are two branches of interest. `master` does not implement repository pattern and exposes EF Core DbContext directly in controller It is not wrapped in repository pattern.

<br/>

> EF Core already implements Repository pattern (DbSet) and Unit of Work pattern (DbContext) <br/>
> Generally speaking, back-end database format tends to be stable in applications.<br/>
> Having LINQ directly exposed is neat. <br/>
> This is small project, so adding another layer seems redundant. I would just wrap few lines of code that are currently in controllers in a repository. 
> <br/>
> It makes it more difficult to setup tests. Within the project though there is no business logic to be tested.<br/>
> I would lean towards use of repository pattern for complex data access (i.e. multiple database, API calls, SQL query written)   

Second branch of interest is `repositorypattern`. As name suggests, EF Core DbContext is wrapped in repository. 

> I was not really sure how much is this expected in the solution, so I provided at least a glance of solution with this pattern

<br/>
<a name="7-tests"></a>

## 7. Unit Testing
Most of the unit tests that are run on `master` branch without repository pattern are run against controller. In Memory Database is used to provide DbContext. These tests are closer to integration tests than unit tests.
<br/>
<br/>
Tests run against utility classes can be considered as unit tests.
<br/>
<br/>
`repositorypattern` branch has different sets of tests run against controllers, with more extensive mocking of services/objects.
<br/>
Tests are done with xUnit, and can be easily executed in Visual Studio Test Explorer.
 

<br/>
<a name="8-possible-improvements"></a>

## 8. Possible improvements
* Implementing better logging 
  > My personal preference is Serilog
* Wrapping exception handling of response in Middleware or Action Filter
  > Currently exception object creation is handled in controller methods

<br/>

* many more small items

<br/>

<a name="9-known-issues"></a>

## 9. Known-Issues
none right now