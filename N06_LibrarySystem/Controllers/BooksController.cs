using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using N06_LibrarySystem.Models;

namespace N06_LibrarySystem.Controllers
{
    public class BooksController : Controller
    {
        private LibraryManagementEntities db = new LibraryManagementEntities();

        // GET: Books
        private string SaveImage(HttpPostedFileBase imageFile)
        {
            string originalFileName = Path.GetFileName(imageFile.FileName);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(originalFileName);
            string extension = Path.GetExtension(originalFileName).ToLower();

            string folder = Server.MapPath("~/Content/images");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string fileName = fileNameWithoutExt + extension;
            string path = Path.Combine(folder, fileName);

            int count = 1;
            while (System.IO.File.Exists(path))
            {
                fileName = $"{fileNameWithoutExt}-{count}{extension}";
                path = Path.Combine(folder, fileName);
                count++;
            }

            imageFile.SaveAs(path);
            return fileName; // Chỉ lưu tên file vào CSDL
        }
        public ActionResult Index()
        {
            var book = db.Book.Include(b => b.Category);
            return View(book.ToList());
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
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

        // GET: Books/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Category, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
    [Bind(Include = "BookID,Title,CategoryID,Price,StockQuantity,Description,IsActive")] Book book,
    HttpPostedFileBase imageFile)
        {
            // Kiểm tra nếu người dùng có chọn ảnh
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                string extension = Path.GetExtension(imageFile.FileName).ToLower();

                // Kiểm tra định dạng ảnh
                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                {
                    ModelState.AddModelError(
                        "ImageUrl",
                        "Chỉ chấp nhận file JPG, JPEG hoặc PNG.");
                }

                // Kiểm tra dung lượng ảnh
                if (imageFile.ContentLength > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        "ImageUrl",
                        "Dung lượng ảnh không được vượt quá 2 MB.");
                }
            }

            if (ModelState.IsValid)
            {
                // Lưu ảnh nếu có chọn ảnh
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    book.ImageUrl = SaveImage(imageFile);
                }

                db.Book.Add(book);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(
                db.Category,
                "CategoryID",
                "CategoryName",
                book.CategoryID);

            return View(book);
        }

        // GET: Books/Edit/5
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
            ViewBag.CategoryID = new SelectList(db.Category, "CategoryID", "CategoryName", book.CategoryID);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
    [Bind(Include = "BookID,Title,CategoryID,Price,StockQuantity,Description,IsActive")] Book book,
    HttpPostedFileBase imageFile)
        {
            // Tìm sách đang có trong database
            Book oldBook = db.Book.Find(book.BookID);

            if (oldBook == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra ảnh mới nếu người dùng có chọn
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                string extension = Path.GetExtension(imageFile.FileName).ToLower();

                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                {
                    ModelState.AddModelError(
                        "ImageUrl",
                        "Chỉ chấp nhận file JPG, JPEG hoặc PNG.");
                }

                if (imageFile.ContentLength > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        "ImageUrl",
                        "Dung lượng ảnh không được vượt quá 2 MB.");
                }
            }

            if (ModelState.IsValid)
            {
                // Cập nhật thông tin sách
                oldBook.Title = book.Title;
                oldBook.CategoryID = book.CategoryID;
                oldBook.Price = book.Price;
                oldBook.StockQuantity = book.StockQuantity;
                oldBook.Description = book.Description;
                oldBook.IsActive = book.IsActive;

                // Chỉ thay ảnh khi người dùng chọn ảnh mới.
                // Nếu không chọn, oldBook.ImageUrl giữ nguyên.
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    oldBook.ImageUrl = SaveImage(imageFile);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Giữ tên ảnh cũ khi hiển thị lại form có lỗi
            book.ImageUrl = oldBook.ImageUrl;

            ViewBag.CategoryID = new SelectList(
                db.Category,
                "CategoryID",
                "CategoryName",
                book.CategoryID);

            return View(book);
        }

        // GET: Books/Delete/5
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

        // POST: Books/Delete/5
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
