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

namespace Movies.Controllers
{
    public class MovieController : Controller
    {
        private MoviesContext db = new MoviesContext();

        // GET: Movie
        [Authorize]
        public async Task<ActionResult> Index()
        {           
            Account user = await db.Accounts.SingleOrDefaultAsync(u => u.Name == User.Identity.Name);

            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Logoff", "Account");
            }
        }

        // GET: Movie/Details/5
        [Authorize]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                ViewBag.ErrorMessage = "No movie Id provided.";
                return View("Error");
            }

            Movie movie = await db.Movies.FindAsync(id);

            if (movie == null)
            {
                ViewBag.ErrorMessage = "Movie not found.";
                return View("Error");
            }

            if (User.Identity.Name != movie.Account.Name)
            {
                ViewBag.ErrorMessage = "You can only view the details your own movies.";
                return View("Error");
            }

            return View(movie);
        }

        // GET: Movie/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Movie/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,ReleaseDate,Genre,Price,Rating,AccountId")] Movie movie)
        {
            Account currentUser = await db.Accounts.SingleOrDefaultAsync(u => u.Name == User.Identity.Name);

            movie.AccountId = currentUser.Id;

            if (ModelState.IsValid)
            {
                db.Movies.Add(movie);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", movie.AccountId);
            return View(movie);
        }

        // GET: Movie/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                ViewBag.ErrorMessage = "No account Id provided.";
                return View("Error");
            }

            Movie movie = await db.Movies.FindAsync(id);

            if (movie == null)
            {
                ViewBag.ErrorMessage = "Movie not found.";
                return View("Error");
            }

            if (User.Identity.Name != movie.Account.Name)
            {
                ViewBag.ErrorMessage = "You can only edit your own movies.";
                return View("Error");
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", movie.AccountId);

            return View(movie);
        }

        // POST: Movie/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            Account currentUser = await db.Accounts.SingleOrDefaultAsync(u => u.Name == User.Identity.Name);

            movie.AccountId = currentUser.Id;

            if (ModelState.IsValid)
            {
                db.Entry(movie).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", movie.AccountId);
            return View(movie);
        }

        // GET: Movie/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                ViewBag.ErrorMessage = "No account Id provided.";
                return View("Error");
            }
            Movie movie = await db.Movies.FindAsync(id);

            if (movie == null)
            {
                ViewBag.ErrorMessage = "Movie not found.";
                return View("Error");
            }

            if (User.Identity.Name != movie.Account.Name)
            {
                ViewBag.ErrorMessage = "You can only delete your own movies.";
                return View("Error");
            }

            return View(movie);
        }

        // POST: Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Movie movie = await db.Movies.FindAsync(id);
            db.Movies.Remove(movie);
            await db.SaveChangesAsync();
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
