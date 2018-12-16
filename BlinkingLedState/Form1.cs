using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace BlinkingLedState
{ 
    public partial class Form1 : Form
    { 
        private string responseContent;
        enum imageColor {GREEN, RED};
        int activeImage; 

        public Form1()
        {
            InitializeComponent(); 
        }

        public Image getFirstImage(string url)
        { 
            return getImage(url);
        }

        public Image getSecondImage(string url)
        { 
            return getImage(url);
        }

        public System.Drawing.Image getImage(string imageUrl)
        {
            System.Drawing.Image image = null;
            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(imageUrl);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;
                System.Net.WebResponse webResponse = webRequest.GetResponse();
                System.IO.Stream stream = webResponse.GetResponseStream();
                image = System.Drawing.Image.FromStream(stream);
                webResponse.Close();
            }
            catch (Exception ex)
            {
                return null;
            }
            return image;
        } 

        static async Task GetRequest(string url, Action<string> resultMessage)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    using (HttpContent content = response.Content)
                    {
                        string myResponse = await content.ReadAsStringAsync(); 
                        resultMessage?.Invoke(myResponse);
                    }
                }
            }

        } 

        public void getResponse (string input)
        {
            responseContent = input;
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            Regex responseContentPattern = new Regex(@"(?<=.*state.*: )\d+(?=,)");
            Match responseMatch;
            Action<string> myAction = getResponse;
            GetRequest("http://35.190.129.6:3000/led_state", myAction)
                .ContinueWith (task =>
                {
                    responseMatch = responseContentPattern.Match(responseContent); 
                    if (responseMatch.Value=="0") { 
                        myPictureBox.Image = getFirstImage("http://www.clker.com/cliparts/5/7/b/5/1194989231691813435led_circle_red.svg.hi.png");
                        activeImage = (int)imageColor.RED;
                    }
                    else {
                        
                        myPictureBox.Image = getSecondImage("http://www.clker.com/cliparts/1/5/4/b/11949892282132520602led_circle_green.svg.med.png");
                        activeImage = (int)imageColor.GREEN;
                    }  
                });  
        } 

        public void Form1_FormClosed(object sender, FormClosedEventArgs e)
        { 
            // your code here
        }

        private void myPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (activeImage==0)
            {
                //Green ONE
                myPictureBox.Image = getFirstImage("http://www.clker.com/cliparts/5/7/b/5/1194989231691813435led_circle_red.svg.hi.png");
                activeImage = (int)imageColor.RED;
            }
            else
            {
                myPictureBox.Image = getSecondImage("http://www.clker.com/cliparts/1/5/4/b/11949892282132520602led_circle_green.svg.med.png");
                activeImage = (int)imageColor.GREEN;
            }
        }
    }
}
