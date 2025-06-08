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
        AnsiConsole.MarkupLine($"[green]Connected to Redis at[/] [yellow]{redisConn}[/]");

        while (true)
        {
            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\nChoose an [green]operation[/]:")
                    .AddChoices(new[] { "Add (set)", "Find (get)", "Delete", "Exit" }));

            if (action == "Exit")
            {
                AnsiConsole.MarkupLine("[grey]Goodbye![/]");
                break;
            }

            var key = AnsiConsole.Ask<string>("[blue]Enter key[/]:");

            switch (action)
            {
                case "Add (set)":
                    var value = AnsiConsole.Prompt(
                        new TextPrompt<string>("[blue]Enter value[/]:")
                            .PromptStyle("green"));
                    db.StringSet(key, value);
                    AnsiConsole.MarkupLine($"[green]Saved[/] key=[yellow]{key}[/] value=[yellow]{value}[/]");
                    break;

                case "Find (get)":
                    var result = db.StringGet(key);
                    if (result.IsNull)
                        AnsiConsole.MarkupLine($"[red]Key [yellow]{key}[/] not found[/]");
                    else
                        AnsiConsole.MarkupLine($"[green]Value for[/] [yellow]{key}[/]: [blue]{result}[/]");
                    break;

                case "Delete":
                    var removed = db.KeyDelete(key);
                    if (removed)
                        AnsiConsole.MarkupLine($"[green]Deleted[/] key=[yellow]{key}[/]");
                    else
                        AnsiConsole.MarkupLine($"[red]Key [yellow]{key}[/] not found[/]");
                    break;
            }
        }
    }
}
