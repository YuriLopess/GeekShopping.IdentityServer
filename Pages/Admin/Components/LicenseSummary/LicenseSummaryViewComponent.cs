using Duende.IdentityServer.Licensing;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.IdentityServer.ViewComponents;

public class LicenseSummaryViewComponent(LicenseUsageSummary licenseUsageSummary) : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync() => Task.FromResult<IViewComponentResult>(View(licenseUsageSummary));
}
