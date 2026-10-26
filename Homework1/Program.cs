using Homework1.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// ????????? ????? ?? ?????????? ??????????? (???????????)

var app = builder.Build();

//Створіть веб застосунок який показує сторінку про вас

//за адресою /
//коротка інформація Ім'я Прізвище, звідки вам скількит вам років
//формуйте повну веб сторінку, зі стилями (додайте трошки дизайну)
//додати посилання на сторінку /message

//на ній форма
//ім'я, пошта, повідомлення
//Відправити

//під формою список веж відправлених повідомлень
//Повідомлення зберігайте у файлі на сервері так щоб вони віднговлючалия навіть після перевантаження програми сервера
var filePath = "messages.json";

if (!File.Exists(filePath))
{
    File.Create(filePath);
    File.WriteAllText(filePath, "[]");
}

if (File.Exists(filePath))
{
    if (File.ReadAllText(filePath).Trim().Length == 0)
    {
        File.WriteAllText(filePath, "[]");
    }
}

var messages = JsonSerializer.Deserialize<List<Message>>(File.ReadAllText(filePath)) ?? new List<Message>();



// ???????????? ???????? ??????? HTTP-???????

app.MapGet("/", async (HttpContext context) =>
{
    var html = @"
<!DOCTYPE html>
    <html>
    <head>
        <title>You</title>
    </head>
    <body>
        <h1>Welcome</h1>
         <h2>John Doe</h2>
         <p>From: City, Country</p>
         <p>Age: 30</p>
         <a href=""/message"">Send Message</a>

    </body>
    </html>
";
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(html);
});

app.MapGet("/message", async (HttpContext context) =>
{
    //context.Response.ContentType = "application/json; charset=utf-8";
    //await context.Response.WriteAsJsonAsync(products);

    var pageTeplate = @"
<!DOCTYPE html>
    <html>
    <head>
        <title>Send Message</title>
    </head>
    <body>
        
    <form method=""post"" action=""/message/add"">
            <label for=""name"">Name:</label>
            <input type=""text"" id=""name"" name=""name"" required><br><br>
            <label for=""email"">Email:</label>
            <input type=""email"" id=""email"" name=""email"" required><br><br>
            <label for=""message"">Message:</label>
            <textarea id=""description"" name=""description"" required></textarea><br><br>
            <input type=""submit"" value=""Send Message"">
        </form>
        {body}
    </body>
    </html>
";

    var tableRows = "";
    messages.ForEach(p =>
    {
        tableRows += $"<tr><td>{p.Name}</td><td>{p.Email}</td><td>{p.MessageText}</td></tr>";
    });

    var html = pageTeplate.Replace("{body}", $"<table><tr><th>Name</th><th>Email</th><th>Message</th></tr>{tableRows}</table>");
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(html);
});

app.MapPost("/message/add", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();
    var name = form["name"].ToString();
    var email = form["email"].ToString();
    var description = form["description"].ToString();
    var newProduct = new Message
    {
        Id = messages.Count + 1,
        Name = name,
        Email = email,
        MessageText = description
    };
    messages.Add(newProduct);
    File.WriteAllText(filePath, JsonSerializer.Serialize(messages));
    context.Response.Redirect("/message");
});



app.Run();