using AntDesigner.NetCore.Games;
using System;
using AntDesigner.NetCore.Games.GameMajiangDaoDaoHu;
namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            Card card2= new Card(1, "w", "2");
            Card card3 = new Card(2, "w", "3");
            Card card4 = new Card(3, "w", "4");
            Card card5 = new Card(4, "w", "5");
            Card card8 = new Card(5, "w", "6");
            Card card6 = new Card(6, "t", "7");
            Card card7 = new Card(7, "t", "8");
            //Card card6 = new Card(5, "w", "6");
            //Card card7 = new Card(6, "w", "7");
            //Card card8 = new Card(6, "w", "7");
            //Card card9 = new Card(6, "w", "7");
            //Card card10 = new Card(7, "w", "8");
            //Card card11 = new Card(8, "w", "9");

            MaJiangCollection majians = new MaJiangCollection {

                card2,
                card3,
                card4,
                card5,
                card6,
                card7,
                //card8,
                //card9,
                //card10,
                //card11,
            };
            MaJiangCollection huCards = null;
            int n=0;
            DateTime start = DateTime.Now;
            for (int i = 0; i < 10000; i++) {
               huCards = majians.GetHuCards();
                n++;
            }
            DateTime end = DateTime.Now;
            TimeSpan s= end - start;
            Console.WriteLine(n + "次" + s.TotalSeconds + "秒");
            for (int i = 0; i < huCards.Count; i++) {
                Console.WriteLine(huCards[i].Name+"/"+huCards[i].CardColor);
            }
            Console.Read();
        }
    }
}
