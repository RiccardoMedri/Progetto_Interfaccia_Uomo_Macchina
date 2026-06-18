using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Medri.Web.Infrastructure
{
    internal static class PendingClientRequestSession
    {
        private const string SessionKey = "Medri.PendingClientRequests";

        public static void Add(ISession session, Guid leadId)
        {
            if (leadId == Guid.Empty)
            {
                return;
            }

            var leadIds = Read(session).ToList();
            if (!leadIds.Contains(leadId))
            {
                leadIds.Add(leadId);
                session.SetString(SessionKey, System.Text.Json.JsonSerializer.Serialize(leadIds));
            }
        }

        public static IReadOnlyList<Guid> Read(ISession session)
        {
            var value = session.GetString(SessionKey);
            if (string.IsNullOrWhiteSpace(value))
            {
                return Array.Empty<Guid>();
            }

            try
            {
                var leadIds = System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(value);
                return leadIds ?? (IReadOnlyList<Guid>)Array.Empty<Guid>();
            }
            catch (JsonException)
            {
                return Array.Empty<Guid>();
            }
        }

        public static void Clear(ISession session) => session.Remove(SessionKey);
    }
}
