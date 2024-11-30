using Microsoft.AspNetCore.Mvc;

List<Order> repo =
[
    new(1,new(2005,3,12),"холодос","Не морозит","Не морозит совсем","Брелова Кристина","Не назначен"),
];

var builder = WebApplication.CreateBuilder();
builder.Services.AddCors();
var app = builder.Build();

app.UseCors(a => a.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

string message = "";

app.MapGet("orders", (int param = 0) =>
{
    string buffer = message;
    message = "";
    if (param != 0)
        return new {repo = repo.FindAll(x=>x.Number==param), message = buffer};
    return new { repo, message = buffer };
});

app.MapGet("create", ([AsParameters] Order dto) =>
    repo.Add(dto));

app.MapGet("update", ([AsParameters] UpdateOrderDTO dto) => 
{ 
    var o = repo.Find(x => x.Number == dto.Number);
    if (o == null)
        return;
    if (dto.Status != o.Status && dto.Status != "")
    {
        o.Status = dto.Status;
        message += $"Статус заявки №{o.Number} изменен\n";
        if (o.Status == "Выполнено")
        {
            message += $"Заявка №{o.Number} завершена\n";
            o.EndDate = DateOnly.FromDateTime(DateTime.Now);
        }
    }
    if (dto.Description != "")
        o.Description = dto.Description;
    if (dto.Master != "")
        o.Master = dto.Master;
    if (dto.Comment != "")
        o.Comment.Add(dto.Comment);
});

int complete_count() => repo.FindAll(x => x.Status == "выполнено").Count;

Dictionary<string, int> get_problem_type_stat() =>
    repo.GroupBy(x => x.ProblemType)
    .Select(x => (x.Key, x.Count()))
    .ToDictionary(k => k.Key, v => v.Item2);

double get_average_time_to_complete() =>
    complete_count() == 0 ? 0 :
    repo.FindAll(x => x.Status == "Выполнено")
    .Select(x => x.EndDate.Value.DayNumber - x.StartDate.DayNumber)
    .Sum() / complete_count();

app.MapGet("/statistics", () =>new{
    complete_count = complete_count(),
    problem_type_stat = get_problem_type_stat(),
    average_time_to_complete = get_average_time_to_complete()
});

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

record class UpdateOrderDTO(int Number, string? Status ="", string? Description = "", string? Master = "", string? Comment = "");