CREATE DATABASE LibraryManagement;
GO
USE LibraryManagement;
GO

-- 1. Bảng Category (Thể loại)
CREATE TABLE Category (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL
);

-- 2. Bảng Author (Tác giả)
CREATE TABLE Author (
    AuthorID INT IDENTITY(1,1) PRIMARY KEY,
    AuthorName NVARCHAR(100) NOT NULL
);

-- 3. Bảng Book (Sách) - Đã bỏ AuthorID và thêm CHECK chống âm
CREATE TABLE Book (
    BookID INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    CategoryID INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL CHECK (Price >= 0),
    StockQuantity INT NOT NULL CHECK (StockQuantity >= 0),
    ImageUrl VARCHAR(500),
    Description NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
);

-- BẢNG MỚI: Bảng trung gian Book_Author (Quan hệ Nhiều - Nhiều)
CREATE TABLE Book_Author (
    BookID INT NOT NULL,
    AuthorID INT NOT NULL,
    PRIMARY KEY (BookID, AuthorID),
    FOREIGN KEY (BookID) REFERENCES Book(BookID) ON DELETE CASCADE,
    FOREIGN KEY (AuthorID) REFERENCES Author(AuthorID) ON DELETE CASCADE
);

-- 4. Bảng Member (Thành viên)
CREATE TABLE Member (
    MemberID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE,
    Phone VARCHAR(20),
    Address NVARCHAR(255),
    CardExpiryDate DATE,
    IsLocked BIT DEFAULT 0
);

-- 5. Bảng Loan (Phiếu mượn) - Thêm Ràng buộc ngày tháng và tiền phạt
CREATE TABLE Loan (
    LoanID INT IDENTITY(1,1) PRIMARY KEY,
    MemberID INT NOT NULL,
    LoanDate DATE NOT NULL DEFAULT GETDATE(),
    DueDate DATE NOT NULL,
    ReturnDate DATE,
    TotalFine DECIMAL(18,2) DEFAULT 0 CHECK (TotalFine >= 0),
    FOREIGN KEY (MemberID) REFERENCES Member(MemberID),
    CONSTRAINT CHK_DueDate CHECK (DueDate >= LoanDate),
    CONSTRAINT CHK_ReturnDate CHECK (ReturnDate IS NULL OR ReturnDate >= LoanDate)
);

-- 6. Bảng LoanDetail (Chi tiết phiếu mượn) - Thêm CHECK chống âm
CREATE TABLE LoanDetail (
    LoanDetailID INT IDENTITY(1,1) PRIMARY KEY,
    LoanID INT NOT NULL,
    BookID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    FineAmount DECIMAL(18,2) DEFAULT 0 CHECK (FineAmount >= 0),
    FOREIGN KEY (LoanID) REFERENCES Loan(LoanID) ON DELETE CASCADE,
    FOREIGN KEY (BookID) REFERENCES Book(BookID)
);

-- ==========================================
-- THÊM DỮ LIỆU MẪU
-- ==========================================

INSERT INTO Category (CategoryName) VALUES 
(N'Lập trình'), (N'Kinh tế'), (N'Văn học'), (N'Kỹ năng sống'), (N'Khoa học');

INSERT INTO Author (AuthorName) VALUES 
(N'Nguyễn Nhật Ánh'), (N'Robert C. Martin'), (N'Dale Carnegie'), (N'Stephen Hawking'), (N'Adam Smith');

-- Thêm Sách (Không còn cột AuthorID)
INSERT INTO Book (Title, CategoryID, Price, StockQuantity, ImageUrl, Description, IsActive) VALUES 
(N'Clean Code', 1, 450000, 10, 'book-01.jpg', N'Handbook of Agile Software Craftsmanship', 1),
(N'Cho Tôi Xin Một Vé Đi Tuổi Thơ', 3, 85000, 15, 'book-02.jpg', N'Tác phẩm nổi tiếng viết về tuổi thơ của Nguyễn Nhật Ánh', 1),
(N'Đắc Nhân Tâm', 4, 98000, 20, 'book-03.jpg', N'Sách kỹ năng sống và nghệ thuật thu phục lòng người', 1),
(N'Lược Sử Thời Gian', 5, 150000, 8, 'book-04.jpg', N'Cuốn sách kinh điển về vũ trụ học của Stephen Hawking', 1),
(N'Của Cải Của Các Dân Tộc', 2, 220000, 5, 'book-05.jpg', N'Nền tảng kinh tế học cổ điển của Adam Smith', 1),
(N'Mắt Biếc', 3, 110000, 12, 'book-06.jpg', N'Câu chuyện tình yêu đơn phương đầy day dứt', 1),
(N'The Clean Coder', 1, 480000, 7, 'book-07.jpg', N'A Code of Conduct for Professional Programmers', 1),
(N'Quẳng Gánh Lo Đi Mà Vui Sống', 4, 89000, 18, 'book-08.jpg', N'Bí quyết để vượt qua lo lắng và tận hưởng cuộc sống', 1);

-- Map Tác giả với Sách qua bảng trung gian
INSERT INTO Book_Author (BookID, AuthorID) VALUES 
(1, 2), -- Clean Code: Robert C. Martin
(2, 1), -- Cho Tôi Xin Một Vé Đi Tuổi Thơ: Nguyễn Nhật Ánh
(3, 3), -- Đắc Nhân Tâm: Dale Carnegie
(4, 4), -- Lược Sử Thời Gian: Stephen Hawking
(5, 5), -- Của Cải Của Các Dân Tộc: Adam Smith
(6, 1), -- Mắt Biếc: Nguyễn Nhật Ánh
(7, 2), -- The Clean Coder: Robert C. Martin
(8, 3); -- Quẳng Gánh Lo Đi Mà Vui Sống: Dale Carnegie

-- 7.Bảng Phân quyền (Account) - Quản lý Role (Admin, Librarian, Reader)
CREATE TABLE Account (
    AccountID INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    Password VARCHAR(255) NOT NULL,
    Role NVARCHAR(50) NOT NULL -- Giá trị: Admin, Librarian, Reader
);

-- 8.Bảng Đặt trước sách (Reservation) - Chức năng đặt sách trước
CREATE TABLE Reservation (
    ReserveID INT IDENTITY(1,1) PRIMARY KEY,
    MemberID INT NOT NULL,
    BookID INT NOT NULL,
    ReserveDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) DEFAULT N'Chờ xử lý',
    FOREIGN KEY (MemberID) REFERENCES Member(MemberID),
    FOREIGN KEY (BookID) REFERENCES Book(BookID)
);