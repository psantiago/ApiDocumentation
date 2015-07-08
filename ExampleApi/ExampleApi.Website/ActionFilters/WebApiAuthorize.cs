using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Controllers;
using ExampleApi.Website.Models.v2;
using ExampleApi.Website.Services;
using Newtonsoft.Json;

namespace ExampleApi.Website.ActionFilters
{
    /// <summary>
    /// Checks the authorization of the given basic authentication.   
    /// </summary>
    /// <remarks>
    /// Always remember that Basic Authentication passes username and passwords
    /// from client to server in plain text, so make sure SSL is used with basic auth
    /// to encode the Authorization header on all requests (not just the login).
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class WebApiAuthorize : AuthorizeAttribute
    {
        public WebApiAuthorize(Permission readPermission, Permission writePermission)
        {
            ReadPermission = readPermission;
            WritePermission = writePermission;
        }

        private readonly Permission ReadPermission;
        private readonly Permission WritePermission;

        private const string WebApiIdentityIdentityKey = "WebApiIdentity";

        //please note, according to https://msdn.microsoft.com/en-us/library/system.web.mvc.authorizeattribute%28v=vs.118%29.aspx,
        //we should store state information in HttpContext.Items for thread safety, NOT as an instance field.
        private static WebApiIdentity Identity
        {
            get { return HttpContext.Current.Items[WebApiIdentityIdentityKey] as WebApiIdentity; }
            set { HttpContext.Current.Items[WebApiIdentityIdentityKey] = value; }
        }

        protected override bool IsAuthorized(HttpActionContext context)
        {
            if (context == null) throw new ArgumentException("HttpActionContext does not exist!");
            Identity = GetWebApiIdentity(context);
            if (Identity == null) return false;

            return Identity.Permissions.Contains(context.Request.Method == HttpMethod.Get ?
                ReadPermission :
                WritePermission);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext context)
        {
            if (Identity == null)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new RequestErrors(new Error("Invalid Token Provided", "Token"))))
                };

                return;
            }

            context.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(new RequestErrors(new Error("The provided token does not have access to perform this request", "Token"))))
            };
        }

        /// <summary>
        /// Gets the identity passed in the request headers. 
        /// Please note, this identity has not been validated to be authorized.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static WebApiIdentity GetWebApiIdentity(HttpActionContext context)
        {
            var authHeader = context.Request.Headers.Authorization;
            if (authHeader == null) return null;

            return WebApiIdentityService.Get(authHeader.Parameter);
        }
    }
}