using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace ghostTetris
{
    public partial class frmStory : Form
    {
        Panel newPanel;

        public frmStory()
        {
            
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            playStory();

            
        }

        private async void playStory()
        {

            await Task.Delay(500);
            Form party = this;
            System.Media.SoundPlayer media = new System.Media.SoundPlayer("../../../sounds/story1.wav");

            newPanel = new Panel();
            newPanel.Size = new Size(party.Size.Width, party.Size.Height - 30);
            newPanel.Location = new Point(0, 0);
            newPanel.BackgroundImageLayout = ImageLayout.Stretch;
            newPanel.Visible = false;
            party.Controls.Add(newPanel);

            newPanel.Visible = true;
            newPanel.BackgroundImage = new Bitmap("../../../images/scene1.png");
            media.Play();
            await Task.Delay(4500);
            media = new System.Media.SoundPlayer("../../../sounds/story2.wav");

            newPanel.BackgroundImage = new Bitmap("../../../images/scene2.png");
            media.Play();
            await Task.Delay(2500);
            media = new System.Media.SoundPlayer("../../../sounds/story3.wav");

            newPanel.BackgroundImage = new Bitmap("../../../images/scene3.png");
            media.Play();
            await Task.Delay(1250);
            media = new System.Media.SoundPlayer("../../../sounds/story4.wav");
            
            newPanel.BackgroundImage = new Bitmap("../../../images/scene4.png");
            media.Play();
            await Task.Delay(1250);
            media = new System.Media.SoundPlayer("../../../sounds/story5.wav");

            newPanel.BackgroundImage = new Bitmap("../../../images/scene5.png");
            media.Play();
            await Task.Delay(2000);
            media = new System.Media.SoundPlayer("../../../sounds/story6.wav");

            newPanel.BackgroundImage = new Bitmap("../../../images/scene6.png");
            media.Play();
            await Task.Delay(5500);
            media = new System.Media.SoundPlayer("../../../sounds/story7.wav");

            newPanel.BackgroundImage = new Bitmap("../../../images/scene7.png");
            media.Play();
            await Task.Delay(3900);
            media = new System.Media.SoundPlayer("../../../sounds/story8.wav");

            newPanel.BackgroundImage = new Bitmap("../../../images/scene8.png");
            media.Play();
            await Task.Delay(2350);
            media = new System.Media.SoundPlayer("../../../sounds/story9.wav");

            newPanel.BackgroundImage = new Bitmap("../../../images/scene9.png");
            media.Play();
            await Task.Delay(1250);
            media = new System.Media.SoundPlayer("../../../sounds/story10.wav");

            newPanel.BackgroundImage = new Bitmap("../../../images/scene10.png");
            media.Play();
            await Task.Delay(1500);
            media = new System.Media.SoundPlayer("../../../sounds/story11.wav");

            newPanel.BackgroundImage = new Bitmap("../../../images/scene11.png");
            media.Play();
            await Task.Delay(1500);

            newPanel.BackgroundImage = new Bitmap("../../../images/scene12.png");
            media = new System.Media.SoundPlayer("../../../sounds/story12.wav");
            media.Play();
            await Task.Delay(13500);

            new ghostTetris().Show();
            this.Hide();
        }

        private void frmStory_Load(object sender, EventArgs e)
        {

        }

        private void frmStory_Load_1(object sender, EventArgs e)
        {

        }
    }
}
