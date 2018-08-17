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
    public class TeamsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Teams
        public ActionResult Index(string searchName, string kasi)
        {
            var kasiList    = new List<string>();
            var teams       = from cr in db.Teams select cr;
            var kasiquery   = from gmq in db.Teams
                            orderby gmq.kasi
                            select gmq.kasi;
            if (kasiquery.ToList().Count == 0) // go to Create because there is no team
                return PartialView("_NoTeams");

            if (!string.IsNullOrEmpty(searchName))
                teams = teams.Where(x => x.name.Contains(searchName));

            if (!string.IsNullOrEmpty(kasi))
            {
                teams = teams.Where(x => x.kasi.
                ToString().Equals(kasi));
            }
            kasiList.AddRange(kasiquery.Distinct());
            ViewBag.kasi = new SelectList(kasiList);
            db.SaveChanges();
            return View(teams);
        }
        // GET: Teams/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Team team       = db.Teams.Find(id);
            var players     = db.Players.Where(x => x.team.id == id);
            team.players    = players.ToList();
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }
        
        public ActionResult CurrentTeamPlayers(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var players = db.Players.Where(x => x.team.id == id);
            if (players == null)
            {
                return HttpNotFound();
            }
            ViewBag.teamName = db.Teams.Find(id).name;
            return View(players);
        }
        
        public ActionResult AddPlayer(int? id)
        {
            Player player       = new Player();
            var positions       = Helper.ReturnPositions();
            var playerTeam      = db.Teams.Find(id);
            player.team         = playerTeam;
            ViewBag.position    = new SelectList(positions);
            return View(player);
        }



        [HttpPost]
        public ActionResult AddPlayer([Bind(Include = "name,age,position,imgPath,imageUpload")] Player player, int? id)
        {
            var positions       = Helper.ReturnPositions();
            var playerTeam      = db.Teams.Find(id);
            ViewBag.position    = new SelectList(positions);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            player.teamId       = (int)id;
            player.team         = playerTeam;
            if (ModelState.IsValid)
            {
                db.Players.Add(player);
                db.SaveChanges();
                return RedirectToAction("CurrentTeamPlayers", new { id = id });
            }
            return View(player);
        }



        // GET: Teams/Create
        public ActionResult Create()
        {
            Team team = new Team();
            return View(team);
        }

        // POST: Teams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,kasi,imgPath,imageUpload")] Team team)
        {
            if (ModelState.IsValid)
            {
                if (team.imageUpload != null)
                {
                    string fileName     = Path.GetFileNameWithoutExtension(team.imageUpload.FileName);
                    string extention    = Path.GetExtension(team.imageUpload.FileName);
                    fileName            = fileName + DateTime.Now.ToString("yymmssfff") + extention;
                    team.imgPath        = "~/Content/imgs/" + fileName;
                    team.imageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/imgs/"), fileName));
                }
                if (!Helper.getTeamNames(db.Teams.ToList()).Contains(team.name))
                    db.Teams.Add(team);
                else
                {
                    ViewBag.error = "Team already exists";
                    return View(team);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
                
            }
            return View(team);
        }

        // GET: Teams/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,kasi,imgPath,imageUpload")] Team team)
        {
            if (ModelState.IsValid)
            {
                if (team.imageUpload != null)
                {
                    string fileName     = Path.GetFileNameWithoutExtension(team.imageUpload.FileName);
                    string extention    = Path.GetExtension(team.imageUpload.FileName);
                    fileName            = fileName + DateTime.Now.ToString("yymmssfff") + extention;
                    team.imgPath        = "~/Content/imgs/" + fileName;
                    team.imageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/imgs/"), fileName));
                }

                db.Entry(team).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(team);
        }

        // GET: Teams/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Team team = db.Teams.Find(id);
            db.Teams.Remove(team);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Addteams()
        {
            var filePath    = @"C:\Users\Siya\Documents\Visual Studio 2015\Projects\ProOnePal\ProOnePal\obj\Debug\TestTeams.txt";
            var myFile      = System.IO.File.Open(filePath, FileMode.Open);
            using (StreamReader myStream = new StreamReader(myFile))
            {
                if (System.IO.File.Exists(filePath))
                {

                    string line;
                    while ((line = myStream.ReadLine()) != null)
                    {
                        var elems = line.Split(',');
                        var team = new Team() { name = elems[0], kasi = elems[1] };
                        db.Teams.Add(team);    
                    }
                }
            }
            db.SaveChanges();
            return View(db.Teams.ToList());
        }

        [HttpPost]
        public string ChageTeamImage(string teamName)
        {
            var teams = Helper.getImagePaths(db);
            return teams[teamName];
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
