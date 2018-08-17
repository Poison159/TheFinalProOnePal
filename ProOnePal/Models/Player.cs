using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace ProOnePal.Models
{
    public class Player
    {
        public Player()
        {
            imgPath = "~/Content/user.png";
            tournamentStats = new List<PlayerTournamentStat>();
        }

        public int Id { get; set; }
        public int teamId { get; set; }
        public virtual Team team { get; set; }
        [Required]
        [DisplayName("name")]
        public string name { get; set; }
        [Required]
        [DisplayName("age")]
        public int age { get; set; }
        [Required]
        [DisplayName("position")]
        public string position { get; set; }
        public List<PlayerTournamentStat> tournamentStats { get; set; }
        [Required]
        [DisplayName("path")]
        public string imgPath { get; set; }
        [NotMapped]
        public HttpPostedFileBase imageUpload { get; set; }
        
    }
}