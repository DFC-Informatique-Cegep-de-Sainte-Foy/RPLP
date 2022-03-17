using RPLP.DAL.SQL.Depots;
using RPLP.SERVICES.InterfacesDepots;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDepotClassroom, DepotClassroom>();
builder.Services.AddScoped<IDepotAdminitrator, DepotAdministrator>();
builder.Services.AddScoped<IDepotAssignment, DepotAssignments>();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
