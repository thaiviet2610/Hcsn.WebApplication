using Hcsn.WebApplication.BL.AssetBL;
using Hcsn.WebApplication.BL.AssetIncrementBL;
using Hcsn.WebApplication.BL.BaseBL;
using Hcsn.WebApplication.DL.AssetDL;
using Hcsn.WebApplication.DL.AssetIncrementDL;
using Hcsn.WebApplication.DL.BaseDL;
using Hcsn.WebApplication.DL.DBConfig;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
}
        );

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency injection
builder.Services.AddScoped<IAssetBL, AssetBL>();
builder.Services.AddScoped<IAssetDL, AssetDL>();
builder.Services.AddScoped<IAssetIncrementDL, AssetIncrementDL>();
builder.Services.AddScoped<IAssetIncrementBL, AssetIncrementBL>();
builder.Services.AddScoped<IRepositoryDB, RepositoryDB>();
builder.Services.AddScoped(typeof(IBaseDL<>), typeof(BaseDL<>));
builder.Services.AddScoped(typeof(IBaseBL<>), typeof(BaseBL<>));

DatabaseContext.ConnectionString = builder.Configuration.GetConnectionString("MySql");
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddCors(p => p.AddPolicy("MyCors", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("MyCors");

app.UseAuthorization();

app.MapControllers();

app.Run();
