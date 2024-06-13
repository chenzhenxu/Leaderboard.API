using Leaderboard.API.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<CustomerScoreRank>();
builder.Services.AddSingleton<CustomerScoreRankSkipList>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.MapPost("/customer/{customerid}/score/{score}", (long customerid, decimal score, [FromServices] CustomerScoreRankSkipList customerScoreRankSkipList) =>
{
    if (score > 1000 || score < -1000)
    {
        return Results.BadRequest("Invalid score");
    }
    var newScore = customerScoreRankSkipList.UpdateCustomerScore(customerid, score);    
    
    return Results.Ok(newScore);
});

app.MapGet("/leaderboard", (int start, int end, [FromServices] CustomerScoreRankSkipList customerScoreRankSkipList) =>
{
    if (start==0)
    {
        return Results.BadRequest("start should be greater than 0");
    }
    if (end == 0)
    {
        return Results.BadRequest("end should be greater than 0");
    }
    if (end < start)
    {
        return Results.BadRequest("start rank should be less than or equal to end rank");
    }

    var customerRankResults = customerScoreRankSkipList.GetRankResults(start,end);
    return Results.Ok(customerRankResults);
});

app.MapGet("/leaderboard/{customerid}", (long customerid, int high, int low, [FromServices] CustomerScoreRankSkipList customerScoreRankSkipList) =>
{
    var customerRankResults = customerScoreRankSkipList.GetRankResults(customerid,high,low);
    return Results.Ok(customerRankResults);
});

app.Run();
