using System.Globalization;

namespace CleanArchitecture.Application.Common.Services.Localization
{
    public interface ILocalizationService
    {
        public bool IsArabic { get; }

        string GetLocalized(string arabic, string english);
    }
}
