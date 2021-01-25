using Microsoft.AspNetCore.Mvc;

namespace WebSPA.Utils
{
    public class JavaScriptResult : ContentResult
    {
        public JavaScriptResult(string script)
        {
            Content = script;
            ContentType = "application/javascript";
        }
    }
}
