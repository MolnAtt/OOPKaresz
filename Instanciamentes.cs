using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karesz
{
	partial class Form1
	{
		#region objektummentes parancsok
		void Lépj() => monitorpanel.megfigyeltrobot.Lép();
		void Fordulj_jobbra() => monitorpanel.megfigyeltrobot.Fordul(jobbra);
		void Fordulj_balra() => monitorpanel.megfigyeltrobot.Fordul(balra);
		void Fordulj(int irány) => monitorpanel.megfigyeltrobot.Fordul(irány);
		int Mennyi(int szín) => monitorpanel.megfigyeltrobot.Mennyi(szín);
		void Vegyél_fel_egy_kavicsot() => monitorpanel.megfigyeltrobot.Felvesz();
		void Tegyél_le_egy_kaviszont(int szín = fekete) => monitorpanel.megfigyeltrobot.Lerak(szín);
		bool Van_e_itt_Kavics() => monitorpanel.megfigyeltrobot.VanKavics();
		int Mi_van_alattam(int ez) => monitorpanel.megfigyeltrobot.MiVanItt();
		bool Van_e_előttem_fal() => monitorpanel.megfigyeltrobot.MiVanElőttem() == fal;
		bool Kilépek_e_a_pályáról() => monitorpanel.megfigyeltrobot.MiVanElőttem() == -1;
		int UltrahangPing() => monitorpanel.megfigyeltrobot.UltrahangSzenzor();
		int Hőmérséklet() => monitorpanel.megfigyeltrobot.Hőmérő();
		#endregion
	}
}
