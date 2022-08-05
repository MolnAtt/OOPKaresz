using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;


namespace Karesz
{
    public partial class Form1 : Form
    {
        string betöltendő_pálya = "palya02.txt";

        void TANÁR_ROBOTJAI()
        {
            new Robot("Karesz", new int[]{ 10,10,10,10,0}, 5, 28, 0);
            Robot lilesz = new Robot("Lilesz", new int[] { 0, 0, 0, 0, 0 }, 4, 5, 2);

            lilesz.Feladat = delegate ()
            {
                for (int i = 0; i < 3; i++)
                {
                    lilesz.Lépj();
                    lilesz.Lépj();
                    lilesz.Lépj();
                    lilesz.Lépj();
                    lilesz.Lépj();
                    lilesz.Lépj();
                    lilesz.Fordulj(jobbra);
                }
                // lilesz.Lőjj();
            };

            Robot janesz = new Robot("Janesz", new int[] { 0, 0, 0, 0, 0 }, 14, 5, 2);

            janesz.Feladat = delegate ()
            {
				while (true)
				{
                    janesz.Lépj();
				} 
            };
        }
    }
}