using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProOnePal.Models
{
    public class PlayerResultStat
    {
        public int id { get; set; }
        public int playerId { get; set; }
        public int resultId { get; set; }
        public Player player { get; set; }
        public Result result { get; set; }
        public int goals { get; set; }
    }
}