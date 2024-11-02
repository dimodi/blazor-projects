namespace WasmAppMinApi.Shared
{
    public class AntiForgeryData
    {
        public string CookieName { get; set; }
        public string CookieToken { get; set; }
        public string FormFieldName { get; set; }
        public string HeaderName { get; set; }
        public string RequestToken { get; set; }

        public AntiForgeryData(string cookieName, string cookieToken, string formFieldName, string headerName, string requestToken)
        {
            CookieName = cookieName;
            CookieToken = cookieToken;
            FormFieldName = formFieldName;
            HeaderName = headerName;
            RequestToken = requestToken;
        }
    }
}
