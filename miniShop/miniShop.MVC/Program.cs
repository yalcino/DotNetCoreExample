﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using miniShop.Application.MapProfile;
using miniShop.Application.Services;
using miniShop.Infrastructure.Data;
using miniShop.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container (IoC).
/*
 *   1. Singleton: Sadece 1 instance al. Hep onu kullan
 *   2. Transient Her seferinde instance al. Hep farklı kullan.
 *   3. Scoped: Instance başka bir nesnenin içinde olduğu sürece aynı o nesne dispose olursa farklı olsun.
 */
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSession();

var connectionString = builder.Configuration.GetConnectionString("db");
builder.Services.AddDbContext<MiniShopDbContext>(option => option.UseSqlServer(connectionString));

builder.Services.AddAutoMapper(typeof(MapProfileForEntities));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.LoginPath = "/Users/Login";
                    option.ReturnUrlParameter = "nereyeGideyim";
                    option.AccessDeniedPath = "/Users/AccessDenied";
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
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(name: "categoryFilter",
                       pattern: "Kategori/{catId?}",
                       defaults: new { controller = "Home", action = "Index" }
                       );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
