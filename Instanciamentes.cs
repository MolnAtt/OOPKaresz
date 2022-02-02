using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karesz
{
	partial class Form1
	{
		#region objektummentes parancsok
		void Lépj() => Robot.megfigyelt.Lép();
		void Fordulj_jobbra() => Robot.megfigyelt.Fordul(jobbra);
		void Fordulj_balra() => Robot.megfigyelt.Fordul(balra);
		void Fordulj(int irány) => Robot.megfigyelt.Fordul(irány);
		int Mennyi(int szín) => Robot.megfigyelt.Mennyi(szín);
		void Vegyél_fel_egy_kavicsot() => Robot.megfigyelt.Felvesz();
		void Tegyél_le_egy_kaviszont(int szín = fekete) => Robot.megfigyelt.Lerak(szín);
		bool Van_e_itt_Kavics() => Robot.megfigyelt.VanKavics();
		int Mi_van_alattam(int ez) => Robot.megfigyelt.MiVanItt();
		bool Van_e_előttem_fal() => Robot.megfigyelt.MiVanElőttem() == fal;
		bool Kilépek_e_a_pályáról() => Robot.megfigyelt.MiVanElőttem() == -1;
		int UltrahangPing() => Robot.megfigyelt.UltrahangSzenzor();
		int Hőmérséklet() => Robot.megfigyelt.Hőmérő();
		#endregion
	}
}
