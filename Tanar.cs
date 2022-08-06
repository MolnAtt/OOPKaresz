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
            new Robot("Karesz", 10, 10, 10, 10, 0, 5, 28, 0);


            Robot janesz = new Robot("Janesz", Robot.képkészlet_lilesz, 0, 0, 0, 0, 0, 14, 1, 2);

            janesz.Feladat = delegate ()
            {
                while (true)
                {
                    janesz.Lépj();
                }
            };

            Robot anti = new Robot("Anti", new int[] { 0, 0, 0, 0, 10 }, 14, 20, 0);

            anti.Feladat = delegate ()
            {
				for (int i = 0; i < 4; i++)
				{
                    anti.Lőjj();
                    anti.Fordulj(jobbra);
				}
            };
        }
    }
}