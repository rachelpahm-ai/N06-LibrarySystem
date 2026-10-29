using N06_LibrarySystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace N06_LibrarySystem.Controllers
{
    public class BooksController : Controller
    {
        private LibraryManagementEntities2 db = new LibraryManagementEntities2();

        public ActionResult Index()
        {
            var books = db.Book.ToList();
            return View(books);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Category, "CategoryID", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Book book, HttpPostedFileBase imageFile, string AuthorName)
        {
            book.CategoryID = 1;

            var author = db.Author.FirstOrDefault(a => a.AuthorName == AuthorName);
            if (author == null)
            {
                author = new Author { AuthorName = AuthorName };
                db.Author.Add(author);
                db.SaveChanges();
            }
            book.AuthorID = author.AuthorID;

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(imageFile.FileName);
                string folderPath = Server.MapPath("~/Images/");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string path = Path.Combine(folderPath, fileName);
                imageFile.SaveAs(path);
                book.ImageUrl = "/Images/" + fileName;
            }

            if (ModelState.IsValid)
            {
                db.Book.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(book);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Book.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }

            if (book.Author != null)
            {
                ViewBag.AuthorName = book.Author.AuthorName;
            }

            ViewBag.CategoryID = new SelectList(db.Category, "CategoryID", "CategoryName", book.CategoryID);
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Book book, HttpPostedFileBase imageFile, string AuthorName)
        {
            var author = db.Author.FirstOrDefault(a => a.AuthorName == AuthorName);
            if (author == null)
            {
                author = new Author { AuthorName = AuthorName };
                db.Author.Add(author);
                db.SaveChanges();
            }
            book.AuthorID = author.AuthorID;
            book.CategoryID = 1;

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(imageFile.FileName);
                string folderPath = Server.MapPath("~/Images/");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string path = Path.Combine(folderPath, fileName);
                imageFile.SaveAs(path);
                book.ImageUrl = "/Images/" + fileName;
            }
            else
            {
                var existingBook = db.Book.AsNoTracking().FirstOrDefault(b => b.BookID == book.BookID);
                if (existingBook != null)
                {
                    book.ImageUrl = existingBook.ImageUrl;
                }
            }

            if (ModelState.IsValid)
            {
                db.Entry(book).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Category, "CategoryID", "CategoryName", book.CategoryID);
            return View(book);
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Book.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Book.Find(id);
            db.Book.Remove(book);
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