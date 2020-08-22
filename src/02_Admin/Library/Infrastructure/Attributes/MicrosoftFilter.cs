using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Infrastructure.Attributes
{
    public class MicrosoftFilter : ILogEventFilter
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                    "ThreadId", Thread.CurrentThread.ManagedThreadId));
        }
        public bool IsEnabled(LogEvent logEvent)
        {
            if (!logEvent.Properties.TryGetValue("SourceContext", out var source))
            {
                return logEvent.Level >= LogEventLevel.Debug;
            }

            if (source.ToString().StartsWith("\"Microsoft"))
            {
                return logEvent.Level >= LogEventLevel.Warning;
            }

            return logEvent.Level >= LogEventLevel.Debug;
        }
    }
}
