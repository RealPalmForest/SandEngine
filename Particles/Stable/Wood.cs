using Microsoft.Xna.Framework;
using SandEngine.AbstractParticles;

namespace SandEngine.Particles.Stable;

public class Wood : StableParticle
{
    public Wood(GameMap parentMap) : base(parentMap)
    {
        int clr = Globals.Random.Next(3);
        switch (clr)
        {
            case 0:
                Color = new Color(51, 39, 20);
                break;
            case 1:
                Color = new Color(89, 68, 40);
                break;
            case 2:
                Color = new Color(104, 79, 41);
                break;
        }
    }
}