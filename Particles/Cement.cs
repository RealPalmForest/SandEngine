using Microsoft.Xna.Framework;
using SandEngine.AbstractParticles;

namespace SandEngine.Particles;

public class Cement : LiquidParticle
{
    public Cement(GameMap parentMap) : base(parentMap)
    {
        DispersionAmount = 1;
        LiquidDensity = 2;

        int darken = Globals.Random.Next(20, 31);
        Color = new Color(
            Color.DarkGray.R - darken,
            Color.DarkGray.G - darken,
            Color.DarkGray.B - darken);
    }

    public override void Update()
    {
        DefaultLiquidBehaviour();

        if (Lifetime > 1000)
        {
            Replace(new Concrete(parentMap));
        }
    }
}