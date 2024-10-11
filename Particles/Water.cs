using Microsoft.Xna.Framework;
using SandEngine.AbstractParticles;

namespace SandEngine.Particles;

public class Water : LiquidParticle
{
    public Water(GameMap parentMap) : base(parentMap)
    {
        DispersionAmount = 9;
        LiquidDensity = 1;

        int darken = Globals.Random.Next(50, 71);
        Color = new Color(
            Color.DodgerBlue.R - darken,
            Color.DodgerBlue.G - darken,
            Color.DodgerBlue.B - darken);
    }

    public override void Update()
    {
        DefaultLiquidBehaviour();
    }
}