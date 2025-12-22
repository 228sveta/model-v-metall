using HeatExchangeCalculator.Services;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<HeatExchangeService>();
builder.Services.AddSingleton<SimpleDbService>();

var app = builder.Build();

// Настройка pipeline
if (!app.Environment.IsDevelopment())
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