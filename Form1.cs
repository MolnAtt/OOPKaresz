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
			Robot Karesz = new Robot("Karesz", new int[] { 100,20,20,20,0});

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
            startgomb2.Enabled = false;
            monitorpanel.Enged(false);
            FELADAT();
            monitorpanel.Enged(true);
            startgomb2.Enabled = true;
            MessageBox.Show("Vége!");
        }

        void elozorobotgomb_Click(object sender, EventArgs e)
        {
            Robot.Megfigyelt_léptetése(-1);
            monitorpanel.Frissít();
        }
        
        
        void kövtkezőrobotgomb_Click(object sender, EventArgs e)
        {
            Robot.Megfigyelt_léptetése(1);
            monitorpanel.Frissít();
        }

        /// <summary>
        /// pálya betöltése gombbal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void pályagomb_Click(object sender, EventArgs e) => 
            pálya.Betölt(pályaválasztó.Text);

        /// <summary>
        /// Ha valaki a pályára kattint, akkor Kareszt leteszi oda.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void képkeret_MouseDown(object sender, MouseEventArgs e)
        {
            if (Robot.lista.Count>0)
            {
                monitorpanel.megfigyeltrobot.Teleport(e.X / pálya.lépték.X, e.Y / pálya.lépték.Y);
                képkeret.Refresh();
                monitorpanel.Frissít();
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
                Vektor itt = Robot.megfigyelt.HolVan();
                pozícióXtextbox.Text = $"{itt.X}";
                pozícióYtextbox.Text = $"{itt.Y}";
                időtextbox.Text = $"{idő}";
                hőtextbox.Text = $"{Robot.megfigyelt.Hőmérő()}";
                ultrahangtextbox.Text = $"{Robot.megfigyelt.UltrahangSzenzor()}";
                for (int szín = 2; szín < 7; szín++)
                    kőtextboxok[szín-2].Text = $"{Robot.megfigyelt.Mennyi(szín)}";
                karesznagyításkeret.BackgroundImage = Robot.megfigyelt.Iránykép();
                monitorpanel2.Refresh();
            }
        }
    }
}