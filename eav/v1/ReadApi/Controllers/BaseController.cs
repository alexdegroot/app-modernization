using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReadApi.Controllers
{
    public class BaseController : Controller
    {
        protected const string TenantIdHeaderName = "Raet-Identity-Tenant-Id";
        protected string GetTenantIdFromHeaders()
        {
            string tenantHeader = this.HttpContext.Request.Headers[TenantIdHeaderName];
            if (string.IsNullOrEmpty(tenantHeader))
            {
                throw new ArgumentException("Tenant ID header value is missing.");
            }
            return tenantHeader;
        }
        protected string ToJson(object value)
        {
            var json = JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                IgnoreNullValues = true
            });
            return json;
        }
    }
}
