using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookLibrary.Models;
using PagedList;
namespace BookLibrary.Controllers

{
    public class booksController : Controller
    {
        private BookLibraryEntities db = new BookLibraryEntities();
        /// <summary>
        /// Return View for displaying Books
        /// </summary>
        /// <param name="sortOrder"> Order of sorting. <field>_<direction>. For example: rating_asc, year_desc </param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString">Search string. Search in "title" and "author" </param>
        /// <param name="page">Number of page. 10 books per page.</param>
        /// <returns>View Index</returns>
        // GET: books
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.currentSort = sortOrder;
            ViewBag.titleSortParm = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.ratingSortParm = sortOrder == "rating_desc" ? "rating_asc" : "rating_desc";
            ViewBag.authorSortParm = sortOrder == "author_desc" ? "author_asc" : "author_desc";
            ViewBag.yearSortParm = sortOrder == "year_desc" ? "year_asc" : "year_desc";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var books = from b in db.books
                        select b;
            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(book => book.title.Contains(searchString)
                                       || book.author.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    books = books.OrderByDescending(book => book.title);
                    break;
                case "rating_desc":
                    books = books.OrderByDescending(book => book.rating);
                    break;
                case "rating_asc":
                    books = books.OrderBy(book => book.rating);
                    break;
                case "author_desc":
                    books = books.OrderByDescending(book => book.author);
                    break;
                case "author_asc":
                    books = books.OrderBy(book => book.author);
                    break;
                case "year_desc":
                    books = books.OrderByDescending(book => book.year);
                    break;
                case "year_asc":
                    books = books.OrderBy(book => book.year);
                    break;
                default:
                    books = books.OrderBy(book => book.title);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(books.ToPagedList(pageNumber, pageSize));
        }
        /// <summary>
        /// Return View with detailed iformation about Book
        /// </summary>
        /// <param name="id">ID of selected book</param> 
        /// <returns>View Details</returns>
        // GET: books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book book = db.books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: books/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Adding new book to library
        /// </summary>
        /// <param name="book"></param>
        /// <returns>View Create</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,title,author,isbn,image,year,description,wishlist,finish,taken,rating")] book book)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    db.books.Add(book);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Error. Could not save book");
            }
            

            return View(book);
        }
        /// <summary>
        /// Return view for editing information about book
        /// </summary>
        /// <param name="id">ID of selected book</param>
        /// <returns>View Edit</returns>
        // GET: books/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book book = db.books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }
        /// <summary>
        /// Edit book in database
        /// </summary>
        /// <param name="book"></param>
        /// <returns>View Index</returns>
        // POST: books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,title,author,isbn,image,year,description,wishlist,finish,taken,rating")] book book)
        {
            try {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Error. Could not save book");
            }
            return View(book);
        }
        /// <summary>
        /// Return view for deleting book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="saveChangesError"></param>
        /// <returns>View Delete</returns>
        // GET: books/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            book book = db.books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }
        /// <summary>
        /// Delete book from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View Index</returns>
        // POST: books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {

                book book = db.books.Find(id);
                db.books.Remove(book);
                db.SaveChanges();
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
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
