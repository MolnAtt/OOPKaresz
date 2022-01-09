using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karesz
{
    partial class Form1
    {
		#region objektummentes parancsok
		private void Lépj() { monitorpanel.megfigyeltrobot.Lép(); }
		private void Fordulj_jobbra() { monitorpanel.megfigyeltrobot.Fordul(jobbra); }
		private void Fordulj_balra() { monitorpanel.megfigyeltrobot.Fordul(balra); }
		private void Fordulj(int irány) { monitorpanel.megfigyeltrobot.Fordul(irány); }
		private int Mennyi(int szín) { return monitorpanel.megfigyeltrobot.Mennyi(szín); }
		private void Vegyél_fel_egy_kavicsot() { monitorpanel.megfigyeltrobot.Felvesz(); }
		private void Tegyél_le_egy_kaviszont(int szín = fekete) { monitorpanel.megfigyeltrobot.Lerak(szín); }
		private bool Van_e_itt_Kavics() { return monitorpanel.megfigyeltrobot.VanKavics(); }
		private int Mi_van_alattam(int ez) { return monitorpanel.megfigyeltrobot.MiVanItt(); }
		private bool Van_e_előttem_fal() { return monitorpanel.megfigyeltrobot.MiVanElőttem() == fal; }
		private bool Kilépek_e_a_pályáról() { return monitorpanel.megfigyeltrobot.MiVanElőttem() == -1; }
		private int UltrahangPing() { return monitorpanel.megfigyeltrobot.UltrahangSzenzor(); }
		private int Hőmérséklet() { return monitorpanel.megfigyeltrobot.Hőmérő(); }
		#endregion
	}
}
