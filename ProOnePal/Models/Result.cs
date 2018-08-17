using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProOnePal.Models
{
    public class Result
    {
        public Result()
        {
            scorers = new List<PlayerResultStat>();
        }
        public int id { get; set; }
        [Required]
        [DisplayName("Fixture Name")]
        public int fixtureId { get; set; } // selected list from db with fixtureName prop
        public Fixture fixture { get; set; }
        [Required]
        public int homeGoals { get; set; }
        [Required]
        public int awayaGoals { get; set; }
        [DisplayName("scorers")]
        public List<PlayerResultStat> scorers { get; set; }
    }
}