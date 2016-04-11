using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

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
        private XmlTextReader reader;
        
        public CookieFinder()
        {
            InitializeComponent();
            listOfFiles = new List<string>();
            if(File.Exists("Config.xml"))
            {
                reader = new XmlTextReader("Config.xml");
                while(reader.Read())
                {
                    switch(reader.NodeType)
                    {
                        case XmlNodeType.Element: break;
                        case XmlNodeType.Text: cookiePath = reader.Value; break;
                        case XmlNodeType.EndElement: break;
                        default: break;
                    }
                }
                reader.Close();
            }
            else
            {
                cookiePath = "\\Microsoft\\Windows\\Cookies\\Low";
            }
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
            lblResult.Text = "Letar efter kakor! :)";
            filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + cookiePath;
            
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
                lblResult.Text = "Hittade ingen sådan kaka :(";
                btnDelete.Visible = false;
            }
            else
            {
                if(listFiles.Count == 1)
                    lblResult.Text = "Hittade " + listFiles.Count + " kaka, vill du ta bort (äta upp) denna kaka?";
                else
                    lblResult.Text = "Hittade " + listFiles.Count + " kakor, vill du ta bort (äta upp) dessa kakor?";
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

            
            lblResult.Text = "Samtliga kakor innehållande " + txtInputURL.Text + " är nu borttagna/uppätna!";
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
