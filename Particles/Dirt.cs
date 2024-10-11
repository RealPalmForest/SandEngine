using System.Diagnostics;
using Microsoft.Xna.Framework;
using SandEngine.AbstractParticles;

namespace SandEngine.Particles;

public class Dirt : FallingParticle
{
    public Dirt(GameMap parentMap) : base(parentMap)
    {
        DisplaceLiquids = true;
        Color = new Color(
            Color.SaddleBrown.R - Globals.Random.Next(50, 71),
            Color.SaddleBrown.G - Globals.Random.Next(30, 51),
            Color.SaddleBrown.B - Globals.Random.Next(30, 51));
        InertialResistance = 0.35f;
    }

    public override void Update()
    {
        DefaultFallingBehaviour();
    }
}