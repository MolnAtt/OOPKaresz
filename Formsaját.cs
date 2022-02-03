using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
//using System.Reflection;
using System.Threading;

namespace Karesz
{
    partial class Form1
    {

        #region Konstansok

        const int várakozás = 5;

        //pályatartalom
        const int üres = 0;
        const int fal = 1;
        const int nincs_pálya = -1;
        const int láva = 7;

        //forgásirány
        const int jobbra = 1;
        const int balra = -1;

        //színek				
        const int fekete = 2;
        const int piros = 3;
        const int zöld = 4;
        const int sárga = 5;
        const int hó = 6;
        const int víz = 8;

        #endregion

        #region Statikus változók

        static int idő = 0;
        static int[] színkódok = new int[] { üres, fal, fekete, piros, zöld, sárga, hó, láva, víz };
        static string[] színnév = new string[] { "üres", "fal", "fekete", "piros", "zöld", "sárga", "hó", "láva", "víz" };
        static Color[] színek = { Color.White, Color.Brown, Color.Black, Color.Red, Color.Green, Color.Yellow, Color.White, Color.Orange, Color.Blue };
        static Random véletlen = new Random();

        #endregion

        #region Form tulajdonságai

        TextBox[] textboxok;
        TextBox[] kőtextboxok;
        Button[] gombok;
        Pálya pálya;
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
        #endregion

        #region Metódusok

        void Saját_InitializeComponent()
        {
            textboxok = new TextBox[]
            {
                időtextbox,
                pozícióXtextbox,
                pozícióYtextbox,
                hőtextbox,
                ultrahangtextbox,
                mivanitttextbox,
                feketetextbox,
                pirostextbox,
                zöldtextbox,
                sárgatextbox,
                hótextbox,
                pályatextbox
            };
            kőtextboxok = new TextBox[]
            {
                feketetextbox,
                pirostextbox,
                zöldtextbox,
                sárgatextbox,
                hótextbox
            };
            gombok = new Button[]
            {
                startgomb2,
                következőrobotgomb,
                elozorobotgomb,
                pályagomb,
            };
            pálya = new Pálya(this);
            pálya.Betölt();
            karesznagyításkeret.BackgroundImageLayout = ImageLayout.Stretch;
            foreach (TextBox textbox in textboxok.Where(t => t != pályatextbox))
                textbox.Enter += (s, e) => { textbox.Parent.Focus(); };
            if (van_karesz)
                new Robot("Karesz", new int[] { 100, 20, 20, 20, 10 }, this);
            if (betöltendő_pálya!="")
                Betölt(betöltendő_pálya);
        }

        void Frissít()
        {
            if (Robot.lista.Count > 0)
            {
                //Robot.megfigyelt = Robot.lista[Robot.megfigyeltindex];
                robotnévlabel.Text = $"{Robot.megfigyeltindex}. {Robot.Megfigyelt.Név}";
                Vektor itt = Robot.Megfigyelt.H;
                pozícióXtextbox.Text = $"{itt.X}";
                pozícióYtextbox.Text = $"{itt.Y}";
                időtextbox.Text = $"{idő}";
                hőtextbox.Text = $"{Robot.Megfigyelt.Hőmérő()}";
                ultrahangtextbox.Text = $"{Robot.Megfigyelt.UltrahangSzenzor()}";
                for (int szín = 2; szín < 7; szín++)
                    kőtextboxok[szín - 2].Text = $"{Robot.Megfigyelt.Mennyi(szín)}";
                karesznagyításkeret.BackgroundImage = Robot.Megfigyelt.Iránykép();
                mivanitttextbox.Text = színnév[Robot.Megfigyelt.MiVanItt()];
                monitorpanel2.Refresh();
                képkeret.Refresh();
                mivanalattamnagyításkeret.Refresh();
            }
        }

        #endregion

        #region Eseménykezelés

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
        /// <summary>
        /// Az előző robotra vált
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void elozorobotgomb_Click(object sender, EventArgs e)
        {
            Robot.Megfigyelt_léptetése(-1);
            Frissít();
        }
        /// <summary>
        /// A következő robotra vált
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void következőrobotgomb_Click(object sender, EventArgs e)
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
            if (Robot.lista.Count > 0)
            {
                Robot.Megfigyelt.Teleport(e.X / pálya.L.X, e.Y / pálya.L.Y);
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
            pálya.Rajz(e);
        void mivanalattamnagyításkeret_Paint(object sender, PaintEventArgs e)
        {
            if (0 < Robot.lista.Count)
                pálya.AlakRajz(Robot.Megfigyelt.MiVanItt(), e, 0, 0, new Vektor(mivanalattamnagyításkeret.Width - 2, mivanalattamnagyításkeret.Height - 2));
        }
        void karesznagyításkeret_Click(object sender, EventArgs e) =>
            Robot.Megfigyelt.Fordul(jobbra);
        void pályatextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                pályagomb.PerformClick();
        }

        #endregion
    }
}
