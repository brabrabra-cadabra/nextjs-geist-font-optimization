using System;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Data.Sqlite;

namespace LibreLibrarySystem
{
    public partial class StudentHistoryWindow : Window
    {
        private ObservableCollection<BorrowRecord> history = new ObservableCollection<BorrowRecord>();

        public StudentHistoryWindow(string studentId)
        {
            InitializeComponent();
            HistoryListView.ItemsSource = history;
            LoadBorrowingHistory(studentId);
        }

        private void LoadBorrowingHistory(string studentId)
        {
            history.Clear();
            using var connection = new SqliteConnection("Data Source=library.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT BookTitle, BorrowedOn, ReturnedOn, Status 
                FROM BorrowRecords 
                WHERE StudentId = $studentId
                ORDER BY BorrowedOn DESC";
            command.Parameters.AddWithValue("$studentId", studentId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                history.Add(new BorrowRecord
                {
                    BookTitle = reader.GetString(0),
                    BorrowedOn = reader.GetString(1),
                    ReturnedOn = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Status = reader.GetString(3)
                });
            }
        }
    }
}
