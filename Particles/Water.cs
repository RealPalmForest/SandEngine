using Microsoft.Xna.Framework;
using SandEngine.AbstractParticles;

namespace SandEngine.Particles;

public class Water : LiquidParticle
{
    public Water(GameMap parentMap) : base(parentMap)
    {
        Color = Color.DodgerBlue;
        DispersionAmount = 7;
    }

    public override void Update()
    {
        DefaultLiquidBehaviour();
    }
}