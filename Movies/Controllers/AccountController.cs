using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Movies.Models;
using System.Web.Security;

namespace Movies.Controllers
{
    public class AccountController : Controller
    {
        private MoviesContext db = new MoviesContext();

        // GET: Index
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Movie");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Account model)
        {
            // Check first if Model is valid
            if (ModelState.IsValid)
            {
                
                // TODO: Hash password

                bool userValid = ValidateUser(model.Name, model.Password);

                // User found in the database
                if (userValid)
                {
                    FormsAuthentication.SetAuthCookie(model.Name, false);
                    return RedirectToAction("Index", "Movie");
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return PartialView("_LoginForm", model);
        }

        public bool ValidateUser(string username, string password)
        {
            Account requiredUser = db.Accounts.SingleOrDefault(u => u.Name == username);

            if (requiredUser != null)
            {
                //User exists, validate
                if (requiredUser.Name == username && requiredUser.Password == password)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //User doesn't exist
                return false;
            }
        }

        public void killSession()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            // Clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // Clear session cookie
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            killSession();

            return RedirectToAction("Index");
        }

        // GET: Account/Manage
        [Authorize]
        public async Task<ActionResult> Manage()
        {
            if (User.Identity.Name == null)
            {
                ViewBag.ErrorMessage = "No account Id provided.";
                return View("Error");
            }

            Account account = await db.Accounts.SingleOrDefaultAsync(a => a.Name == User.Identity.Name);

            if (account == null)
            {
                ViewBag.ErrorMessage = "Account not found.";
                return View("Error");
            }

            if (User.Identity.Name != account.Name)
            {
                ViewBag.ErrorMessage = "You can only view the details of your own account.";
                return View("Error");
            }

            return View(account);
        }

        // GET: Account/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Account/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Password")] Account account)
        {
            if (ModelState.IsValid)
            {
                // Search database to see if user name is already taken
                Account accountInDatabase = await db.Accounts.SingleOrDefaultAsync(a => a.Name == account.Name);
                
                if (accountInDatabase != null)
                {
                    ViewBag.ErrorMessage = "Account name taken. Please choose another name.";
                    return View("Error");
                }

                //TODO: Hash password

                db.Accounts.Add(account);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(account);
        }

        // GET: Account/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                ViewBag.ErrorMessage = "No account Id provided.";
                return View("Error");
            }

            Account account = await db.Accounts.FindAsync(id);

            if (account == null)
            {
                ViewBag.ErrorMessage = "Account not found.";
                return View("Error");
            }

            if (User.Identity.Name != account.Name)
            {
                ViewBag.ErrorMessage = "You can only edit your own account.";
                return View("Error");
            }

            return View(account);
        }

        // POST: Account/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Password")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        // GET: Account/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                ViewBag.ErrorMessage = "No account Id provided.";
                return View("Error");
            }
            Account account = await db.Accounts.FindAsync(id);

            if (account == null)
            {
                ViewBag.ErrorMessage = "Account not found.";
                return View("Error");
            }

            if (User.Identity.Name != account.Name)
            {
                ViewBag.ErrorMessage = "You cannot delete an account that is not your own";
                return View("Error");
            }

            return View(account);
        }

        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Account account = await db.Accounts.FindAsync(id);
            db.Accounts.Remove(account);
            await db.SaveChangesAsync();
            return LogOff();
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
