using Microsoft.EntityFrameworkCore;

using CRML.Server.Data;
using CRML.Server.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(options => {
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DatabaseContext>(options => {
    string name = string.Empty;

    if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)) {
        name = "Win32Connection";
    } else {
        name = "LinuxConnection";
    }

    options.UseSqlServer(builder.Configuration.GetConnectionString(name));
});

builder.Services.AddScoped<CRML.Server.Controllers.IControllerRepository<Customer>,    CustomerRepository>();
builder.Services.AddScoped<CRML.Server.Controllers.IControllerRepository<Appointment>, AppointmentRepository>();
builder.Services.AddScoped<CRML.Server.Controllers.IControllerRepository<Motif>,       MotifRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

