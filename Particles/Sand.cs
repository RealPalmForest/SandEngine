using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace SandEngine.Particles;

public class Sand : Particle
{
    public Sand(GameMap parentMap) : base(parentMap) 
    { 
        DisplaceLiquids = true;
        Color = Color.Goldenrod;
    }

    public override void Update()
    {
        SandUpdate();
    }
}