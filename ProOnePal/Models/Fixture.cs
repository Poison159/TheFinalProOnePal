using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProOnePal.Models
{
    public class Fixture
    {
        public Fixture()
        {
            Played = "No";
        }

        public int id { get; set; }
        [Required]
        [DisplayName("Tournament Name")]
        public int tournamentId { get; set; }
        public Tournament tournament { get; set; }
        [DisplayName("stage")]
        public string stage { get; set; } // selected list to choose
        [Required]
        [DisplayName("Home Team")]
        public string homeTeam { get; set; } // selected list
        [Required]
        [DisplayName("Away Team")]
        public string awayTeam { get; set; } // selected list
        [Required]
        [DisplayName("Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime date { get; set; }
        [Required]
        [DisplayName("Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:H:mm}", ApplyFormatInEditMode = true)]
        public DateTime time { get; set; }
        [Required]
        public string  pitch { get; set; }
        public string fixtureName { get; set; }
        public string Played { get; set; }
    }
}