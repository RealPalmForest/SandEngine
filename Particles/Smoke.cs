using Microsoft.Xna.Framework;
using SandEngine.AbstractParticles;

namespace SandEngine.Particles;

public class Smoke : GasParticle
{
    public Smoke(GameMap parentMap) : base(parentMap)
    {
        Color = Color.Gray;
        DispersionAmount = 5;
        Acceleration = -0.1f;
        MaxVelocity = 1;
    }

    public override void Update()
    {
        DefaultGasBehaviour();
    }
}