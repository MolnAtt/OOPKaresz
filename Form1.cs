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

        void ROBOTOK_LÉTREHOZÁSA()
        {
            new Robot("Karesz", this, 5, 28, 0);
            new Robot("Lilesz", this, 4, 5, 2);
        }

        void FELADAT()
        {
            Robot lilesz = Robot.Get("Lilesz");
            Robot karesz = Robot.Get("Karesz");


            karesz.Lép();

            lilesz.Fordul(jobbra);
        }
    }
}
/* LEGFONTOSABB PARANCSOK

karesz.Lép();                  -------- Karesz előre lép egyet.
Fordulj(jobbra);               -------- Karesz jobbra fordul.
Fordulj(balra);                -------- Karesz balra fordul.
Tegyél_le_egy_kavicsot();      -------- Karesz letesz egy fekete kavicsot
Tegyél_le_egy_kavicsot(piros); -------- Karesz letesz egy piros kavicsot.

Van_e_előttem_fal();           -------- igaz, ha Karesz fal előtt áll, egyébként hamis.
Kilépek_e_a_pályáról();        -------- igaz, ha Karesz a pálya szélén kifele néz, egyébként hamis.
Van_e_itt_Kavics();            -------- igaz, ha Karesz épp kavicson áll, egyébként hamis.
*/