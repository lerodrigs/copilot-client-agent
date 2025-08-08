using System;

namespace copilot_client_services.interfaces.services;

public interface IMyCopilotClientService
{
    Task<object> StartConversation(CancellationToken cancellationToken); 
}
