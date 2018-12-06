using System.Collections.Generic;

namespace LibProShip.Domain.Decode
{
    public class Player :IValueObject<Player>
    {
        public string Id { get; set; }
        public string Name{ get; set; }
        public ICollection<string> Tags{ get; set; }
        public bool SameAs(Player other)
        {
            throw new System.NotImplementedException();
        }
    }
}