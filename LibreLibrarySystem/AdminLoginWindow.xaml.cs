using System.Windows;

namespace LibreLibrarySystem
{
    public partial class AdminLoginWindow : Window
    {
        private const string AdminUsername = "admin";
        private const string AdminPassword = "password123"; // For demo purposes only, in real app use secure storage

        public AdminLoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

if (username == AdminUsername && password == AdminPassword)
{
    MessageBox.Show("Login successful. You can now manage the library system.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    DialogResult = true;
    this.Close();
}
else
{
    MessageBox.Show("Invalid credentials. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
}
}

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
