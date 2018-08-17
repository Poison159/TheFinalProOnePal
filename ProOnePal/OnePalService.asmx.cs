using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using ProOnePal.Models;

namespace ProOnePal
{
    /// <summary>
    /// Summary description for OnePalService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class OnePalService : System.Web.Services.WebService
    {

        [WebMethod]
        public string getPathfromName(string teamName)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var teamPaths = Helper.getImagePaths(db);
            return teamPaths[teamName];
        }
    }
}
