using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProOnePal.Models
{
    public class Team
    {
        public Team()
        {
            imgPath = "~/Content/user.png";
            tournamentStats = new List<TeamTournamentStat>();
            players = new List<Player>();
        }
        public int id { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string name { get; set; }
        [Required]
        [Display(Name = "kasi")]
        public string kasi { get; set; } // selected list
        [Display(Name = "players")]
        public List<Player> players { get; set; }
        public List<TeamTournamentStat> tournamentStats { get; set; }
        [Display(Name = "TeamCrest")]
        public string imgPath { get; set; }
        [NotMapped]
        public HttpPostedFileBase imageUpload { get; set; }
       
    }
}