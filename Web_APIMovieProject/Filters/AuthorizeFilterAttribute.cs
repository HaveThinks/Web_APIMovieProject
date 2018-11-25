using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Threading;
using System.Security.Principal;
using Web_APIMovieProject.Models;

namespace Web_APIMovieProject.Filters
{
    public class AuthorizeFilterAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            //获取请求头部的授权信息
            var Auth = actionContext.Request.Headers.Authorization;
            //判断授权信息是否为空，以及是否使用basic授权方式。
            if (Auth!=null&&Auth.Scheme.ToLower()=="basic")
            {
                //Auth.Parameter;获取请求中的token
                var token = Auth.Parameter;
                //判断授权信息是否正确

                return checktoToken(token);
            }
            return false;
        }
        //链接数据库或缓存服务器，验证token是否有效
        private bool checktoToken(string token)
        {
            if (token!="")
            {
                MovieEntities db = new MovieEntities();
                //查找授权记录
                IEnumerable<Tokens> list = db.Tokens.Where(c=>c.Token==token);
                var list1 = db.Tokens.Where(c=>c.Token== "22");
                if (list.Count()>0)
                {
                    Tokens model = list.First();
                    //判断授权是否过期
                    if (model.ExpireDate>DateTime.Now)
                    {
                        //获取token对应用户编号
                        string userid = model.UserID.ToString();
                        //token有效，设置当前用户信息
                        Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(userid), null);
                        return true;
                    }
                   
                   
                    
                }
            }
            return false;


        }
    }
}