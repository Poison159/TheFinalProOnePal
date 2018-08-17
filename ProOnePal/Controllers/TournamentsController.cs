using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProOnePal.Models;
using System.IO;

namespace ProOnePal.Controllers
{
    public class TournamentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tournaments
        public ActionResult Index()
        {
            return View(db.Tournaments.ToList());
        }

        // GET: Tournaments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tournament tournament   = db.Tournaments.Find(id);
            tournament.enteredTeams = Helper.getenterdTeams(tournament, db);
            tournament.fixtures     = db.Fixtures.Where(x => x.tournament.name == tournament.name).ToList();
            var groups              = Helper.assignTeamsToGroups(tournament, tournament.stages);
            db.SaveChanges();
            ViewBag.Groups = new SelectList(groups);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            return View(tournament);
        }

        // GET: Tournaments/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tournaments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name")] Tournament tournament)
        {
            if (!Helper.GetTournamentName(db).Contains(tournament.name))
            { 
                if (ModelState.IsValid)
                {
                    db.Tournaments.Add(tournament);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(tournament);
        }

        // GET: Tournaments/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tournament tournament = db.Tournaments.Find(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            return View(tournament);
        }

        // POST: Tournaments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,maxGames,maxTeams,maxStages,name")] Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tournament).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tournament);
        }

        // GET: Tournaments/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tournament tournament = db.Tournaments.Find(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            return View(tournament);
        }

        // POST: Tournaments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tournament tournament = db.Tournaments.Find(id);
            db.Tournaments.Remove(tournament);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult AddTeam(int? id, string error)
        {
            ViewBag.teamId          = new SelectList(db.Teams, "id", "name");
            var tournament          = db.Tournaments.Find(id);
            tournament.enteredTeams = Helper.getenterdTeams(tournament, db);
            ViewBag.error = error;
            return View(tournament);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AddTeam(int? id, int teamId)
        {
            ViewBag.teamId = new SelectList(db.Teams, "id", "name");
            var images              = Helper.getImagePaths(db);
            var team                = db.Teams.Find(teamId);
            var tournament          = db.Tournaments.Find(id);
            var tournCount          = db.Tournaments.Count();
            var statsForTeamCount   = db.teamTournamentStats.ToList().Where(x => x.teamId == teamId).Count();

            if (statsForTeamCount < tournCount && !tournament.enteredTeams.Contains(team))
            {
                team.tournamentStats.Add(new TeamTournamentStat()
                { tournamentName = tournament.name });
                team.players = db.Players.Where(x => x.team.id == team.id).ToList();
                foreach (var player in team.players)
                {
                    db.playerTournamentStats.Add(new PlayerTournamentStat()
                    { tournamentName = tournament.name, playerId = player.Id, player = player });
                }
                ViewBag.success = "Team added succesfully";
                tournament.enteredTeams.Add(team);
            }
            else {
                ViewBag.error = "Team already in tournamet";
                ViewBag.images = images;
                return View(tournament);
            }
            db.SaveChanges();
            return View(tournament);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddFixture(int ? id)
        {
            Fixture fixture     = new Fixture();
            var tournament      = db.Tournaments.Find(id);
            fixture.tournament  = tournament;
            var teams           = Helper.getTeamsByTournamentName(db,tournament.name);  
            var teamNames       = Helper.getTeamNames(teams);

            ViewBag.HomeTeam    = new SelectList(teamNames);
            ViewBag.AwayTeam    = new SelectList(teamNames);
            
            return View(fixture);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFixture([Bind(Include = "id,time,homeTeam,awayTeam,date,pitch")] Fixture fixture, int? id)
        {
            var tournament       = db.Tournaments.Find(id);
            var teams            = Helper.getTeamsByTournamentName(db, tournament.name);
            var teamNames        = Helper.getTeamNames(teams);
            var errorMeassage    = "";
            Helper.assignTournamentsToFixtures(db);
            tournament.results   = db.Results.Where(x => x.fixture.tournament.name 
            == tournament.name).ToList();
            fixture.stage        = "GF"; //remember to variate depending on the rounds in tournament
            fixture.tournamentId = (int)id;
            fixture.tournament   = tournament;
            fixture.fixtureName  = Helper.createFixtureName(fixture);
            
            ViewBag.HomeTeam     = new SelectList(teamNames);
            ViewBag.AwayTeam     = new SelectList(teamNames);

            if (Helper.getStageFromTournament(tournament) == "QF")
            {
                if (!Helper.areTeamsInSameGroup(fixture.homeTeam, fixture.awayTeam, tournament, db))
                {
                    errorMeassage = "Home & Away team not in same gruop";
                    ViewBag.error = errorMeassage;
                    return View(fixture);
                }
            }
            if (fixture.homeTeam != fixture.awayTeam)
            {
                if (ModelState.IsValid)
                {
                    db.Fixtures.Add(fixture);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else{
                errorMeassage = "Home & Away team are the same";
                ViewBag.error = errorMeassage;
                return View(fixture);
            }
            return View(fixture);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult RemoveTeam(int? id,string name)
        {
            ViewBag.teamId = new SelectList(db.Teams, "id", "name");
            var team        = db.Teams.Find(id);
            var tourn       = db.Tournaments.ToList().First(x => x.name == name);
            var teamStat    = db.teamTournamentStats.First(x => x.tournamentName == name && x.teamId == id);

            tourn.enteredTeams = Helper.getenterdTeams(tourn, db);
            tourn.enteredTeams.Remove(team);
            if (teamStat.gamesPlayed == 0)
                db.teamTournamentStats.Remove(teamStat);
            else
            {
                ViewBag.error = "You cannot remove team that's already played";
                return RedirectToAction("AddTeam", new { id = tourn.id });
            }
            db.SaveChanges();
            return RedirectToAction("AddTeam",new {id = tourn.id, error = "Team Removed" });
        }
     


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
