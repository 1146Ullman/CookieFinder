using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;

namespace CookieFinder
{
    /// <summary>
    /// Class/program to search out specific cookies based on a search parameter.
    /// </summary>
    public partial class CookieFinder : Form
    {
        private List<string> listOfFiles;
        private string filePath;
        private string cookiePath;
        private string lang;
        
        public CookieFinder()
        {
            listOfFiles = new List<string>();
            if(File.Exists("Config.xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("Config.xml");
                XmlNodeList cookieList = doc.GetElementsByTagName("CookieFinderConfig");

                foreach( XmlNode node in cookieList )
                {
                    XmlElement cookieElement = (XmlElement)node;
                    cookiePath = cookieElement.GetElementsByTagName("CookiePath")[0].InnerText;
                    lang = cookieElement.GetElementsByTagName("Language")[0].InnerText;
                }

                lang = lang.ToLower();
                switch(lang)
                {
                    case "sv": CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("sv-SE");
                               Thread.CurrentThread.CurrentUICulture = new CultureInfo("sv-SE"); break;
                    case "en": CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
                               Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US"); break;
                    default: break;
                }
            }
            else
            {
                cookiePath = "\\Microsoft\\Windows\\Cookies\\Low";
            }
            InitializeComponent();
        }
        
        /// <summary>
        /// Exits the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Takes the search parameter in txtInputURL and looks for it in the cookies
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFind_Click(object sender, EventArgs e)
        {
            lblResult.Text = Resources.Strings.Looking;
            //lblResult.Text = "Letar efter kakor! :)";
            if(!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + cookiePath))
            {
                filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + cookiePath;
            }
            else
            {
                filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + cookiePath;
            }
                        
            if( txtInputURL.ToString() != "" )
                findThemCookies(filePath, "*.txt", listOfFiles, txtInputURL.Text);

        }

        /// <summary>
        /// Removes the filthy cookies found with the specified search perameter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            filthyCookies();
        }

        /// <summary>
        /// Looks for cookies containing a specific address
        /// </summary>
        /// <param name="cookieLowPath">File path to where the cookies are</param>
        /// <param name="fileEnding">The file ending</param>
        /// <param name="listFiles">List of cookie files</param>
        /// <param name="lookFor">Holds what to look for in the cookie</param>
        private void findThemCookies(string cookieLowPath, string fileEnding, List<string> listFiles, string lookFor)
        {
            string line = "";
            foreach (string s in Directory.GetFiles(cookieLowPath, fileEnding).Select(Path.GetFileName))
            {
                try
                {
                    line = File.ReadLines(cookieLowPath + "\\" + s).Skip(2).Take(1).First();
                    string found = line.Substring(line.IndexOf(lookFor), lookFor.Length);
                    if( found == lookFor )
                    {
                        listFiles.Add(s);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            if(listOfFiles.Count == 0)
            {
                lblResult.Text = Resources.Strings.NotFound;
                btnDelete.Visible = false;
            }
            else
            {
                if(listFiles.Count == 1)
                    lblResult.Text = Resources.Strings.Found + listFiles.Count + Resources.Strings.Cookie;
                else
                    lblResult.Text = Resources.Strings.Found + listFiles.Count + Resources.Strings.Cookies;
                btnDelete.Visible = true;
            }            
        }

        /// <summary>
        /// Removes the filthy cookies you don't want based on the selection (if any)
        /// </summary>
        private void filthyCookies()
        {
            foreach (string file in listOfFiles)
            {
                File.Delete(filePath + "\\" + file);
            }
            listOfFiles.Clear();

            
            lblResult.Text = Resources.Strings.Containing + txtInputURL.Text + Resources.Strings.Removed;
            btnDelete.Visible = false;
        }

        private void txtInputURL_KeyPress(object sender, KeyPressEventArgs e)
        {
            if( e.KeyChar == (char)Keys.Return )
            {
                btnFind_Click(sender, e);
                /*lblResult.Text = "Letar efter kakor! :)";
                filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + cookiePath;

                if (txtInputURL.ToString() != "")
                    findThemCookies(filePath, "*.txt", listOfFiles, txtInputURL.Text);
                */
                e.Handled = true;
            }
        }

    }
}
