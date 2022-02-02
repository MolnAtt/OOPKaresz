using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Karesz
{
    public partial class Form1 : Form
    {
		void FELADAT()
		{
			Robot Karesz = new Robot("Karesz", new int[] { 100,20,20,20,0}, this);

            for (int i = 0; i < 10; i++)
            {
				Karesz.Lép();
            }
			Karesz.Fordul(1);
		}

        /// <summary>
        /// Végrehajtja a feladatot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void startgomb2_Click(object sender, EventArgs e)
        {
            Enged = false;
            FELADAT();
            Enged = true;
            MessageBox.Show("Vége!");
        }
        bool enged;
        bool Enged
        {
            get => enged;
            set 
            {
                foreach (TextBox textbox in textboxok)
                    textbox.Enabled = value;
                foreach (Button gomb in gombok)
                    gomb.Enabled = value;
                enged = value;
            }
        }
        void elozorobotgomb_Click(object sender, EventArgs e)
        {
            Robot.Megfigyelt_léptetése(-1);
            Frissít();
        }
        
        
        void kövtkezőrobotgomb_Click(object sender, EventArgs e)
        {
            Robot.Megfigyelt_léptetése(1);
            Frissít();
        }

        /// <summary>
        /// pálya betöltése gombbal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void pályagomb_Click(object sender, EventArgs e) => 
            pálya.Betölt(pályatextbox.Text);

        /// <summary>
        /// Ha valaki a pályára kattint, akkor Kareszt leteszi oda.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void képkeret_MouseDown(object sender, MouseEventArgs e)
        {
            if (Robot.lista.Count>0)
            {
                Robot.megfigyelt.Teleport(e.X / pálya.L.X, e.Y / pálya.L.Y);
                képkeret.Refresh();
                Frissít();
            }
        }

        /// <summary>
        /// Lerajzolja a Pályánál található függvénnyel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void képkeret_Paint(object sender, PaintEventArgs e) => 
            pálya.Rajz(képkeret, e);

        void karesznagyításkeret_Click(object sender, EventArgs e) =>
            Robot.megfigyelt.Fordul(jobbra);

        void Frissít()
        {
            if (Robot.lista.Count > 0)
            {
                //Robot.megfigyelt = Robot.lista[Robot.megfigyeltindex];
                robotnévlabel.Text = $"{Robot.megfigyeltindex}. {Robot.megfigyelt.Név}";
                Vektor itt = Robot.megfigyelt.H;
                pozícióXtextbox.Text = $"{itt.X}";
                pozícióYtextbox.Text = $"{itt.Y}";
                időtextbox.Text = $"{idő}";
                hőtextbox.Text = $"{Robot.megfigyelt.Hőmérő()}";
                ultrahangtextbox.Text = $"{Robot.megfigyelt.UltrahangSzenzor()}";
                for (int szín = 2; szín < 7; szín++)
                    kőtextboxok[szín-2].Text = $"{Robot.megfigyelt.Mennyi(szín)}";
                karesznagyításkeret.BackgroundImage = Robot.megfigyelt.Iránykép();
                monitorpanel2.Refresh();
                pálya.Frissít();
            }
        }
    }
}