using Microsoft.EntityFrameworkCore;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.SERVICES.InterfacesDepots;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<RPLPDbContext>(
         //options => options.UseSqlServer("Server=rplp.db; Database=RPLP; User Id=sa; password=Cad3pend86!"));
         options => options.UseSqlServer("Server=localhost,1433; Database=RPLP; User Id=sa; password=Cad3pend86!"));

builder.Services.AddScoped<IDepotClassroom, DepotClassroom>();
builder.Services.AddScoped<IDepotAdministrator, DepotAdministrator>();
builder.Services.AddScoped<IDepotAssignment, DepotAssignment>();
builder.Services.AddScoped<IDepotComment, DepotComment>();
builder.Services.AddScoped<IDepotOrganisation, DepotOrganisation>();
builder.Services.AddScoped<IDepotRepository, DepotRepository>();
builder.Services.AddScoped<IDepotStudent, DepotStudent>();
builder.Services.AddScoped<IDepotTeacher, DepotTeacher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
