using System.Globalization;

namespace Medri.Web.Features.Property
{
    internal static class PropertyFormatting
    {
        public static string FormatPrice(decimal price, string contract)
        {
            var euros = string.Format(CultureInfo.GetCultureInfo("it-IT"), "{0:N0}", price);
            return contract == "Affitto" ? $"EUR {euros} / mese" : $"EUR {euros}";
        }

        public static string DisplayOrFallback(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }
    }
}
