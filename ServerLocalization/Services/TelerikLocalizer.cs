using ServerLocalization.Resources;
using Telerik.Blazor.Services;


namespace ServerLocalization.Services
{
	public class TelerikLocalizer : ITelerikStringLocalizer
	{
        public string this[string name]
        {
            get
            {
                return GetStringFromResource(name);
            }
        }

        public string GetStringFromResource(string key)
        {
            return TelerikMessages.ResourceManager.GetString(key, TelerikMessages.Culture) ?? key;
        }
    }
}
