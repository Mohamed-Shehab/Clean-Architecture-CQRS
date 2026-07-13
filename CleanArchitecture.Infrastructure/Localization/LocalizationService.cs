using CleanArchitecture.Application.Common.Services.Localization;
using System.Globalization;

namespace CleanArchitecture.Infrastructure.Localization
{
    public sealed class LocalizationService : ILocalizationService
    {
        public bool IsArabic => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar";

        public string GetLocalized(string arabic, string english)
        {
            return IsArabic ? arabic : english;
        }
    }
}
