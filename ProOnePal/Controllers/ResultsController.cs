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
    public class ResultsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Results
        public ActionResult Index()
        {
            var results = db.Results.ToList();
            foreach (var res in results)
            {
                res.fixture = db.Fixtures.Find(res.fixtureId);
                res.scorers = db.playerResultStats.Where(x => x.resultId == res.id).ToList();
                Helper.assignPlayersToScorers(res.scorers,db);
            }
            Helper.assignTournamentsToFixtures2(results,db);
            return View(results);
        }

        // GET: Results/Details/5
        public ActionResult Details(int? id)
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

        // GET: Results/Create
        public ActionResult Create()
        {
            Result result = new Result();
            ViewBag.fixtureId = new SelectList(db.Fixtures.Where(x => x.Played == "N"),"id","fixtureName");
            return View(result);
        }

        // POST: Results/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,fixtureId")] Result result)
        {
            var fixture         = db.Fixtures.Find(result.fixtureId);
            result.fixture      = fixture;
            result.homeGoals    = 0; result.awayaGoals = 0;
            
            ViewBag.fixtureId   = new SelectList(db.Fixtures,"id","fixtureName", result.fixtureId);
            ViewBag.homeTeam    = result.fixture.fixtureName.Split(',').ElementAt(0);
            ViewBag.awayTeam    = result.fixture.fixtureName.Split(',').ElementAt(1);
            // make sure not to save duplicate ( done by removing fixtures after result is set)
                db.Results.Add(result); // save so as to edit other props after taking goals input

            db.SaveChanges();
            return PartialView("_ResultsInfo");
        }
        [HttpPost]
        public ActionResult ResultsInfo(string homeGoals, string awayGoals)
        {
            int goalsForHome    = 0;
            int goalsForAway    = 0;
            if (!Helper.IsInputValid(homeGoals, awayGoals,
                ref goalsForHome, ref goalsForAway))
            {
                ViewBag.error   = "goals input was invalid";
                var fixtures    = Helper.getFixturesString(db.Fixtures.ToList());
                ViewBag.fixture = new SelectList(fixtures);
                return PartialView("Create");
            }

            if (ModelState.IsValid && db.Results != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Results/Edit/5
        public ActionResult Edit(int? id)
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
            return View(result);
        }

        // POST: Results/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,fixtureName,homeGoals,awayaGoals")] Result result)
        {
            if (ModelState.IsValid)
            {
                db.Entry(result).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(result);
        }

        // GET: Results/Delete/5
        public ActionResult Delete(int? id)
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
            return View(result);
        }

        // POST: Results/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Result result = db.Results.Find(id);
            db.Results.Remove(result);
            db.SaveChanges();
            return RedirectToAction("Index");
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
