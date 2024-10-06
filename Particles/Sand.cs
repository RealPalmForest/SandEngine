using System.Diagnostics;
using Microsoft.Xna.Framework;
using SandEngine.AbstractParticles;

namespace SandEngine.Particles;

public class Sand : FallingParticle
{
    public Sand(GameMap parentMap) : base(parentMap)
    {
        DisplaceLiquids = true;
        Color = Color.Goldenrod;
    }

    public override void Update()
    {
        DefaultFallingBehaviour();
    }
}