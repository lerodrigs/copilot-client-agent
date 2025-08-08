using System;
using copilot_client_services.interfaces.services;
using copilot_client_shared.Extensions;
using Microsoft.Agents.CopilotStudio.Client;
using Microsoft.Agents.Core.Models;

namespace copilot_client_services.services;

public class MyCopilotClientService(CopilotClient copilotClient) : IMyCopilotClientService
{
    public async Task<object> StartConversation(CancellationToken cancellationToken)
    {
        try
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            await foreach (var activity in copilotClient.StartConversationAsync(emitStartConversationEvent: true, cancellationToken: cancellationToken))
            {
                System.Diagnostics.Trace.WriteLine($">>>>MessageLoop Duration: {sw.Elapsed.ToDurationString()}");
                PrintActivity(activity);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                Console.Write("\nuser> ");
                string question = Console.ReadLine()!; // Get user input from the console to send. 
                Console.Write("\nagent> ");
                // Send the user input to the Copilot Studio agent and await the response.
                // In this case we are not sending a conversation ID, as the agent is already connected by "StartConversationAsync", a conversation ID is persisted by the underlying client. 
                await foreach (Activity act in copilotClient.AskQuestionAsync(question, null, cancellationToken))
                {   
                    System.Diagnostics.Trace.WriteLine($">>>>MessageLoop Duration: {sw.Elapsed.ToDurationString()}");
                    // for each response,  report to the UX
                    PrintActivity(act);
                }
            }
            return new();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    private void PrintActivity(IActivity act)
    {
        switch (act.Type)
        {
            case "message":
                if (act.TextFormat == "markdown")
                {

                    Console.WriteLine(act.Text);
                    if (act.SuggestedActions?.Actions.Count > 0)
                    {
                        Console.WriteLine("Suggested actions:\n");
                        act.SuggestedActions.Actions.ToList().ForEach(action => Console.WriteLine("\t" + action.Text));
                    }
                }
                else
                {
                    Console.Write($"\n{act.Text}\n");
                }
                break;
            case "typing":
                Console.Write(".");
                break;
            case "event":
                Console.Write("+");
                break;
            default:
                Console.Write($"[{act.Type}]");
                break;
        }
    }
}
