using System.Diagnostics;
using Microsoft.Xna.Framework;
using SandEngine.AbstractParticles;

namespace SandEngine.Particles.Falling;

public class Sand : FallingParticle
{
    public Sand(GameMap parentMap) : base(parentMap)
    {
        DisplaceLiquids = true;

        int clr = Globals.Random.Next(3);
        switch (clr)
        {
            case 0:
                Color = new Color(210, 204, 149);
                break;
            case 1:
                Color = new Color(158, 152, 107);
                break;
            case 2:
                Color = new Color(165, 141, 82);
                break;
        }
    }

    public override void Update()
    {
        DefaultFallingBehaviour();
    }
}