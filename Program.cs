// Monogram: OR
// Projekt neve: Gokart időpontfoglaló - Egyéni kisprojekt
// Kezdeti dátum: 2025-09-15

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;

class Versenyzo
{
    public string keresztnev;
    public string vezeteknev;
    public DateTime szuletesiDatum;
    public bool nagykoru;
    public string azonosito;
    public string email;
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        string palyaNev = "Mario Kart";
        string palyaCim = "6969 Kukutyin, Rákos utca 7.";
        string palyaTelefon = "+36-30-555-0123";
        string palyaWeb = "mk-gokart.hu";

        Console.WriteLine("====================================");
        Console.WriteLine("Projekt: Gokart időpontfoglaló");
        Console.WriteLine("Dátum: " + DateTime.Today.ToString("yyyy-MM-dd"));
        Console.WriteLine("Pálya: " + palyaNev + " | " + palyaCim);
        Console.WriteLine("Telefon: " + palyaTelefon + " | Web: " + palyaWeb);
        Console.WriteLine("====================================");

        List<string> keresztnevek = new List<string>();
        List<string> vezeteknevek = new List<string>();

        try
        {
            string tartalom1 = File.ReadAllText("keresztnevek.txt", Encoding.UTF8);
            string[] darabok1 = tartalom1.Split(',');
            foreach (string s in darabok1)
            {
                string n = s.Trim().Trim('\'');
                if (n.Length > 0 && !keresztnevek.Contains(n)) keresztnevek.Add(n);
            }

            string tartalom2 = File.ReadAllText("vezeteknevek.txt", Encoding.UTF8);
            string[] darabok2 = tartalom2.Split(',');
            foreach (string s in darabok2)
            {
                string n = s.Trim().Trim('\'');
                if (n.Length > 0 && !vezeteknevek.Contains(n)) vezeteknevek.Add(n);
            }
        }
        catch
        {
            keresztnevek.Add("Anna"); keresztnevek.Add("Péter");
            vezeteknevek.Add("Kovács"); vezeteknevek.Add("Nagy");
        }

        Random rnd = new Random();
        int versenyzoDb = rnd.Next(5, 150);
        List<Versenyzo> versenyzok = new List<Versenyzo>();

        for (int i = 0; i < versenyzoDb; i++)
        {
            string knev = keresztnevek[rnd.Next(1, keresztnevek.Count)];
            string vnev = vezeteknevek[rnd.Next(1, vezeteknevek.Count)];
            int eletkor = rnd.Next(10, 50);
            DateTime szuletes = DateTime.Today.AddYears(-eletkor).AddDays(rnd.Next(0, 365));
            bool felnott = (DateTime.Today.Year - szuletes.Year) >= 18;
            string id = "GO-" + EkezetNelkul(vnev + knev) + "-" + szuletes.ToString("yyyyMMdd");
            string mail = EkezetNelkul(vnev).ToLower() + "." + EkezetNelkul(knev).ToLower() + "@gmail.com";

            Versenyzo v = new Versenyzo();
            v.keresztnev = knev;
            v.vezeteknev = vnev;
            v.szuletesiDatum = szuletes;
            v.nagykoru = felnott;
            v.azonosito = id;
            v.email = mail;
            versenyzok.Add(v);
        }

        List<string> foglalasok = new List<string>();

        Console.WriteLine("\n Versenyzők listája ");
        for (int i = 0; i < versenyzok.Count; i++)
        {
            Versenyzo v = versenyzok[i];
            Console.WriteLine((i + 1) + ". " + v.vezeteknev + " " + v.keresztnev +
                " | " + v.szuletesiDatum.ToString("yyyy.MM.dd") +
                " | 18+: " + (v.nagykoru ? "Igen" : "Nem") +
                " | ID: " + v.azonosito +
                " | Email: " + v.email);
        }

        KiirTabla(foglalasok);

        string valasz;
        do
        {
            Console.Write("\nSzeretne foglalni? (i/n): ");
            valasz = Console.ReadLine();

            if (valasz == "i")
            {
                Console.Write("Adja meg a versenyző ID-ját: ");
                string id = Console.ReadLine();

                bool talalat = false;
                for (int i = 0; i < versenyzok.Count; i++)
                {
                    if (versenyzok[i].azonosito == id) talalat = true;
                }
                if (!talalat) { Console.WriteLine("Nincs ilyen ID."); continue; }

                Console.Write("Adja meg a dátumot (yyyy-MM-dd): ");
                string datumStr = Console.ReadLine();
                DateTime datum = new DateTime();
                if (datumStr.Length != 10 || datumStr[4] != '-' || datumStr[7] != '-')
                {
                    Console.WriteLine("Hibás formátum!");
                }
                else
                {
                    try
                    {
                        int ev = int.Parse(datumStr.Substring(0, 4));
                        int honap = int.Parse(datumStr.Substring(5, 2));
                        int nap = int.Parse(datumStr.Substring(8, 2));

                        datum = new DateTime(ev, honap, nap);
                        Console.WriteLine("A dátum rendben: " + datum.ToString("yyyy-MM-dd"));
                    }
                    catch
                    {
                        Console.WriteLine("Hibás dátumérték!");
                        continue;
                    }
                }


                Console.Write("Adja meg az órát (8-18): ");
                int ora = int.Parse(Console.ReadLine());

                string kulcs = datum.ToString("yyyy-MM-dd") + "-" + ora;
                int db = 0;
                for (int i = 0; i < foglalasok.Count; i++)
                {
                    if (foglalasok[i].StartsWith(kulcs)) db++;
                }

                if (db < 20)
                {
                    foglalasok.Add(kulcs + "|" + id);
                    Console.WriteLine("Foglalás sikeres!");
                }
                else Console.WriteLine("Ez az időpont megtelt.");

                KiirTabla(foglalasok);
            }

        } while (valasz == "i");
    }

    static void KiirTabla(List<string> foglalasok)
    {
        Console.WriteLine("\n Időpontok táblázata  ");
        DateTime ma = DateTime.Today;
        DateTime harminc = new DateTime(ma.Year, ma.Month, DateTime.DaysInMonth(ma.Year, ma.Month));

        Console.Write("Dátum      ");
        for (int ora = 8; ora < 19; ora++)
        {
            Console.Write("|   " + ora.ToString("00") + "-" + (ora + 1).ToString("00"));
        }
        Console.WriteLine("|");

        for (DateTime nap = ma; nap <= harminc; nap = nap.AddDays(1))
        {
            Console.Write(nap.ToString("yyyy-MM-dd") + " ");
            for (int ora = 8; ora < 19; ora++)
            {
                string kulcs = nap.ToString("yyyy-MM-dd") + "-" + ora;
                int db = 0;
                for (int i = 0; i < foglalasok.Count; i++)
                {
                    if (foglalasok[i].StartsWith(kulcs)) db++;
                }

                if (db == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("| Szabad ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("|Foglalt ");
                }
                Console.ResetColor();
            }
            Console.WriteLine("|");
        }
    }


    static string EkezetNelkul(string s)
    {
        string keresett = "áéíóöőúüűÁÉÍÓÖŐÚÜŰ";
        string csere = "aeiooouuuAEIOOOUUU";

        for (int i = 0; i < keresett.Length; i++)
            s = s.Replace(keresett[i], csere[i]);

        return s;
    }
}