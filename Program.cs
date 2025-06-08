using System;
using Spectre.Console;
using StackExchange.Redis;

class Program
{
    static void Main(string[] args)
    {
        var redisConn = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "localhost:6379";
        IConnectionMultiplexer mux;
        try
        {
            mux = ConnectionMultiplexer.Connect(redisConn);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error connecting to Redis[/]: {ex.Message}");
            return;
        }

        var db = mux.GetDatabase();
        var endpoint = mux.GetEndPoints()[0];
        var server = mux.GetServer(endpoint);

        AnsiConsole.MarkupLine($"[green]Connected to Redis at[/] [yellow]{redisConn}[/]");

        while (true)
        {
            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\nChoose an [green]operation[/]:")
                    .AddChoices(new[] {
                        "Show All",
                        "Add (set)",
                        "Find (get)",
                        "Delete",
                        "Remove All",
                        "Clear Console",
                        "Exit"
                    }));

            if (action == "Exit")
            {
                AnsiConsole.MarkupLine("[grey]Goodbye![/]");
                break;
            }

            switch (action)
            {
                case "Show All":
                    var table = new Table();
                    table.AddColumn("Key");
                    table.AddColumn("Value");
                    foreach (var key in server.Keys())
                    {
                        var val = db.StringGet(key);
                        table.AddRow(key, val.IsNull ? "<null>" : val);
                    }
                    AnsiConsole.Write(table);
                    break;

                case "Add (set)":
                    var keyToSet = AnsiConsole.Ask<string>("[blue]Enter key[/]:");
                    var valueToSet = AnsiConsole.Prompt(
                        new TextPrompt<string>("[blue]Enter value[/]:")
                            .PromptStyle("green"));
                    db.StringSet(keyToSet, valueToSet);
                    AnsiConsole.MarkupLine($"[green]Saved[/] key=[yellow]{keyToSet}[/] value=[yellow]{valueToSet}[/]");
                    break;

                case "Find (get)":
                    var keyToGet = AnsiConsole.Ask<string>("[blue]Enter key[/]:");
                    var result = db.StringGet(keyToGet);
                    if (result.IsNull)
                        AnsiConsole.MarkupLine($"[red]Key [yellow]{keyToGet}[/] not found[/]");
                    else
                        AnsiConsole.MarkupLine($"[green]Value for[/] [yellow]{keyToGet}[/]: [blue]{result}[/]");
                    break;

                case "Delete":
                    var keyToDelete = AnsiConsole.Ask<string>("[blue]Enter key[/]:");
                    var removed = db.KeyDelete(keyToDelete);
                    if (removed)
                        AnsiConsole.MarkupLine($"[green]Deleted[/] key=[yellow]{keyToDelete}[/]");
                    else
                        AnsiConsole.MarkupLine($"[red]Key [yellow]{keyToDelete}[/] not found[/]");
                    break;

                case "Remove All":
                    if (AnsiConsole.Confirm("Are you sure you want to remove *all* keys?"))
                    {
                        foreach (var key in server.Keys())
                        {
                            db.KeyDelete(key);
                        }
                        AnsiConsole.MarkupLine("[red]All keys deleted[/]");
                    }
                    break;

                case "Clear Console":
                    Console.Clear();
                    AnsiConsole.MarkupLine($"[green]Screen cleared at {DateTime.Now:T}[/]");
                    break;
            }
        }
    }
}
