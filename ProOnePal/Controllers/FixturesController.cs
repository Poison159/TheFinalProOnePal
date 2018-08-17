using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProOnePal.Models;

namespace ProOnePal.Controllers
{
    public class FixturesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public FixturesController()
        {
            Helper.assignTournamentsToFixtures(db);
        }
        // GET: Fixtures
        public ActionResult Index()
        {
            Dictionary<string,string> images    = Helper.getImagePaths(db);
            ViewBag.Images                      = images;
            return View(db.Fixtures.ToList().Where(x => x.Played == "No"));
        }

        // GET: Fixtures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fixture fixture = db.Fixtures.Find(id);
            if (fixture == null)
                return HttpNotFound();
            
            return View(fixture);
        }

        // GET: Fixtures/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var stages              = Helper.getStages();
            var tournaments         = Helper.GetTournamentName(db);
            var teamNames           = Helper.getTeamNames(db.Teams.ToList());
            
            ViewBag.tournamentName  = new SelectList(tournaments);
            ViewBag.HomeTeam        = new SelectList(teamNames);
            ViewBag.AwayTeam        = new SelectList(teamNames);

            return View();
        }

        // POST: Fixtures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,tournamentName,time,homeTeam,awayTeam,date,pitch")] Fixture fixture)
        {
            var stages              = Helper.getStages();
            var tournaments         = Helper.GetTournamentName(db);
            var teamNames           = Helper.getTeamNames(db.Teams.Where(x => 
            x.tournamentStats.Where(a => a.tournamentName == fixture.tournament.name)
            != null).ToList());
            
            fixture.stage           = "Group Stages"; //remember to variate depending on the rounds in tournament
            ViewBag.Stages          = new SelectList(stages);
            ViewBag.tournamentName  = new SelectList(tournaments);
            ViewBag.HomeTeam        = new SelectList(teamNames);
            ViewBag.AwayTeam        = new SelectList(teamNames);
            //end

            if (ModelState.IsValid)
            {
                var tourn = db.Tournaments.First(x => x.name == fixture.tournament.name);
                tourn.fixtures.Add(fixture);
                db.Fixtures.Add(fixture);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fixture);
        }

        // GET: Fixtures/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fixture fixture = db.Fixtures.Find(id);
            if (fixture == null)
            {
                return HttpNotFound();
            }
            return View(fixture);
        }

        // POST: Fixtures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,tournamentId,stage,homeTeam,awayTeam,date,pitch")] Fixture fixture)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fixture).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fixture);
        }

        // GET: Fixtures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fixture fixture = db.Fixtures.Find(id);
            if (fixture == null)
            {
                return HttpNotFound();
            }
            return View(fixture);
        }

        // POST: Fixtures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Fixture fixture = db.Fixtures.Find(id);
            db.Fixtures.Remove(fixture);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CreateResult(int? id)
        {
            Result result       = new Result();
            var fixture         = db.Fixtures.Find(id);
            result.fixtureId    = fixture.id;
            result.fixture      = fixture;

            ViewBag.homeTeam    = result.fixture.fixtureName.Split(',').ElementAt(0);
            ViewBag.awayTeam    = result.fixture.fixtureName.Split(',').ElementAt(1);
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateResult([Bind(Include = "id,fixtureId,homeGoals,awayaGoals")] Result result, int? id)
        {
            var fixture         = db.Fixtures.Find(id);
            result.fixture      = fixture;
            ViewBag.homeTeam    = result.fixture.fixtureName.Split(',').ElementAt(0);
            ViewBag.awayTeam    = result.fixture.fixtureName.Split(',').ElementAt(1);
            // make sure not to save duplicate ( done by removing fixtures after result is set)
            // save so as to edit other props after taking goals input
            if (ModelState.IsValid)
            {
                db.Results.Add(result);
                Helper.SaveData(db, result);
                // got to page saying "result added successfully and have a link 
                //with result id to GoalsInfo"
                return RedirectToAction("ResultDetails", new {id = result.id }); 
            }
            
            return View(result);
        }
        public ActionResult ResultDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Result result = db.Results.Find(id);
            if (result == null)
            {
                return HttpNotFound();
            }
            Helper.assignTournamentsToFixtures(db);
            return View(result);
        }

        public ActionResult GoalsInfo(int? id)
        {
            var result          = db.Results.Find(id);
            var playersInGame   = db.Players.ToList().Where(x => x.team.name
            == result.fixture.homeTeam || x.team.name == result.fixture.awayTeam);
            foreach (var player in playersInGame)
            {
                var playerStat  = db.playerTournamentStats.First(x => x.tournamentName 
                == result.fixture.tournament.name && x.playerId == player.Id);
                playerStat.gamesPlayed++;
            }
            db.SaveChanges();
            ViewBag.PlayersId = new SelectList(playersInGame, "Id", "name");
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GoalsInfo(int? id, int playersId)
        {
            
            var player          = db.Players.Find(playersId);
            var result          = db.Results.Find(id);
            var statsForPlayer  = db.playerTournamentStats.ToList().Where(x => x.playerId == playersId);
            var tourn           = db.Tournaments.Find(result.fixture.tournamentId);
            result.scorers      = db.playerResultStats.Where(x => x.resultId == result.id).ToList();
            var playersInGame   = db.Players.ToList().Where(x => x.team.name
            == result.fixture.homeTeam || x.team.name == result.fixture.awayTeam);
            var totalGoals      = (result.homeGoals + result.awayaGoals) - result.scorers.Count;
            
            if (statsForPlayer.Count() <= db.Tournaments.Count() && totalGoals > 0)
            {
                var playerStat  = player.tournamentStats.First(x => x.tournamentName == tourn.name);
                playerStat.goals++;
                PlayerResultStat ResultStat = new PlayerResultStat {
                    playerId = player.Id,
                    player = player,
                    resultId = result.id,
                    result = result
                };
                ResultStat.goals++;
                db.playerResultStats.Add(ResultStat);
                db.SaveChanges();
            }
            ViewBag.PlayersId = new SelectList(playersInGame, "Id", "name");
            if (totalGoals == 0)
                ViewBag.error = "No more Goals to assign";

            
            return View(result);
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
