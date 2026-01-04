using Hotel_Management_System.Controllers;
using Hotel_Management_System.Screens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hotel_Management_System
{
    public partial class Login : Form
    {

        DatabaseConnection dc = new DatabaseConnection();
        String query;
        public int hotelIdToken;
        public int employeeIdToken;

        public Login()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void guna2ImageRadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private String checkNewUser()
        {
            SqlConnection con = dc.getConnection();
            con.Open();
            query = "SELECT NewUser FROM Authentication.Login WHERE username = '" + usernameTextField.Text + "' AND password = '" + passwordTextField.Text + "'";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            String str = "";
            while (dr.Read())
            {
                str = dr.GetString(0);
            }
            con.Close();
            return str;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            // Step 1: Check if the user is a Super Admin
            query = "SELECT AdminId FROM Authentication.Admin WHERE Username = @username AND Password = @password";
            SqlConnection connection = dc.getConnection();
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", usernameTextField.Text);
            cmd.Parameters.AddWithValue("@password", passwordTextField.Text);
            SqlDataReader reader = cmd.ExecuteReader();

            if (String.IsNullOrEmpty(usernameTextField.Text) || String.IsNullOrEmpty(passwordTextField.Text))
            {
                errorLabel.Text = "        All fields are required.";
                errorLabel.Visible = true;
                connection.Close();
                return;
            }

            if (reader.HasRows)
            {
                // Super Admin login successful
                errorLabel.Visible = false;
                connection.Close();
                this.Hide();
                AdminHotelsView adminView = new AdminHotelsView();
                adminView.Show();
                return;
            }
            connection.Close();

            // Step 2: If not an admin, check the regular user table
            query = "SELECT LoginId, NewUser FROM Authentication.Login WHERE Username = @username AND Password = @password";
            connection.Open();
            cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", usernameTextField.Text);
            cmd.Parameters.AddWithValue("@password", passwordTextField.Text);
            reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                errorLabel.Text = "Incorrect username or password.";
                errorLabel.Visible = true;
                connection.Close();
                return;
            }

            // Read user data
            string isNewUser = "";
            while (reader.Read())
            {
                isNewUser = reader["NewUser"].ToString().Trim();
            }
            connection.Close();

            // Fetch tokens
            TokenHotelIdHOTEL();
            Statics.setHotelId(hotelIdToken);
            TokenEployeeId();
            Statics.setEmployeeId(employeeIdToken);

            if (isNewUser.Equals("Yes"))
            {
                // Redirect to password creation screen
                Statics.setUname(usernameTextField.Text);
                Statics.setPass(passwordTextField.Text);
                CreatePassword reset = new CreatePassword();
                reset.Show();
                this.Hide();
            }
            else
            {
                // Redirect to dashboard
                errorLabel.Visible = false;
                this.Hide();
                Dashboard db = new Dashboard();
                db.Show();
            }
        }

        private void TokenEployeeId()
        {
            SqlConnection con = dc.getConnection();
            con.Open();
            query = "SELECT EmployeeId FROM Authentication.Login WHERE username = @username AND password = @password";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@username", usernameTextField.Text);
            cmd.Parameters.AddWithValue("@password", passwordTextField.Text);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                if (dr.GetValue(0) != DBNull.Value)
                {
                    employeeIdToken = dr.GetInt32(0);
                }
            }
            con.Close();
        }

        private void TokenHotelIdHOTEL()
        {
            SqlConnection con = dc.getConnection();
            con.Open();
            query = "SELECT HotelId FROM Authentication.Login WHERE Username = @username AND Password = @password";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@username", usernameTextField.Text);
            cmd.Parameters.AddWithValue("@password", passwordTextField.Text);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                hotelIdToken = dr.GetInt32(0);
            }
            con.Close();
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
            SuperAdminLogin superAdmin = new SuperAdminLogin();
            superAdmin.Show();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            if (usernameTextField.Text == "")
            {
                MessageBox.Show("Please enter username.", "Missing Info", MessageBoxButtons.OK);
            }
            else
            {
                query = "SELECT LoginId FROM Authentication.Login WHERE Username = @username";
                SqlConnection connection = dc.getConnection();
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@username", usernameTextField.Text);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (reader.HasRows)
                {
                    Statics.setTempUname(usernameTextField.Text);
                    this.Hide();
                    ResetPassword rp = new ResetPassword();
                    rp.Show();
                }
                else
                {
                    MessageBox.Show("Username not found.", "Incorrect Info", MessageBoxButtons.OK);
                }
            }
        }

        private void passwordTextField_TextChanged(object sender, EventArgs e)
        {

        }

        private void changeVisibile(object sender, EventArgs e)
        {
            Image myimage1 = new Bitmap(@"C:\Users\CC 106\Downloads\Hotel-Management-System-master\Icons\eyevisoff.png");
            Image myimage2 = new Bitmap(@"C:\Users\CC 106\Downloads\Hotel-Management-System-master\Icons\eyevisible.png");

            if (passwordTextField.UseSystemPasswordChar == true)
            {
                passwordTextField.UseSystemPasswordChar = false;
                passwordTextField.IconRight = myimage2;
            }
            else
            {
                passwordTextField.UseSystemPasswordChar = true;
                passwordTextField.IconRight = myimage1;
            }
        }

        private void usernameTextField_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow letters, numbers, '@', '.', and control keys (like backspace)
            if (!char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != '@' && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Blocks the key input
            }
        }

        private void passwordTextField_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only letters, numbers, and control keys (like backspace)
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Blocks the key input
            }
        }
    }
}