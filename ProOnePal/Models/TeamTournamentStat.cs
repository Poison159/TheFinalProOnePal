using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProOnePal.Models
{
    public class TeamTournamentStat
    {
        
        public int id { get; set; }
        public int teamId { get; set; }
        public Team team { get; set; }
        public string tournamentName { get; set; }
        [Required]
        [Display(Name = "GP")]
        public int gamesPlayed { get; set; }
        [Required]
        [Display(Name = "GW")]
        public int gamesWon { get; set; }
        [Display(Name = "GD")]
        public int gamesDrawn { get; set; }
        [Required]
        [Display(Name = "GL")]
        public int gamesLost { get; set; }
        [Display(Name = "P")]
        public int points { get; set; }
        [Display(Name = "GD")]
        public int goalDiff { get; set; }
        [Required]
        [Display(Name = "GF")]
        public int goalsFor { get; set; }
        [Required]
        [Display(Name = "GA")]
        public int goalsAgainst { get; set; }

        [DisplayName("Group")]
        public string group { get; set; }
        public void getPoints()
        {
            points = (gamesWon * 3) + gamesDrawn;
        }
        public void getGamesDrawn()
        {
            gamesDrawn = gamesPlayed - (gamesWon + gamesLost);
        }
        public void getGoalDiff()
        {
            goalDiff = goalsFor - goalsAgainst;
        }
    }
}