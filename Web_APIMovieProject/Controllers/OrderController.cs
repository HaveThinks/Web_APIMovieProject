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
    /// 订单信息
    /// </summary>
    [Filters.AuthorizeFilter]
    public class OrderController : ApiController
    {

        MovieEntities dc = new MovieEntities();
        /// <summary>
        /// 获取所有订单信息
        /// </summary>
        /// <returns></returns>
        [Route("api/Orders")]
        public IHttpActionResult GetOrders()
        {
            //读取接口调用授权信息
            var Userid = System.Threading.Thread.CurrentPrincipal.Identity.Name;
            //查找用户登录信息
            IEnumerable<Orders> list = dc.Orders.Where(m=>m.UserID==int.Parse(Userid)).OrderByDescending(c=>c.ReserveTime);
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
    /// 下订单
    /// </summary>
    /// <param name="model">场次座位</param>
    /// <returns></returns>
        [Route("api/Orders")]
        public IHttpActionResult PostOrder([FromBody]OrderModel model)
        {
            //输入数据验证
            if (IsModelError(model))
            {
                return BadRequest();
            }
            //判断座位是否占用
            if (IsSeatOccupy(model))
            {
                return StatusCode(HttpStatusCode.Conflict);
            }
            //读取接口调用授权信息
            //var userid = 24;
            var userid = int.Parse(System.Threading.Thread.CurrentPrincipal.Identity.Name);
            //生成授权码

            //计算总价
            decimal? price = Getprice(model.TimeID, model.SeatList.Count);
            //下订单
            int orderid = SaveOrder(model,userid,price);
            //保存订单明细
            SaveOrderDetails(model,orderid);
            return Ok();
        }
        /// <summary>
        /// 保存订单明细
        /// </summary>
        /// <param name="model"></param>
        /// <param name="orderID"></param>
        private void SaveOrderDetails(OrderModel model, int orderID)
        {
            List<OrderDetails> list = new List<OrderDetails>();
            for (int i = 0; i < model.SeatList.Count; i++)
            {
                list.Add(new OrderDetails
                {
                    Day = DateTime.Now,
                    OrderID = orderID,
                    TimeID = model.TimeID,
                    Col = model.SeatList[i].Col,
                    Row = model.SeatList[i].Row
                });
            }
            //保存提交信息
            dc.OrderDetails.Add(list.FirstOrDefault());
            dc.SaveChanges();
        }
        /// <summary>
        /// 下订单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userid"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        private int SaveOrder(OrderModel model,int userid,decimal? price)
        {
            //获取电影场次信息
            MovieTime time = dc.MovieTime.FirstOrDefault(m => m.TimeID == model.TimeID);
            //获取电影信息
            Movie movie = dc.Movie.FirstOrDefault(m => m.MovieID == time.MovieID);
            //循环保存座位信息
            string seat = "";
            foreach (var item in model.SeatList)
            {
                seat += item.Row + "排" + item.Col + "座";
            }
            //拼接完整电影播放时间
            var dd = DateTime.Now.ToShortDateString() + " " + time.Time.Value.Hours + ":" + time.Time.Value.Minutes + ":00";
            DateTime date = Convert.ToDateTime(dd);
            //创建对象保存订单
            Orders order = new Orders
            {
                OrderDate = DateTime.Today,
                Price = price,
                TimeID = model.TimeID,
                UserID = userid,
                MovieName = movie.Name,
                ImgUrl = movie.ImgUrl,
                Status = "已完成",
                Total = model.SeatList.Count,
                ReserveTime = date,
                Seat = seat,
                Room = time.Room
            };
            dc.Orders.Add(order);
            dc.SaveChanges();
            return order.OrderID;
        }
        /// <summary>
        /// 计算总价
        /// </summary>
        /// <param name="timeid"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private decimal? Getprice(int timeid, int count)
        {
            //场次信息得到票价
            MovieTime movieTime = dc.MovieTime.FirstOrDefault(m => m.TimeID == timeid);
            //返回总价格
            return movieTime.Price * count;
        }
        /// <summary>
        /// 输入数据验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsModelError(OrderModel model)
        {
            //判断time编号是否存在
            return dc.MovieTime.Where(m => m.TimeID == model.TimeID).Count() == 0;
        }
        /// <summary>
        /// 判断座位是否占用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsSeatOccupy(OrderModel model)
        {
            for (int i = 0; i < model.SeatList.Count; i++)
            {
                var item = model.SeatList[i];
                //判断座位是否重复
                for (int j = i; j < model.SeatList.Count-1; j++)
                {
                    var item2 = model.SeatList[j + 1];
                    if (item.Col==item2.Col&&item.Row==item2.Row)
                    {
                        return true;
                    }
                    //判断座位是否占用
                    var count = dc.OrderDetails.Count(m => m.Day == DateTime.Now && m.TimeID == model.TimeID&&m.Col==item.Col&&m.Row==item.Col);
                    if (count>0)
                    {
                        return true;
                    }
                }
            }
            return false;
            
        }
        [AllowAnonymous]
        [Route("api/Orders")]
        public string Options()
        {

            return null;
        }
    }
}
