// Program.cs
using DynamicAppGenerator.Data;
using DynamicAppGenerator.Repositories;
using DynamicAppGenerator.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Register services
builder.Services.AddScoped<LayoutsService>();
builder.Services.AddScoped<LayoutEditorService>();
builder.Services.AddScoped<GeneratorService>();
// Register database connection
builder.Services.AddSingleton<DatabaseConnection>();
builder.Services.AddScoped<LayoutsService>();
// Register repositories
builder.Services.AddScoped<RoleRepository>();

// Register template service with template directory
builder.Services.AddSingleton<TemplateService>(provider =>
    new TemplateService(Path.Combine(builder.Environment.ContentRootPath, "Templates")));

// Register generator service
builder.Services.AddScoped<GeneratorService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
