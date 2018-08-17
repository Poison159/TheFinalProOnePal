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
    public class PlayersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Players
        public ActionResult Index()
        {
            var players = db.Players.Include(p => p.team);
            return View(players.ToList());
        }

        // GET: Players/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // GET: Players/Create
        public ActionResult Create()
        {
            ViewBag.teamId = new SelectList(db.Teams, "id", "name");
            Player player = new Player();
            var positions = Helper.ReturnPositions();
            ViewBag.position = new SelectList(positions);
            return View(player);
        }

        // POST: Players/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,teamId,name,age,position,imgPath")] Player player)
        {
            ViewBag.teamId = new SelectList(db.Teams, "id", "name", player.teamId);
            var positions = Helper.ReturnPositions();
            ViewBag.position = new SelectList(positions);
            if (ModelState.IsValid)
            {
                if (!Helper.CheckIfPlayerInTeam(player,db.Teams,db.Players))
                    db.Players.Add(player);
                else
                {
                    ViewBag.error = "Player already exists";
                    return View(player);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(player);
        }

        // GET: Players/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            ViewBag.teamId = new SelectList(db.Teams, "id", "name", player.teamId);
            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,teamId,name,age,position,imgPath")] Player player)
        {
            if (ModelState.IsValid)
            {
                db.Entry(player).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.teamId = new SelectList(db.Teams, "id", "name", player.teamId);
            return View(player);
        }

        // GET: Players/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Player player = db.Players.Find(id);
            db.Players.Remove(player);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult AddPlayers()
        {
            var filePath = @"C:\Users\Siya\Documents\Visual Studio 2015\Projects\ProOnePal\ProOnePal\obj\Debug\TestPlayers.txt";
            var myFile = System.IO.File.Open(filePath, FileMode.Open);
            using (StreamReader myStream = new StreamReader(myFile))
            {
                if (System.IO.File.Exists(filePath))
                {
                    string line;
                    while ((line = myStream.ReadLine()) != null)
                    {
                        
                        var elems = line.Split(',');
                        var player = new Player() { name = elems[0], age = int.Parse(elems[1]) };
                        player.position = Helper.RandomPosition();
                        player.teamId  =  Helper.RandomTeamsId(db.Teams.ToList());
                        db.Players.Add(player);
                    }
                }
            }
            db.SaveChanges();
            return View(db.Teams.ToList());
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
