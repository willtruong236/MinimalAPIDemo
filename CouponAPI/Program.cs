using System.Net;
using AutoMapper;
using CouponAPI;
using CouponAPI.Data;
using CouponAPI.Models;
using CouponAPI.Models.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/coupon", () =>
{
    APIResponse response = new APIResponse();
    response.Result = CouponStore.couponList;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("GetCoupons").Produces<APIResponse>(200);


app.MapGet("/api/coupon/{id:int}", (int id) =>
{
    APIResponse response = new APIResponse();
    response.Result = CouponStore.couponList.FirstOrDefault(u => u.Id == id);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("GetCoupon").Produces<APIResponse>(200);

app.MapPost("/api/coupon", async (IMapper _mapper, IValidator<CouponDTO> _validation, [FromBody] CouponDTO couponDTO) =>
{
    APIResponse response = new APIResponse
    {
        IsSuccess = false,
        StatusCode = HttpStatusCode.BadRequest
    };

    var validationResult = await _validation.ValidateAsync(couponDTO);
    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }
    if (CouponStore.couponList.FirstOrDefault(u => u.Name.ToLower() == couponDTO.Name.ToLower()) != null)
    {
        response.ErrorMessages.Add("Coupon Name already Exists");
        return Results.BadRequest(response);
    }

    Coupon coupon = _mapper.Map<Coupon>(couponDTO);
    coupon.Id = CouponStore.couponList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
    CouponStore.couponList.Add(coupon);

    response.Result = coupon;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.Created;
    return Results.Created($"/api/coupon/{coupon.Id}", response);
}).WithName("CreateCoupon").Produces<APIResponse>(201).Produces(400);

app.MapPut("/api/coupon", async (IMapper _mapper, IValidator<UpdateCouponDTO> _validation,[FromBody] UpdateCouponDTO updateCouponDTO) =>
{
    APIResponse response = new APIResponse
    {
        IsSuccess = false,
        StatusCode = HttpStatusCode.BadRequest
    };

    if (CouponStore.couponList.FirstOrDefault(u => u.Id == updateCouponDTO.Id) == null)
    {
        response.ErrorMessages.Add("Invalid ID");
        return Results.BadRequest(response);
    }
    var validationResult = await _validation.ValidateAsync(updateCouponDTO);
    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }

    Coupon couponFromStore = CouponStore.couponList.FirstOrDefault(u => u.Id == updateCouponDTO.Id);
    couponFromStore.Name = updateCouponDTO.Name;
    couponFromStore.Percent = updateCouponDTO.Percent;
    couponFromStore.IsActive = updateCouponDTO.IsActive;
    couponFromStore.LastUpdated = DateTime.Now;

    response.Result = _mapper.Map<CouponDTO>(couponFromStore);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("UpdateCoupon").Produces<APIResponse>(200).Produces(400);

app.MapDelete("/api/coupon/{id:int}", (int id) =>
{
    APIResponse response = new APIResponse
    {
        IsSuccess = false,
        StatusCode = HttpStatusCode.BadRequest
    };

    Coupon couponFromStore = CouponStore.couponList.FirstOrDefault(u => u.Id == id);
    if (couponFromStore == null)
    {
        response.ErrorMessages.Add("Invalid ID");
        return Results.BadRequest(response);
    }
    else
    {
        CouponStore.couponList.Remove(couponFromStore);

        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }
}).WithName("DeleteCoupon").Produces<APIResponse>(200).Produces(400); ;

app.UseHttpsRedirection();

app.Run();