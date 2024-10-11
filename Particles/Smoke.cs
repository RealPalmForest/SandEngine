using Microsoft.Xna.Framework;
using SandEngine.AbstractParticles;

namespace SandEngine.Particles;

public class Smoke : GasParticle
{
    public Smoke(GameMap parentMap) : base(parentMap)
    {
        Color = Color.Gray;
        DispersionAmount = 15;
        Acceleration = -0.1f;
        Gravity = -1;
        MaxVelocity = 1;
    }

    public override void Update()
    {
        DefaultGasBehaviour();
    }
}