using TaskCBT.WebApp;

var builder = WebApplication.CreateBuilder(args);

Variable.Raw = builder.Configuration.GetRequiredSection(Variable.SectionKey).Get<Variable>()!;
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
