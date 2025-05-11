using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;
using Microsoft.Extensions.Options;
using MonkeySpeak.Backend.Core.Configurations;

namespace Pages
{
    public class IndexModel(IOptions<App> appInfo) : PageModel
    {
        private readonly App _appConfiguration = appInfo.Value;
        
        public string BackendVersion => _appConfiguration.BackendVersion;
        public string FrontendVersion => _appConfiguration.FrontendVersion + " " + _appConfiguration.FrontendAdditionalVersionText;
        public string AppName => _appConfiguration.Name;
    }
}