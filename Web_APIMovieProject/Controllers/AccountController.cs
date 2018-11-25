using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Web_APIMovieProject.Models;

namespace Web_APIMovieProject.Controllers
{
    /// <summary>
    /// 登录信息控制器
    /// </summary>
    [Filters.AuthorizeFilter]
    public class AccountController : ApiController
    {


       
        
        MovieEntities dc = new MovieEntities();
        /// <summary>
        /// 注册用户   
        /// </summary>
        /// <param name="u">用户对象</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("api/Register")]
        public IHttpActionResult Register([FromBody]Users u)
        {
           
            if (u!=null)
            {
              var list= dc.Users.Where(c => c.UserName == u.UserName);
                if (list.Count()>0)
                {
                    return StatusCode(HttpStatusCode.Conflict);
                }
                else
                {
                    dc.Users.Add(u);
                    dc.SaveChanges();
                    return Ok();
                }
            }
            else
            {
                return BadRequest();

            }

        }
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="u">用户对象</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("api/Logion")]
        public IHttpActionResult Logion([FromBody]Users u)
        {
            if (u!=null)
            {
                var list = dc.Users.Where(c => c.UserName == u.UserName&&c.Password==u.Password);
                if (list.Count()==0)
                {
                    return StatusCode(HttpStatusCode.Unauthorized);
                }
                else
                {
                    Users user = list.First();
                    //删除Token
                    DeleteToken(user.UserID);
                    //随机生成token
                    Guid g = Guid.NewGuid();
                    string token = g.ToString().Replace("-","");
                    Tokens t = new Tokens
                    {
                        Token = token,
                        ExpireDate = DateTime.Now.AddDays(3),
                        UserID=user.UserID
                    
                    };
                    dc.Tokens.Add(t);
                    dc.SaveChanges();
                    return Ok(token);
                }
            }
            else
            {
                return BadRequest();
            }

        }
        /// <summary>
        /// 删除token
        /// </summary>
        /// <param name="UserId"></param>
        private static void DeleteToken(int UserId)
        {
            MovieEntities dc = new MovieEntities();
            var list = dc.Tokens.Where(c => c.UserID == UserId).FirstOrDefault();
            dc.Tokens.Remove(list);
            dc.SaveChanges();
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>    
        [Route("api/User")]
        public IHttpActionResult UserMs()
        {
            int userid = int.Parse(System.Threading.Thread.CurrentPrincipal.Identity.Name);
            Users u = dc.Users.Where(c=>c.UserID==userid).FirstOrDefault();
            u.Password = "******";
            return Ok(u);
        }
        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [Route("api/Logout")]
        public IHttpActionResult Logout()
        {
            int userid = int.Parse(System.Threading.Thread.CurrentPrincipal.Identity.Name);
            DeleteToken(userid);
            return Ok();

        }
    }
}
