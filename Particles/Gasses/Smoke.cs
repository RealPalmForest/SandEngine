using Microsoft.Xna.Framework;
using SandEngine.AbstractParticles;

namespace SandEngine.Particles.Gasses;

public class Smoke : GasParticle
{
    public Smoke(GameMap parentMap) : base(parentMap)
    {
        int darken = Globals.Random.Next(20, 41);
        Color = new Color(
            Color.Gray.R - darken,
            Color.Gray.G - darken,
            Color.Gray.B - darken);

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