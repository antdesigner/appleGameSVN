using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AntDesigner.GameCityBase.boxs
{ 
    public class BoxsManager
    {
        private Collection<Box> BoxsCollection { get; set; }
        private Collection<Box> HitBoxsCollection { get; }
        private Random randoem = new Random();
        private bool change = false;
        // public static Action<decimal, string> addPlayerAccount;
        //   public static Action<decimal, string> deductPlayerAccount;
         public Action<decimal, string> addPlayerAccount;
           public Action<decimal, string> deductPlayerAccount;
        public static string Degree{get;set;}
        public BoxsManager()
        {
            string thisFullName = this.GetType().FullName;
          //  string assemblyName = this.GetType().
            Assembly assembly = Assembly.Load(new AssemblyName("GameTiger"));
            ABboxsBuilder  builder = (ABboxsBuilder)assembly.CreateInstance(thisFullName + "+"+Degree);//反射内部类用"+"号
            Director director = new Director();
            BoxsCollection = director.BuilBoxs(builder);
            //boxsCollection = new Collection<Box>();

            //boxsCollection.Add(new Box("OrangeBig1", 5, 4));
            //boxsCollection.Add(new Box("BellBig1", 5, 4));
            //boxsCollection.Add(new Box("KingBig", 50, 1));
            //boxsCollection.Add(new Box("KingSmall", 25, 1));
            //boxsCollection.Add(new Box("AppleBig1", 5, 5));
            //boxsCollection.Add(new Box("AppleSmall", 1, 10));
            //boxsCollection.Add(new Box("OliveBig1", 5, 4));
            //boxsCollection.Add(new Box("WatermelonSmall", 3, 5));
            //boxsCollection.Add(new Box("WatermelonBig", 10, 4));
            //boxsCollection.Add(new Box("ChangeSmall", 1, 1));
            //boxsCollection.Add(new Box("AppleBig2", 5, 5));
            //boxsCollection.Add(new Box("OrangeSmall", 3, 6));
            //boxsCollection.Add(new Box("BellBig2", 5, 4));
            //boxsCollection.Add(new Box("OrangeBig2", 5, 4));
            //boxsCollection.Add(new Box("DoubleSevenSmall", 3, 6));
            //boxsCollection.Add(new Box("DoubleSevenBig", 10, 1));
            //boxsCollection.Add(new Box("AppleBig3", 5, 5));
            //boxsCollection.Add(new Box("OliveSmall", 3, 5));
            //boxsCollection.Add(new Box("OliveBig2", 5, 4));
            //boxsCollection.Add(new Box("StarBig", 10, 2));
            //boxsCollection.Add(new Box("StarSmall", 3, 7));
            //boxsCollection.Add(new Box("ChangeBig", 1, 1));
            //boxsCollection.Add(new Box("AppleBig4", 5, 5));
            //boxsCollection.Add(new Box("BellSmall", 3, 6));

            HitBoxsCollection = new Collection<Box>();
        }
        public Collection<Box> WinningResult(List<StakeBox> stakeBoxs)
        {
            PutRandomBoxIntoHitBoxs();
            Collection<Box> winningResult = Comput(stakeBoxs);
            decimal losting = stakeBoxs.Sum(p => p.Stake);
            deductPlayerAccount(-(losting / 10), "下注");
            decimal winning = winningResult.Sum(p => p.Prize);
            if (winning > 0)
            {
                addPlayerAccount((winning / 10), "赢");
            }

            return winningResult;
        }
        private void PutRandomBoxIntoHitBoxs()
        {
            int hit = randoem.Next(1, 100);
            int n = 0;

            foreach (var box in BoxsCollection)
            {

                if ((box.WeightCoefficient + n) >= hit)
                {
                    if (
                  (box.Name == "ChangeBig")
                  ||
                  (box.Name == "ChangeSmall")
                  )
                    {
                        change = true;
                        PutRandomBoxIntoHitBoxs();
                        return;
                    }
                    if (HitBoxsCollection.Contains(box))
                    {
                        continue;
                    }
                    HitBoxsCollection.Add(box);
                    if (change == true)
                    {
                        PutRandomBoxIntoHitBoxs();
                        change = false;
                    }
                    break;

                }
                else
                {
                    n = n + box.WeightCoefficient;
                }
            }
            return;
        }
        private Collection<Box> Comput(List<StakeBox> stakeBoxs)
        {

            foreach (var stakeBox in stakeBoxs)
            {
                foreach (var box in HitBoxsCollection)
                {
                    if (box.Name.Contains(stakeBox.Name))
                    {
                        box.Prize = stakeBox.Stake * box.Odds;
                    }
                }
            }

            return HitBoxsCollection;
        }
        abstract class ABboxsBuilder
        {
           internal  abstract  Collection<Box> BuildBox();
        }
        class BuilderNormal : ABboxsBuilder
        {
            internal  override Collection<Box> BuildBox()
            {
                Collection<Box> boxsCollection = new Collection<Box>
                {
                    new Box("OrangeBig1", 5, 4),
                    new Box("BellBig1", 5, 4),
                    new Box("KingBig", 50, 1),
                    new Box("KingSmall", 25, 1),
                    new Box("AppleBig1", 5, 3),
                    new Box("AppleSmall", 1, 11),
                    new Box("OliveBig1", 5, 4),
                    new Box("WatermelonSmall", 3, 7),
                    new Box("WatermelonBig", 10, 4),
                    new Box("ChangeSmall", 1, 1),
                    new Box("AppleBig2", 5, 3),
                    new Box("OrangeSmall", 3, 7),
                    new Box("BellBig2", 5, 4),
                    new Box("OrangeBig2", 5, 4),
                    new Box("DoubleSevenSmall", 3, 7),
                    new Box("DoubleSevenBig", 10, 1),
                    new Box("AppleBig3", 5, 3),
                    new Box("OliveSmall", 3, 7),
                    new Box("OliveBig2", 5, 4),
                    new Box("StarBig", 10, 2),
                    new Box("StarSmall", 3, 7),
                    new Box("ChangeBig", 1, 1),
                    new Box("AppleBig4", 5, 3),
                    new Box("BellSmall", 3, 7)
                };
                return boxsCollection;
            }
        }
        class BuilderHard : ABboxsBuilder
        {
         internal override Collection<Box> BuildBox()
            {
                Collection<Box> boxsCollection = new Collection<Box>
                {
                    new Box("OrangeBig1", 5, 4),
                    new Box("BellBig1", 5, 4),
                    new Box("KingBig", 50, 1),
                    new Box("KingSmall", 25, 1),
                    new Box("AppleBig1", 5, 2),
                    new Box("AppleSmall", 1, 15),
                    new Box("OliveBig1", 5, 4),
                    new Box("WatermelonSmall", 3, 8),
                    new Box("WatermelonBig", 10, 3),
                    new Box("ChangeSmall", 1, 1),
                    new Box("AppleBig2", 5, 2),
                    new Box("OrangeSmall", 3, 8),
                    new Box("BellBig2", 5, 4),
                    new Box("OrangeBig2", 5, 3),
                    new Box("DoubleSevenSmall", 3, 7),
                    new Box("DoubleSevenBig", 10, 1),
                    new Box("AppleBig3", 5, 2),
                    new Box("OliveSmall", 3, 8),
                    new Box("OliveBig2", 5, 3),
                    new Box("StarBig", 10, 1),
                    new Box("StarSmall", 3, 8),
                    new Box("ChangeBig", 1, 1),
                    new Box("AppleBig4", 5, 2),
                    new Box("BellSmall", 3, 7)
                };
                return boxsCollection;
            }
        }
        class BuilderEasy : ABboxsBuilder
        {
          internal override Collection<Box> BuildBox()
            {
                Collection<Box> boxsCollection = new Collection<Box>
                {
                    new Box("OrangeBig1", 5, 4),
                    new Box("BellBig1", 5, 3),
                    new Box("KingBig", 50, 1),
                    new Box("KingSmall", 25, 2),
                    new Box("AppleBig1", 5, 3),
                    new Box("AppleSmall", 1, 12),
                    new Box("OliveBig1", 5, 4),
                    new Box("WatermelonSmall", 3, 5),
                    new Box("WatermelonBig", 10, 4),
                    new Box("ChangeSmall", 1, 1),
                    new Box("AppleBig2", 5, 5),
                    new Box("OrangeSmall", 3, 6),
                    new Box("BellBig2", 5, 4),
                    new Box("OrangeBig2", 5, 4),
                    new Box("DoubleSevenSmall", 3, 5),
                    new Box("DoubleSevenBig", 10, 2),
                    new Box("AppleBig3", 5, 5),
                    new Box("OliveSmall", 3, 4),
                    new Box("OliveBig2", 5, 5),
                    new Box("StarBig", 10, 3),
                    new Box("StarSmall", 3, 6),
                    new Box("ChangeBig", 1, 2),
                    new Box("AppleBig4", 5, 4),
                    new Box("BellSmall", 3, 6)
                };
                return boxsCollection;
            }
        }
        class Director
        {
           
          internal  Collection<Box>  BuilBoxs(ABboxsBuilder builder)
            {
                return builder.BuildBox();
            }
        }
    }
}
