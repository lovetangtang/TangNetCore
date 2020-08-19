using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PermissionFilter :Attribute , IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            IIdentity user = context.HttpContext.User.Identity;
            if (!user.IsAuthenticated)
            {
                //跳转到登录页
                context.Result = new LocalRedirectResult("./test");
                return Task.CompletedTask;
            }

            //根据当前用户，判断当前访问的action，没有权限时返回403错误
            context.Result = new ForbidResult();

            return Task.CompletedTask;
        }
    }

}
