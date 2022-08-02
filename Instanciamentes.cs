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
			Robot.akit_kiválasztottak.Lép();
		/*void Fordulj_jobbra() => 
			Robot.Megfigyelt.Fordul(jobbra);
		void Fordulj_balra() => 
			Robot.Megfigyelt.Fordul(balra);
		*/
		void Fordulj(int irány) => 
			Robot.akit_kiválasztottak.Fordul(irány);
		int Mennyi(int szín) => 
			Robot.akit_kiválasztottak.Mennyi(szín);
		void Vegyél_fel_egy_kavicsot() => 
			Robot.akit_kiválasztottak.Felvesz();
		void Tegyél_le_egy_kavicsot(int szín = fekete) => 
			Robot.akit_kiválasztottak.Lerak(szín);
		bool Van_e_itt_Kavics() => 
			Robot.akit_kiválasztottak.VanKavics();
		int Mi_van_alattam(int ez) => 
			Robot.akit_kiválasztottak.MiVanItt();
		bool Van_e_előttem_fal() => 
			Robot.akit_kiválasztottak.MiVanElőttem() == fal;
		bool Kilépek_e_a_pályáról() => 
			Robot.akit_kiválasztottak.MiVanElőttem() == -1;
		int Ultrahang() => 
			Robot.akit_kiválasztottak.UltrahangSzenzor();
		int Hőmérséklet() => 
			Robot.akit_kiválasztottak.Hőmérő();

        #endregion

        #region Pályára visszavezetett parancsok

        void Betölt(string path) => pálya.Betölt(path);

        #endregion
	}
}
