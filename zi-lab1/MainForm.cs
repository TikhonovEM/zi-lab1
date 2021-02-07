using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zi_lab1
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            foreach(var line in System.IO.File.ReadLines("data.txt"))
            {
                Entity entity;
                if (line.StartsWith("f"))
                    entity = new File(line);
                else
                    entity = new Folder(line);
                Context.Entities.Add(entity);
            }
            
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            var login = UsernameInput.Text;
            var pass = PasswordInput.Text;
            var user = Context.Users.SingleOrDefault(u => u.Username.Equals(login) && u.PasswordHash.Equals(pass));
            if(user != null)
            {
                Context.CurrentUser = user;
                new CatalogsForm().Show();
            }
            else
            {
                MessageBox.Show("Не удалось!");
                UsernameInput.Text = string.Empty;
                PasswordInput.Text = string.Empty;
            }
        }
    }
}
