using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Backend.Areas.Identity.Data; // Adjust based on your project structure

var builder = WebApplication.CreateBuilder(args);

// ✅ **1. Configure Database**
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") 
    ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// ✅ **2. Configure Identity & Authentication**
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// ✅ **3. Add Health Checks (without AddDbContextCheck)**
builder.Services.AddHealthChecks(); // No EntityFrameworkCore extension needed

// ✅ **4. Register Controllers & API-related Services**
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Enables API exploration (Swagger)
builder.Services.AddSwaggerGen(); // Adds Swagger for API testing

var app = builder.Build();

// ✅ **5. Apply Database Migrations Automatically**
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// ✅ **6. Configure Middleware**
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// ✅ **7. Map Controllers & Health Checks**
app.MapControllers(); // Maps API Controllers
app.MapHealthChecks("/health"); // Health check endpoint

// ✅ **8. Start the Application**
app.Run();
