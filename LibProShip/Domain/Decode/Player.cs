using System.Collections.Generic;

namespace LibProShip.Domain.Decode
{
    public class Player :IValueObject<Player>
    {
        public string Id { get;  }
        public string Name{ get; }
        public bool SameAs(Player other)
        {
            throw new System.NotImplementedException();
        }
    }
}