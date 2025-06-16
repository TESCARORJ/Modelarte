namespace ByTescaro.ConstrutorApp.UI.Properties
{
    public static class ServiceProviderExtensions
    {
        public static HttpClient CreateHttpClientWithCookies(this IServiceProvider sp, string apiBaseUrl)
        {
            var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;

            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new System.Net.CookieContainer()
            };

            if (httpContext?.Request.Cookies != null)
            {
                foreach (var cookie in httpContext.Request.Cookies)
                {
                    handler.CookieContainer.Add(
                        new Uri(apiBaseUrl),
                        new System.Net.Cookie(cookie.Key, cookie.Value)
                    );
                }
            }

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(apiBaseUrl)
            };
        }
    }

}
