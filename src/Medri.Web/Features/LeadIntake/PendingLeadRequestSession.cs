using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using Medri.Services.Medri.Application;

namespace Medri.Web.Features.LeadIntake
{
    internal static class PendingLeadRequestSession
    {
        private const string SessionKey = "Medri.PendingLeadRequest";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static void Store(ISession session, BuyLeadRequestViewModel viewModel) =>
            StoreCore(session, "Buy", viewModel);

        public static void Store(ISession session, RentLeadRequestViewModel viewModel) =>
            StoreCore(session, "Rent", viewModel);

        public static void Store(ISession session, SellLeadRequestViewModel viewModel) =>
            StoreCore(session, "Sell", viewModel);

        public static void Store(ISession session, RentOutLeadRequestViewModel viewModel) =>
            StoreCore(session, "RentOut", viewModel);

        public static void Store(ISession session, ValuationRequestViewModel viewModel) =>
            StoreCore(session, "Valuation", viewModel);

        public static bool TryCreateRequest(ISession session, out LeadRequestDto request)
        {
            request = null;
            var value = session.GetString(SessionKey);
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            try
            {
                var pending = JsonSerializer.Deserialize<PendingLeadRequest>(value, JsonOptions);
                if (pending == null || string.IsNullOrWhiteSpace(pending.Payload))
                {
                    return false;
                }

                request = pending.RequestType switch
                {
                    "Buy" => CreateRequest<BuyLeadRequestViewModel>(pending.Payload, LeadIntakeMapper.Create),
                    "Rent" => CreateRequest<RentLeadRequestViewModel>(pending.Payload, LeadIntakeMapper.Create),
                    "Sell" => CreateRequest<SellLeadRequestViewModel>(pending.Payload, LeadIntakeMapper.Create),
                    "RentOut" => CreateRequest<RentOutLeadRequestViewModel>(pending.Payload, LeadIntakeMapper.Create),
                    "Valuation" => CreateRequest<ValuationRequestViewModel>(pending.Payload, LeadIntakeMapper.Create),
                    _ => null
                };

                return request != null;
            }
            catch (JsonException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }

        public static void Clear(ISession session) => session.Remove(SessionKey);

        private static LeadRequestDto CreateRequest<T>(
            string payload,
            Func<T, LeadRequestDto> createRequest)
            where T : class
        {
            var viewModel = JsonSerializer.Deserialize<T>(payload, JsonOptions);
            return viewModel == null ? null : createRequest(viewModel);
        }

        private static void StoreCore(ISession session, string requestType, object viewModel)
        {
            var pending = new PendingLeadRequest
            {
                RequestType = requestType,
                Payload = JsonSerializer.Serialize(viewModel, viewModel.GetType(), JsonOptions)
            };

            session.SetString(SessionKey, JsonSerializer.Serialize(pending, JsonOptions));
        }

        private sealed class PendingLeadRequest
        {
            public string RequestType { get; set; }

            public string Payload { get; set; }
        }
    }
}
