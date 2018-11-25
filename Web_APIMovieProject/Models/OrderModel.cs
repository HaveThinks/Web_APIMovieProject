using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_APIMovieProject.Models
{
    public class OrderModel
    {
        public int TimeID { get; set; }

        public List<SeatModel> SeatList { get; set; }
    }

    public class SeatModel
    {

        public int Row { get; set; }
        public int Col { get; set; }
    }
}