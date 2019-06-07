using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPManager
{
    public partial class Form1 : Form
    {
        NotifyIcon notifyIcon;
        string previous_ip;
        private System.Windows.Forms.Timer timer1;
        public Form1()
        {
            InitializeComponent();
        }
        void StartWork()
        {
            previous_ip = GetIP();
            SendEmail(previous_ip);
            timer1 = new System.Windows.Forms.Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 15000; // in miliseconds
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (CheckForInternetConnection())
            {
                //Thread.Sleep(3000); 
                string current_ip = GetIP();
                if (current_ip != previous_ip)
                {
                    previous_ip = current_ip;
                    SendEmail(current_ip);
                }
            }
        }
        void SendEmail(string currentip)
        {
            using (var client = new System.Net.Mail.SmtpClient("smtp-mail.outlook.com", 587))
            {
                // Pass SMTP credentials
                client.Credentials =
                    new NetworkCredential("zahret_7ayati@live.com", "123pass!@#");

                // Enable SSL encryption
                client.EnableSsl = true;

                // Try to send the message. Show status in console.
                try
                {
                    Console.WriteLine("Attempting to send email...");
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("example@example.com", "CTMIS-no-reply");
                    mail.To.Add(new MailAddress("example@example.com"));
                   
                    mail.Subject = "Public IP Changed";
                    mail.Body = "New IP: " + currentip;
                    client.Send(mail);
                    Console.WriteLine("Email sent!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The email was not sent.");
                    Console.WriteLine("Error message: " + ex.Message);
                }
            }

        }
               
        string GetIP()
        {
            string externalIP = "";
            externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
            externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")).Matches(externalIP)[0].ToString();
            //string externalip = new WebClient().DownloadString("http://icanhazip.com");
            return externalIP;
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartWork();
            ContextMenu contextMenu1;  //system tray icon right click
            MenuItem menuItem1; //object shows when system tray icon is right clicked

            contextMenu1 = new System.Windows.Forms.ContextMenu();
            menuItem1 = new System.Windows.Forms.MenuItem();

            contextMenu1.MenuItems.AddRange(
                        new System.Windows.Forms.MenuItem[] { menuItem1 }); // Initialize contextMenu1

            menuItem1.Index = 0;    // Initialize menuItem1
            menuItem1.Text = "E&xit";

            menuItem1.Click += new System.EventHandler(menuItem1_Click);    //event handler

            notifyIcon = new NotifyIcon();  //notify icon in system tray
            notifyIcon.Icon = IPManager.Properties.Resources.ip_icon;
            notifyIcon.MouseDoubleClick += new MouseEventHandler(double_click); //event on double click
            notifyIcon.Visible = true;  //make icon always visible in system tray
            notifyIcon.ContextMenu = contextMenu1;
        }

        private void double_click(object sender, MouseEventArgs e)  //on double click on system tray icon, show form
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Show();
        }

        private void menuItem1_Click(object Sender, EventArgs e)    //when system tray icon is right clicked
        {
            System.Environment.Exit(1); // Close the form, which closes the application.
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
