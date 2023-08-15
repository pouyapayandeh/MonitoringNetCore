using Microsoft.AspNetCore.Mvc.Controllers;
using MonitoringNetCore.Services;

namespace Monitoring.Common.Middleware;

public class LicenseMiddleware
{
    private readonly RequestDelegate _next;

    public LicenseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context,LicenseService licenseService)
    {
        var controllerActionDescriptor = context
            .GetEndpoint()
            .Metadata
            .GetMetadata<ControllerActionDescriptor>();

        if (controllerActionDescriptor != null)
        {
            var controllerName = controllerActionDescriptor.ControllerName;


            // await _next(httpContext);
            // if specific condition does not meet
            if (await licenseService.IsLicensed() || controllerName == "License")
            {
                // await _next.Invoke(context);
            }
            else
            {
                context.Response.Redirect("/License/Create");
            }
        }

        await _next.Invoke(context);

    }
}