using DotNetCoreBaseAPI;

var builder = WebApplication.CreateBuilder(args);

//Call base api application builder
builder.CreateWebApplicationBuilder(args);
Console.WriteLine(builder.Configuration.GetValue<string>("TestSetting2"));
Console.WriteLine(builder.Configuration.GetValue<string>("TestSetting1"));

//Configure Http pipeline
var app = builder.Build();

//Call base api http pipeline
app.BuildBaseHttpPipeline(builder.Configuration);

app.Run();
