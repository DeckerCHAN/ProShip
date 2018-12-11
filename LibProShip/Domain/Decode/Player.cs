using System.Collections.Generic;

namespace LibProShip.Domain.Decode
{
    public class Player : ValueObject<Player>
    {
        public string Name { get; private set; }

        public Player()
        {
        }

        public Player(long id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        
    }
}