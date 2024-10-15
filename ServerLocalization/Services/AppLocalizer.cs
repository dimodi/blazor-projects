using Microsoft.Extensions.Localization;
using ServerLocalization.Resources;

namespace ServerLocalization.Services
{
    // This class is required only if there is a service registration in Program.cs
    public class AppLocalizer : IStringLocalizer
    {
        public LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();

        LocalizedString IStringLocalizer.this[string name] =>
            new LocalizedString(name, Translations.ResourceManager.GetString(name, Translations.Culture) ?? string.Empty);

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }
    }
}
