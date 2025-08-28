
using POECLDV6212.Services;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddControllersWithViews();

//Register Table Storage with configuration
builder.Services.AddSingleton(new Table_Service(configuration.GetConnectionString("AzureStorage")));

//Register Blob Service
builder.Services.AddSingleton(new Blob_Service(configuration.GetConnectionString("AzureStorage")));

//Register Queue Services
builder.Services.AddSingleton<QueueService>(sp =>
{
    var connectionString = configuration.GetConnectionString("AzureStorage");
    return new QueueService(connectionString, "orders");
});

//Register FileSharing Services
builder.Services.AddSingleton<File_Service>(sp =>
{
    var connectionString = configuration.GetConnectionString("AzureStorage");
    return new File_Service(connectionString, "dummycontracts");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();