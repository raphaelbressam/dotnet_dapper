using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Linq;
using TutorialDapper.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello Dapper!");

app.MapGet("/employees", async () =>
{
    using IDbConnection dbConnection = new SQLiteConnection("Data Source=E:\\Repos\\dotnet_dapper\\src\\Database\\contoso.db;Version=3;");
    var employees = (await dbConnection.QueryAsync<EmployeesModel>("SELECT * FROM Employees")).ToList();
    return employees;
});

app.MapPost("/employees", async () =>
{
    using IDbConnection dbConnection = new SQLiteConnection(app.Configuration.GetConnectionString("SQLite"));
    var employee = new EmployeesModel { Id = Guid.NewGuid().ToString(), Age = 36, FirstName = "Raphael", LastName = "Bressam" };
    var updatedRows = await dbConnection.ExecuteAsync("INSERT INTO Employees (Id,FirstName,LastName,Age) VALUES (@Id,@FirstName,@LastName,@Age);", employee);
    return updatedRows;
});

app.MapPut("/employees/{id}", async (string id) =>
{
    using IDbConnection dbConnection = new SQLiteConnection(app.Configuration.GetConnectionString("SQLite"));
    var updatedRows = await dbConnection.ExecuteAsync("UPDATE Employees SET Age=@Age WHERE Id=@Id;", new { Id = id, Age = DateTime.Now.Second });
    return updatedRows;
});

app.Run();
