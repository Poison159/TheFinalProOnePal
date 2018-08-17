using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProOnePal.Models;
using System.Data.Entity;

namespace ProOnePal.Models
{
    public class Helper
    {
        public static int Max { get; internal set; }

        public static TeamStats getAllTeamsStats(IEnumerable<Team> teams)
        {
            var teamStats = new TeamStats();

            foreach (var team in teams)
            {
                teamStats.wins      += getGames(team,"won");
                teamStats.losses    += getGames(team, "lost");
                teamStats.draws     += getGames(team, "drawn");
            }

            return teamStats;
        }

        internal static Dictionary<string, string> getImagePaths(ApplicationDbContext db)
        {
            var ret = new Dictionary<string, string>();
            foreach (var team in db.Teams.ToList())
                ret.Add(team.name,team.imgPath);
            return ret;
        }

        internal static void assignPlayersToScorers(List<PlayerResultStat> scorers, ApplicationDbContext db)
        {
            foreach (var resStat in scorers)
                resStat.player = db.Players.Find(resStat.playerId);
        }

        internal static void assignTournamentsToFixtures(ApplicationDbContext db)
        {
            foreach (var fixture in db.Fixtures.ToList())
            {
                var toun            = db.Tournaments.Find(fixture.tournamentId);
                fixture.tournament  = toun;
            }
            db.SaveChanges();
        }

        internal static void assignTournamentsToFixtures2(List<Result> results, ApplicationDbContext db)
        {
            foreach (var res in results)
            {
                var toun                = db.Tournaments.Find(res.fixture.tournamentId);
                res.fixture.tournament  = toun;   
            }
            db.SaveChanges();
        }

        public static void getStatsbyName(List<Team> teams, string tournamentName)
        {
            foreach (var team in teams)
            {
                foreach (var stat in team.tournamentStats.ToList().Where(x => x.tournamentName == tournamentName))
                {
                    if (stat.tournamentName == tournamentName)
                    {
                        team.tournamentStats.Insert(0,stat);
                        break;
                    }
                }
            }
        }

        internal static bool CheckIfPlayerInTeam(Player player,DbSet<Team> teams,DbSet<Player> players)
        {
            var team = teams.Find(player.teamId);
            team.players = players.Where(x => x.team.id == team.id).ToList();
            if (Helper.getPlayerNames(team.players).Contains(player.name))
                return true;
            return false;
        }

        internal static List<string> getPlayerNames(List<Player> list)
        {
            var playerNames = new List<string>();
            foreach (var item in list)
                playerNames.Add(item.name);
            return playerNames;
        }

        internal static List<Team> getenterdTeams(Tournament tournament,ApplicationDbContext db)
        {
            var list = new List<Team>();
            var filteredStats = db.teamTournamentStats.Where(x => x.tournamentName == tournament.name).ToList();
            foreach (var team in db.Teams)
            {
                foreach (var stat in filteredStats)
                {
                    if (stat.teamId == team.id)
                        list.Add(team);
                }
            }
            return list;
        }

        internal static List<string> getStages()
        {
            return new List<string>() {"Group Stages","Knock Out","Last_16","Q_Final","Final" };
        }

        internal static List<string> GetTournamentName(ApplicationDbContext db)
        {
            var tournaments = new List<string>();
            foreach (var tourn in db.Tournaments.ToList())
                    if (!tournaments.Contains(tourn.name))
                        tournaments.Add(tourn.name);
            db.SaveChanges();
            return tournaments;
        }

        public static int getGames(Team team, string kind)
        {
            int result = 0;
            if (kind == "won")
            {
                foreach (var stat in team.tournamentStats)
                {
                    result += stat.gamesWon;
                }
                return result;
            } else if (kind == "lost")
            {
                foreach (var stat in team.tournamentStats)
                    result += stat.gamesLost;
                return result;
            } else if (kind == "drawn") {
                foreach (var stat in team.tournamentStats)
                    result += stat.gamesLost;
                return result;
            }
            else 
                return result;
        }

        internal static bool IsInputValid(string homeGoals, string awayGoals,ref int goalsForHome,ref int goalsForAway)
        {
            if (!string.IsNullOrEmpty(homeGoals) && !string.IsNullOrEmpty(awayGoals))
            {
                if (int.TryParse(homeGoals, out goalsForHome) && int.TryParse(awayGoals, out goalsForAway))
                    return true;
                else
                    return false;
            }
            return false;
        }

        internal static string getStageFromTournament(Tournament tournament)
        {
            if (tournament.results.Count > 12 && tournament.results.Count <= 16)
                return "QF";
            if (tournament.results.Count > 16 && tournament.results.Count <= 18)
                return "SM";
            if (tournament.results.Count > 0 && tournament.results.Count <= 19)
                return "F";
            else
                return "";
        }
        public static int getGoalDiff(int gf, int ga)
        {
            return (gf - ga);
        }

        public static Ratio ReturnRatio(IEnumerable<Player> list)
        {
            var ecxel       = list.Where(x => x.position == "ST");
            var good        = list.Where(x => x.position == "LW" || x.position == "RW");
            var poor        = list.Where(x => x.position == "CAD" || x.position == "LB" || x.position == "RB");
            var fair        = list.Where(x => x.position == "MD" || x.position == "CAM");

            Ratio obj       = new Ratio();
            obj.skikers     = ecxel.Count();
            obj.wingers     = good.Count();
            obj.midfilders  = fair.Count();
            obj.defenders   = poor.Count();

            return obj;
        }

        internal static int RandomTeamsId(List<Team> list)
        {
            var rand = new Random();
            return list.OrderBy(x => rand.Next()).First().id;
        }

        internal static IEnumerable<int> getTeamIds(List<Team> list)
        {
            foreach (var item in list)
                yield return item.id;
        }

        internal static string RandomPosition()
        {
            Random rand     = new Random();
            var positions   = ReturnPositions();
            return positions.OrderBy(x => rand.Next()).First();
        }

        internal static List<string> getFixturesString(List<Fixture> list)
        {
            List<string> fixtureList = new List<string>();
            if (list != null) {
                foreach (var item in list)
                {
                    string temp = "";
                    temp = item.homeTeam + "," + item.awayTeam + "," + item.date.ToShortDateString() + "," + item.pitch;
                    fixtureList.Add(temp);
                }
            }
            return fixtureList;
        }

        internal static bool areTeamsInSameGroup(string homeTeam, string awayTeam,Tournament tourn, ApplicationDbContext db)
        {
            var homeStat = db.teamTournamentStats.First(x => x.team.name == homeTeam &&
            x.tournamentName == tourn.name);
            var awayStat = db.teamTournamentStats.First(x => x.team.name == awayTeam &&
            x.tournamentName == tourn.name);

            if (homeStat.group == awayStat.group)
                return true;
            else
                return false;
        }

        public static List<string> ReturnPositions()
        {
            List<string> positions = new List<string>()
            {
                "ST","DF","LW","RW","CAD","MD","RB","LB","GK"
            };
            return (positions);
        }

        public static List<string> getTeamNames(List<Team> teams)
        {
            List<string> teamNames = new List<string>();
            foreach (var team in teams)
            {
                teamNames.Add(team.name);
            }
            return teamNames;
        }

        internal static string createFixtureName(Fixture fixture)
        {
            return fixture.homeTeam + "," + fixture.awayTeam + "," + fixture.date;
        }

        private static IEnumerable<int> getIdList(List<Team> teams)
        {
            foreach (var team in teams)
            {
                yield return team.id;
            }
        }
        internal static void SaveData(ApplicationDbContext db, Result result)
        {
            var fixture     = db.Fixtures.Find(result.fixtureId);
            fixture.Played  = "Yes";
            db.SaveChanges();
            var tournament  = db.Tournaments.Find(fixture.tournamentId);
            assignStats(result, db, tournament.name, fixture);
        }

        public static void assignStats(Result result, ApplicationDbContext db,string tournamentName, Fixture fixture)  
        {
            List<Team> teamInFixture    = findTeams(result,db.Teams.ToList());
            Team homeTeam               = teamInFixture.ElementAt(0);
            Team awayTeam               = teamInFixture.ElementAt(1);

            // filter with teamId and tournamentName 
            TeamTournamentStat statForHomeTeamTobeModified = db.teamTournamentStats.ToList().First(x => x.teamId == homeTeam.id &&
                                                                x.tournamentName == tournamentName); 
            TeamTournamentStat statForAwayTeamTobeModified = db.teamTournamentStats.ToList().First(x => x.teamId == awayTeam.id &&
                                                                x.tournamentName == tournamentName);
            
            assignDefaultStats(statForHomeTeamTobeModified, statForAwayTeamTobeModified, result);
            
            Tournament tourn = db.Tournaments.ToList().First(x => x.name == tournamentName);
            tourn.results = db.Results.Where(x => x.fixtureId == result.fixtureId).ToList();

            if(tourn.results.Count > 12)
                Helper.ChangeTeamStatus(db.teamTournamentStats.ToList(),tourn);

            db.SaveChanges();
        }

        private static void ChangeTeamStatus(List<TeamTournamentStat> tournStats,Tournament tourn)
        {
            var letters = new List<string> { "A", "B", "C", "D", "E", "F", "G" };
            foreach (var letter in AssignGroups(letters,tourn))
            {
                var stats = tournStats.Where(x => x.group 
                == letter).ToList().OrderBy(a => a.points).Take(2);
                foreach (var stat in tournStats)
                    stat.group = getStageFromTournament(tourn);
            }
        }

        public static List<Team> getTeamsByTournamentName(ApplicationDbContext db, string tournamentname)
        {
            var retTeams = new List<Team>();
            
            foreach (var stat in db.teamTournamentStats.ToList())
            {
                if(stat.tournamentName == tournamentname)
                {
                    var team = db.Teams.Find(stat.teamId);
                    retTeams.Add(team);
                }
            }
            return retTeams;
        }

        public static void assignDefaultStats(TeamTournamentStat homeStats, TeamTournamentStat awayStats, Result result)
        {
            homeStats.gamesPlayed++;
            homeStats.goalsFor      += result.homeGoals;
            homeStats.goalsAgainst  += result.awayaGoals;
            homeStats.getGoalDiff();

            awayStats.gamesPlayed++;
            awayStats.goalsFor      += result.awayaGoals;
            awayStats.goalsAgainst  += result.homeGoals;
            awayStats.getGoalDiff();
            PopulateGamesWon(homeStats, awayStats, result);
        }

        private static void PopulateGamesWon(TeamTournamentStat homeTeam, TeamTournamentStat awayTeam, Result result)
        {
            if (result.homeGoals > result.awayaGoals)
            {
                homeTeam.gamesWon++;
                homeTeam.points     += 3;
                awayTeam.gamesLost++;
            }
            else if (result.homeGoals < result.awayaGoals)
            {
                awayTeam.gamesWon++;
                awayTeam.points     += 3;
                homeTeam.gamesLost++;
            }
            else {
                homeTeam.gamesDrawn++;
                awayTeam.gamesDrawn++;
                homeTeam.points++;
                awayTeam.points++;
            }
        }

        private static List<Team> findTeams(Result result, List<Team> teams)
        {
            var teamsInFixture  = new List<Team>();
            string homeTeamName = result.fixture.fixtureName.Split(',').ElementAt(0);
            string awayTeamName = result.fixture.fixtureName.Split(',').ElementAt(1);

            Team awayTeam       = teams.First(x => x.name == awayTeamName);
            Team homeTeam       = teams.First(x => x.name == homeTeamName);

            teamsInFixture.Add(homeTeam);
            teamsInFixture.Add(awayTeam);
            return teamsInFixture;
        }

        private static Team getTeam(string teamName, List<Team> teams)
        {
            foreach (var item in teams)
            {
                if (item.name == teamName)
                    return item;
            }
            return null;
        }

        public static Fixture findFixture(string fitureName,List<Fixture> fixtures)
        {
            var array = fitureName.Split(',');
            foreach (var item in fixtures)
            {
                if (item.homeTeam == array[0] && item.awayTeam == array[1])
                {
                    return item;
                }
            }
            return null;
        }


        internal static IEnumerable<string> assignTeamsToGroups(Tournament tourn, Dictionary<string, List<Team>> groups)
        {
            var letters = new List<string> { "A", "B", "C", "D", "E", "F", "G" };
            var lettersToAssign = AssignGroups(letters, tourn);
            foreach (var letter in letters.Take(lettersToAssign.Count()))
            {
                groups.Add(letter, tourn.enteredTeams.Where(x =>
                x.tournamentStats.Where(a => a.group == letter) != null).ToList());
            }
            return lettersToAssign;
        }

        public static IEnumerable<string> AssignGroups(List<string> letters,Tournament tourn)
        {
            var teams = tourn.enteredTeams;
            IEnumerable<int> teamIdList = getIdList(teams);
            Random rand                 = new Random();
            var randIds                 = teamIdList.OrderBy(x => rand.Next()).ToArray();
            var lettersToAssign         = letters.Take(teams.Count / 4);
            if(!AllTeamsEneteredHaveGrops(teams))
                giveGroupsRandom(teams, lettersToAssign, randIds,tourn);
            return lettersToAssign;
        }

        public static void giveGroupsRandom(List<Team> teams, IEnumerable<string> lettersToAssign, int[] randIds, Tournament tourn)
        {
            if (isReady(teams.Count))
            {
                int j = 0;
                foreach (var group in lettersToAssign)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        teams.First(x => x.id == randIds[j])
                            .tournamentStats.FirstOrDefault(x => x.tournamentName
                            == tourn.name).group = group;
                        j++;
                    }
                }
            }
        }

        private static bool isReady(int count)
        {
            if (count == 4 || count == 8 || count == 16 || count == 32)
                return true;
            return false;
        }

        private static bool AllTeamsEneteredHaveGrops(List<Team> teams)
        {
            foreach (var team in teams)
                foreach (var stat in team.tournamentStats)
                    if (stat.group == null)
                        return false;
            return true;
        }
    }
}