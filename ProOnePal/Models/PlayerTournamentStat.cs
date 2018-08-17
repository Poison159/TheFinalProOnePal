using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProOnePal.Models
{
    public class PlayerTournamentStat
    {
        public int id { get; set; }
        public int playerId { get; set; }
        public Player player { get; set; }
        public string tournamentName { get; set; }
        [Required]
        [DisplayName("goals")]
        public int goals { get; set; }
        [Required]
        [DisplayName("games played")]
        public int gamesPlayed { get; set; }
    }
}