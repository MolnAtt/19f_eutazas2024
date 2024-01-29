using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace eutazas
{
	internal class Program
	{

		class Adat
		{
			public int megallo;
			public DateTime datum_felszallas;
			public string id;
			public string tipus;
			public DateTime datum_berlet;
			public int jegyszam;

			public Adat(int megallo, DateTime datum_felszallas, string id, string tipus, DateTime datum_berlet, int jegyszam)
			{
				this.megallo = megallo;
				this.datum_felszallas = datum_felszallas;
				this.id = id;
				this.tipus = tipus;
				this.datum_berlet = datum_berlet;
				this.jegyszam = jegyszam;
			}
		}

		static void Main(string[] args)
		{
			string[] sorok = File.ReadAllLines("utasadat.txt");

			List<Adat> lista = new List<Adat>();

			foreach (string sor in sorok)
			{
				string[] t = sor.Split(' ');

				string tipus = t[3];

				string datumstr = t[1]; // 20190326-0716
				DateTime datum_felszallas = new DateTime(
					int.Parse(t[1].Substring(0, 4)),
					int.Parse(t[1].Substring(4, 2)),
					int.Parse(t[1].Substring(6, 2)),
					int.Parse(t[1].Substring(9, 2)),
					int.Parse(t[1].Substring(11, 2)),
					0);

				if (tipus == "JGY")
				{
					lista.Add(new Adat(int.Parse(t[0]), datum_felszallas, t[2], t[3], new DateTime(), int.Parse(t[4])));
				}
				else
				{
					lista.Add(new Adat(int.Parse(t[0]), datum_felszallas, t[2], t[3], new DateTime(
						int.Parse(t[4].Substring(0, 4)),
						int.Parse(t[4].Substring(4, 2)),
						int.Parse(t[4].Substring(6, 2)),
						23,59,59,1
						), -1));
				}
			}

            Console.WriteLine($"2. balbabla {lista.Count} ");


			Console.WriteLine($"3. feladat: {lista.Count(x => (x.tipus == "JGY" && x.jegyszam == 0) || (x.tipus != "JGY" && x.datum_berlet < x.datum_felszallas))}");


			Megállónként(lista);

        }

		private static void Megállónként(List<Adat> lista)
		{
			// Group by feladat -----> SZÓTÁR

			// megálló : db
			// int : int

			Dictionary<int, int> szotar = new Dictionary<int, int>();

			foreach (Adat adat in lista)
			{
				if (szotar.ContainsKey(adat.megallo))
				{
					szotar[adat.megallo] += 1;
				}
				else
				{
					szotar[adat.megallo]  = 1;
				}
			}

			// valahogy így fog kinézni:
			// 0: 5,
			// 4: 9,
			// 5: 3,
			// ...


			//foreach (int megallo in szotar.Keys)
			//{
			//	Console.WriteLine($"kulcs: {megallo} --> {szotar[megallo]} db");
			//}

			int bestmo = 0;

			foreach (int megallo in szotar.Keys)
			{
				if ( szotar[bestmo] < szotar[megallo])
				{
					bestmo = megallo;
				}
			}


            Console.WriteLine($"A {bestmo} megállóhelyen szálltak fel a legtöbben, mégpedig ennyien: {szotar[bestmo]} fő");


        }
	}
}


