using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProOnePal.Models
{
    public class Tournament
    {
        public Tournament()
        {
            maxGames = 4;
            maxStages = 5;
            maxTeams = 16;
            enteredTeams    = new List<Team>();
            fixtures        = new List<Fixture>();
            results         = new List<Result>();
            stages          = new Dictionary<string, List<Team>>();
        }
        
        public int id { get; set; }
        [Range(0, 5)]
        public int maxGames { get; set; }
        #region
        [Range(0, 16)]
        public int maxTeams { get; set; }
        [Range(0,5)]
        public int maxStages { get; set; }
        #endregion
        [Required]
        [DisplayName("Tournament Name")]
        public string name { get; set; }
        public List<Team> enteredTeams { get; set; }
        //fixtures only matching the tournament name
        public List<Fixture> fixtures { get; set; }
        // list of results per stage
        public List<Result> results { get; set; }
        public Dictionary<string,List<Team>> stages { get; set; }
    }
}