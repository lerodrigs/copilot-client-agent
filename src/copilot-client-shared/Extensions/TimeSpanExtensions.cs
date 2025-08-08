using System;

namespace copilot_client_shared.Extensions;

public static class TimeSpanExtensions
{
    public static string ToDurationString(this TimeSpan timeSpan)
    {
        return timeSpan.ToString(@"hh\:mm\:ss\.fff");
    }
}
