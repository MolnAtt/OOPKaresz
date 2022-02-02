using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karesz
{
	partial class Form1
	{
        #region Robotokra visszavezetett parancsok

        void Lépj() => 
			Robot.Megfigyelt.Lép();
		void Fordulj_jobbra() => 
			Robot.Megfigyelt.Fordul(jobbra);
		void Fordulj_balra() => 
			Robot.Megfigyelt.Fordul(balra);
		void Fordulj(int irány) => 
			Robot.Megfigyelt.Fordul(irány);
		int Mennyi(int szín) => 
			Robot.Megfigyelt.Mennyi(szín);
		void Vegyél_fel_egy_kavicsot() => 
			Robot.Megfigyelt.Felvesz();
		void Tegyél_le_egy_kavicsot(int szín = fekete) => 
			Robot.Megfigyelt.Lerak(szín);
		bool Van_e_itt_Kavics() => 
			Robot.Megfigyelt.VanKavics();
		int Mi_van_alattam(int ez) => 
			Robot.Megfigyelt.MiVanItt();
		bool Van_e_előttem_fal() => 
			Robot.Megfigyelt.MiVanElőttem() == fal;
		bool Kilépek_e_a_pályáról() => 
			Robot.Megfigyelt.MiVanElőttem() == -1;
		int UltrahangPing() => 
			Robot.Megfigyelt.UltrahangSzenzor();
		int Hőmérséklet() => 
			Robot.Megfigyelt.Hőmérő();

        #endregion

        #region Pályára visszavezetett parancsok

        void Betölt(string path) => pálya.Betölt(path);

        #endregion
	}
}
