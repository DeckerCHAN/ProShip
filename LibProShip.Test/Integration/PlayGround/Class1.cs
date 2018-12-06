using System.Collections.Generic;

namespace LibProShip.Test.Integration.PlayGround
{
        public interface IA<in T> where T : IB
        {
            void Hold(T ib);
        }
    
        public interface IB
        {
        }
    
        public class BImpl : IB
        {
        }
    
    
        public class AImpl : IA<BImpl>
        {
            private IB b;
    
            public void Hold(BImpl ib)
            {
                this.b = ib;
            }
        }
    
    
        public class Programme
        {
            public static List<dynamic> List= new List<dynamic>();
    
            public static void Main(string[] args)
            {
                var b = new BImpl();
                var a = new AImpl();
                a.Hold(b);
    
                List.Add(a);
                //This is not allowed due to AImpl is not assignable to IA<IB>
            }
        }
}