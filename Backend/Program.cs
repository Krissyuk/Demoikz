using Microsoft.AspNetCore.Mvc;
using System.Numerics;

List<Order> repo =
[
    new(1,new(2005,3,12),"холодос","Не морозит","Не морозит совсем","Брелова Кристина","Не назначен"),
];

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
var app = builder.Build();

app.UseCors(a => a.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapGet("/", () => "Hello World!");

app.Run();

class Order(int number, DateOnly startDate, string device, string problemtype, string description, string fullname, string status)
{
    public int Number { get; set; } = number;
    public DateOnly StartDate { get; set; } = startDate;
    public string Device { get; set; } = device;
    public string ProblemType { get; set; } = problemtype;
    public string Description { get; set; } = description;
    public string Fullname { get; set; } = fullname;
    public string Status { get; set; } = status;
    public string? Master { get; set; } = "Не назначен";
    public DateOnly? EndDate { get; set; } = null;
    public List<string> Comment { get; set; } = [];
}
