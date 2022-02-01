using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;
namespace Karesz
{
	partial class Form1
	{
		class MonitorPanel : Panel
		{
			private int megfigyeltrobotindex = 0;
			public Robot megfigyeltrobot;
			Button Robotváltó = new Button
			{
				Location = new Point(20, 20),
				Margin = new Padding(4, 4, 4, 4),
				Name = "Robotváltó",
				TabIndex = 28,
				Text = "Robotváltás",
				UseVisualStyleBackColor = true
			};
			Label robotcímke = new Label
			{
				AutoSize = true,
				Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(238))),
				Location = new Point(20, 70),
				Margin = new Padding(4, 0, 4, 0),
				Name = "robotcímke",
				Size = new Size(132, 25),
				TabIndex = 10,
				Text = "Robot adatai"
			};
			Label koordináták = new Label
			{
				AutoSize = true,
				Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(238))),
				Location = new Point(5, 150),
				Margin = new Padding(4, 0, 4, 0),
				Name = "koordináták",
				Size = new Size(18, 20),
				TabIndex = 13,
				Text = "Koordinátái: (5;28)"
			};
			Label idől = new Label
			{
				AutoSize = true,
				Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(238))),
				Location = new Point(5, 190),
				Margin = new Padding(4, 0, 4, 0),
				Name = "idől",
				Size = new Size(18, 20),
				TabIndex = 14,
				Text = "Eltelt idő: 0 s"
			};
			Label hől = new Label
			{
				AutoSize = true,
				Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(238))),
				Location = new Point(5, 170),
				Margin = new Padding(4, 0, 4, 0),
				Name = "idől",
				Size = new Size(18, 20),
				TabIndex = 14,
				Text = "Hőmérséklet: "
			};
			Label kőszámfelirat = new Label
			{
				AutoSize = true,
				Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(238))),
				Location = new Point(25, 350),
				Margin = new Padding(4, 0, 4, 0),
				Name = "label7",
				Size = new Size(111, 20),
				TabIndex = 15,
				Text = "Zsebeiben lévő \nkövek száma:",
			};
			public TextBox[] kőmutatók = new TextBox[5]
				{
					new TextBox{ Name = "feketedb" },
					new TextBox{ Name = "pirosdb" },
					new TextBox{ Name = "zölddb" },
					new TextBox{ Name = "sárgadb" },
					new TextBox{ Name = "hódb" }
				};
			Label[] kőfeliratok = new Label[5]
				{
					new Label{ Name="fekete", Text = "fekete", ForeColor = Color.Black},
					new Label{ Name="piros", Text = "piros", ForeColor = Color.Red},
					new Label{ Name="zöld", Text = "zöld", ForeColor = Color.Green},
					new Label{ Name="sárga", Text = "sárga", ForeColor = Color.Yellow},
					new Label{ Name="hógolyó", Text = "hógolyó", ForeColor = Color.White}
				};
			PictureBox NagyítottKép = new PictureBox
			{
				BackgroundImage = global::Karesz.Properties.Resources.Karesz0,
				BackgroundImageLayout = ImageLayout.Stretch,
				Location = new Point(20, 220),
				Margin = new Padding(4, 4, 4, 4),
				Name = "NagyítottKép",
				Size = new Size(120, 120),
				TabIndex = 27,
				TabStop = false,
			};

			public MonitorPanel()
			{

				BackColor = SystemColors.ControlDark;
				Location = new Point(pálya.Size.Width + 20, 70);
				Margin = new Padding(4, 4, 4, 4);
				Name = "monitorpanel";
				Size = new Size(160, pálya.Size.Height - 130);
				Robotváltó.Size = new Size(Size.Width - 40, 30);
				TabIndex = 13;
				koordináták.Location = new Point(10, robotcímke.Location.Y + 40);
				idől.Location = new Point(koordináták.Location.X, koordináták.Location.Y + 30);
				hől.Location = new Point(koordináták.Location.X, idől.Location.Y + 30);


				Controls.Add(Robotváltó);
				Controls.Add(koordináták);
				Controls.Add(idől);
				Controls.Add(hől);
				Controls.Add(kőszámfelirat);
				Controls.Add(robotcímke);
				Controls.Add(NagyítottKép);
				((System.ComponentModel.ISupportInitialize)(NagyítottKép)).BeginInit();
				((System.ComponentModel.ISupportInitialize)(NagyítottKép)).EndInit();




				for (int i = 0; i < 5; i++)
				{
					// Labelek
					kőfeliratok[i].AutoSize = true;
					kőfeliratok[i].Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(238)));
					kőfeliratok[i].Location = new Point(10, kőszámfelirat.Location.Y + 60 + i * 40);    // ezért forciklus!
					kőfeliratok[i].Margin = new Padding(4, 0, 4, 0);
					kőfeliratok[i].Size = new Size(54, 20);
					kőfeliratok[i].TabIndex = 16;
					Controls.Add(kőfeliratok[i]);

					// Textboxok
					kőmutatók[i].Location = new Point(90, kőszámfelirat.Location.Y + 60 + i * 40);  // ezért forciklus!
					kőmutatók[i].Margin = new Padding(4, 4, 4, 4);
					kőmutatók[i].Name = "feketedb";
					kőmutatók[i].Size = new Size(64, 22);
					kőmutatók[i].TabIndex = 20;
					kőmutatók[i].Text = "10";
					Controls.Add(kőmutatók[i]);
				}
				NagyítottKép.Click += new System.EventHandler(NagyítottKép_Click);
				Robotváltó.Click += new System.EventHandler(Robotváltó_Click);
			}
			private void Robotváltó_Click(object sender, EventArgs e)
			{
				megfigyeltrobotindex = (megfigyeltrobotindex + 1) % Robot.lista.Count;
				megfigyeltrobot = Robot.lista[megfigyeltrobotindex];
				monitorpanel.Frissít();
			}
			void NagyítottKép_Click(object sender, EventArgs e) => megfigyeltrobot.Fordul(jobbra);
			public void Enged(bool szabade) { foreach (TextBox szövegdoboz in kőmutatók) szövegdoboz.Enabled = szabade;  }
			public void Frissít()
			{
				if (Robot.lista.Count > 0)
				{
					megfigyeltrobot = Robot.lista[megfigyeltrobotindex];
					robotcímke.Text = $"{megfigyeltrobot.Név} adatai";
					koordináták.Text = $"Pozíció: ({megfigyeltrobot.HolVan().X};{megfigyeltrobot.HolVan().Y})";
					idől.Text = $"Eltelt idő: {óra.GetIdő()}";
					hől.Text = $"Hőmérséklet: {megfigyeltrobot.Hőmérő()}";
					for (int szín = 0; szín < 5; szín++)
						kőmutatók[szín].Text = megfigyeltrobot.Mennyi(2 + szín).ToString(); 
					NagyítottKép.BackgroundImage = megfigyeltrobot.Iránykép();
					Refresh();
				}
			}
		}
	}
}