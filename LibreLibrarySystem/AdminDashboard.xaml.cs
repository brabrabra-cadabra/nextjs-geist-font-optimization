using System;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Data.Sqlite;

namespace LibreLibrarySystem
{
    public partial class AdminDashboard : Window
    {
        private ObservableCollection<Book> books = new ObservableCollection<Book>();
        private ObservableCollection<Student> students = new ObservableCollection<Student>();
        private ObservableCollection<BorrowRecord> borrowings = new ObservableCollection<BorrowRecord>();

        public AdminDashboard()
        {
            InitializeComponent();
            BooksListView.ItemsSource = books;
            StudentsListView.ItemsSource = students;
            BorrowingsListView.ItemsSource = borrowings;
            LoadData();
        }

        private void LoadData()
        {
            LoadBooks();
            LoadStudents();
            LoadBorrowings();
        }

        private void LoadBooks()
        {
            books.Clear();
            using var connection = new SqliteConnection("Data Source=library.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT BookTitle, ISBN, Status, AddedOn FROM Books";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                books.Add(new Book
                {
                    Title = reader.GetString(0),
                    ISBN = reader.GetString(1),
                    Status = reader.GetString(2),
                    AddedOn = reader.GetString(3)
                });
            }
        }

        private void LoadStudents()
        {
            students.Clear();
            using var connection = new SqliteConnection("Data Source=library.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT StudentId, Name, Course, EnrolledOn FROM Students";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                students.Add(new Student
                {
                    StudentId = reader.GetString(0),
                    Name = reader.GetString(1),
                    Course = reader.GetString(2),
                    EnrolledOn = reader.GetString(3)
                });
            }
        }

        private void LoadBorrowings()
        {
            borrowings.Clear();
            using var connection = new SqliteConnection("Data Source=library.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT s.Name, b.BookTitle, br.BorrowedOn, br.ReturnedOn, br.Status 
                FROM BorrowRecords br
                JOIN Students s ON br.StudentId = s.StudentId
                JOIN Books b ON br.BookTitle = b.BookTitle";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                borrowings.Add(new BorrowRecord
                {
                    StudentName = reader.GetString(0),
                    BookTitle = reader.GetString(1),
                    BorrowedOn = reader.GetString(2),
                    ReturnedOn = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Status = reader.GetString(4)
                });
            }
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            string title = NewBookTitleBox.Text.Trim();
            string isbn = NewBookISBNBox.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(isbn))
            {
                MessageBox.Show("Please enter both title and ISBN.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var connection = new SqliteConnection("Data Source=library.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Books (BookTitle, ISBN, Status, AddedOn) 
                VALUES ($title, $isbn, 'available', $addedOn)";
            command.Parameters.AddWithValue("$title", title);
            command.Parameters.AddWithValue("$isbn", isbn);
            command.Parameters.AddWithValue("$addedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            try
            {
                command.ExecuteNonQuery();
                MessageBox.Show("Book added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                NewBookTitleBox.Clear();
                NewBookISBNBox.Clear();
                LoadBooks();
            }
            catch (SqliteException)
            {
                MessageBox.Show("Error adding book. The book might already exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            string id = NewStudentIdBox.Text.Trim();
            string name = NewStudentNameBox.Text.Trim();
            string course = NewStudentCourseBox.Text.Trim();

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(course))
            {
                MessageBox.Show("Please fill in all fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var connection = new SqliteConnection("Data Source=library.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Students (StudentId, Name, Course, EnrolledOn) 
                VALUES ($id, $name, $course, $enrolledOn)";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$name", name);
            command.Parameters.AddWithValue("$course", course);
            command.Parameters.AddWithValue("$enrolledOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            try
            {
                command.ExecuteNonQuery();
                MessageBox.Show("Student added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                NewStudentIdBox.Clear();
                NewStudentNameBox.Clear();
                NewStudentCourseBox.Clear();
                LoadStudents();
            }
            catch (SqliteException)
            {
                MessageBox.Show("Error adding student. The student ID might already exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BooksButton_Click(object sender, RoutedEventArgs e)
        {
            BooksSection.Visibility = Visibility.Visible;
            StudentsSection.Visibility = Visibility.Collapsed;
            BorrowingsSection.Visibility = Visibility.Collapsed;
        }

        private void StudentsButton_Click(object sender, RoutedEventArgs e)
        {
            BooksSection.Visibility = Visibility.Collapsed;
            StudentsSection.Visibility = Visibility.Visible;
            BorrowingsSection.Visibility = Visibility.Collapsed;
        }

        private void BorrowingsButton_Click(object sender, RoutedEventArgs e)
        {
            BooksSection.Visibility = Visibility.Collapsed;
            StudentsSection.Visibility = Visibility.Collapsed;
            BorrowingsSection.Visibility = Visibility.Visible;
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class Book
    {
        public string Title { get; set; } = "";
        public string ISBN { get; set; } = "";
        public string Status { get; set; } = "";
        public string AddedOn { get; set; } = "";
    }

    public class Student
    {
        public string StudentId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Course { get; set; } = "";
        public string EnrolledOn { get; set; } = "";
    }
}
