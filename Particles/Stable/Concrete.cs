using Microsoft.Xna.Framework;
using SandEngine.AbstractParticles;

namespace SandEngine.Particles.Stable;

public class Concrete : StableParticle
{
    public Concrete(GameMap parentMap) : base(parentMap)
    {
        int darken = Globals.Random.Next(50, 81);
        Color = new Color(
            Color.DarkGray.R - darken,
            Color.DarkGray.G - darken,
            Color.DarkGray.B - darken);
    }
}