using Microsoft.Xna.Framework;
using SandEngine.AbstractParticles;

namespace SandEngine.Particles;

public class Water : LiquidParticle
{
    public Water(GameMap parentMap) : base(parentMap)
    {
        Color = Color.DodgerBlue;
        DispersionAmount = 9;
    }

    public override void Update()
    {
        DefaultLiquidBehaviour();
    }
}