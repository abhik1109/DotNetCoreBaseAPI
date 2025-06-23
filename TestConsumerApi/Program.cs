using DotNetCoreBaseAPI;

var builder = WebApplication.CreateBuilder(args);
builder.CreateWebApplicationBuilder(args);
Console.WriteLine(builder.Configuration.GetValue<string>("TestSetting2"));
Console.WriteLine(builder.Configuration.GetValue<string>("TestSetting1"));
var app = builder.Build();
app.BuildBaseHttpPipeline(builder.Configuration);

app.Run();
