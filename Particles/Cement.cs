using Microsoft.Xna.Framework;
using SandEngine.AbstractParticles;

namespace SandEngine.Particles;

public class Cement : LiquidParticle
{
    public Cement(GameMap parentMap) : base(parentMap)
    {
        DispersionAmount = 1;
        LiquidDensity = 3;

        int darken = Globals.Random.Next(20, 41);
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
            // If this particle as at the very bottom, solidify
            if (!parentMap.IsInBounds(X, Y + 1))
                Solidify();
            // If there is a different particle below, solidify
            else if (GetBelow() != null && GetBelow().GetType() != this.GetType())
                Solidify();
        }
    }

    public void Solidify()
    {
        Replace(new Concrete(parentMap));
    }
}