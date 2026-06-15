namespace Medri.Services.Medri.Identity
{
    public static class UserRoles
    {
        public const string Client = "Client";
        public const string Admin = "Admin";
        public const string Operator = "Operator";
        public const string AdminArea = Admin + "," + Operator;
    }
}
