using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MovieAPI.Models;

namespace MovieAPI.Controllers
{
    /// <summary>
    /// 电影信息控制器
    /// </summary>
    [Filters.AuthorizeFilter]
    public class MovieController : ApiController
    {
        MovieEntities dc = new MovieEntities();
        /// <summary>
        /// 最新上映电影
        /// </summary>
        /// <returns></returns>
        [Route("api/Movie/New")]
        [AllowAnonymous]
        public IHttpActionResult GetNewMovie()
        {
            IEnumerable<Movie> list = dc.Movie.OrderByDescending(c => c.ReleaseTime);
            return Ok(list);

        }
        /// <summary>
        /// 电影详情
        /// </summary>
        /// <param name="MovieId"></param>
        /// <returns></returns>
        [Route("api/Movie/Info")]
        [AllowAnonymous]
        public IHttpActionResult GetMovieDetails(int MovieId)
        {
            IEnumerable<Movie> list = dc.Movie.Where(c=>c.MovieID==MovieId);
            if (list.Count()>0)
            {
                return Ok(list.FirstOrDefault());
            }
            else
            {
                return NotFound();
            }
        }/// <summary>
         /// 电影场次信息
         /// </summary>
         /// <param name="MovieId">电影id</param>
         /// <returns></returns>
        [Route("api/Movie/Shows")]
        [AllowAnonymous]
        public IHttpActionResult GetMovieTime(int MovieId)
        {
            IEnumerable<MovieTime> list = dc.MovieTime.Where(c => c.MovieID == MovieId).OrderBy(m=>m.Time);
            if (list.Count()>0)
            {
                return Ok(list);
            }
            else
            {
                return NotFound();
            }


        }
        /// <summary>
        /// 电影场次详情
        /// </summary>
        /// <param name="timeid">电影播放时间Id</param>
        /// <returns></returns>
        
        [Route("api/Movie/ShowsDetail")]
        [AllowAnonymous]
        public IHttpActionResult GetTimeForId(int timeid)
        {
            IEnumerable<MovieTime> list = dc.MovieTime.Where(c => c.TimeID == timeid).OrderBy(m=>m.Time);
            if (list.Count() > 0)
            {
                return Ok(list.FirstOrDefault());
            }
            else
            {
                return NotFound();
            }
        }
        /// <summary>
        /// 当天电影场次已选座位
        /// </summary>
        /// <param name="timeid">时间id</param>
        /// <returns></returns>
        [Route("api/Movie/Seat")]
        public IHttpActionResult GetMovieTimeSeat(int timeid)
        {
            IEnumerable<OrderDetails> list = dc.OrderDetails.Where(c => c.TimeID == timeid /*&& c.Day == DateTime.Now*/);
            if (list.Count() > 0)
            {
                return Ok(list);
            }
            else
            {
                return NotFound();
            }

        }
        [Route("api/Movie/Seats")]
        [AllowAnonymous]
        public IHttpActionResult GetMovieTimeSeats(int timeid)
        {
            IEnumerable<OrderDetails> list = dc.OrderDetails.Where(c => c.TimeID == timeid /*&& c.Day == DateTime.Now*/);
            if (list.Count() > 0)
            {
                return Ok(list);
            }
            else
            {
                return NotFound();
            }

        }
        /// <summary>
        ///返回http200接口执行成功代码，无消息内容
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("api/Movie/Seat")]
        public string Options()
        {
            //返回http200接口执行成功代码，无消息内容
            return null;
        }
    }
}
