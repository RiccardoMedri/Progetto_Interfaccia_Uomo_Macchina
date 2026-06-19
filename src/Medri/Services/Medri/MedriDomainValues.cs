namespace Medri.Services.Medri
{
    public static class LeadWorkflowStatuses
    {
        public const string New = "New";
        public const string InContact = "InContact";
        public const string Qualified = "Qualified";
        public const string Archived = "Archived";
    }

    public static class RequestStatuses
    {
        public const string New = "New";
        public const string Updating = "Updating";
        public const string InMatching = "InMatching";
        public const string Archived = "Archived";
    }

    public static class PropertyPublicationStatuses
    {
        public const string Incomplete = "Incomplete";
        public const string Ready = "Ready";
        public const string Published = "Published";
        public const string NeedsUpdate = "NeedsUpdate";
        public const string Reserved = "Reserved";
        public const string Archived = "Archived";
    }

    public static class AppointmentStatuses
    {
        public const string Draft = "Draft";
        public const string Submitted = "Submitted";
        public const string Scheduled = "Scheduled";
        public const string InMatching = "InMatching";
        public const string Received = "Received";
    }

    public static class RequestTypes
    {
        public const string Buy = "Buy";
        public const string Rent = "Rent";
        public const string Sell = "Sell";
        public const string RentOut = "RentOut";
        public const string Valuation = "Valuation";
        public const string PropertyContact = "PropertyContact";
        public const string GeneralContact = "GeneralContact";
        public const string Seed = "Seed";
    }

    public static class AgencyStaffRoles
    {
        public const string Advisor = "Advisor";
        public const string Manager = "Responsabile agenzia";
        public const string Operator = "Operatore";
    }
}
