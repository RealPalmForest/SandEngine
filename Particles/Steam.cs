using Microsoft.Xna.Framework;

namespace SandEngine.Particles;

public class Steam : Particle
{
    public Steam(GameMap parentMap) : base(parentMap) 
    { 
        Color = Color.Gray; 
        IsGas = true;
        DispersionAmount = 5;
        Acceleration = -0.1f;
    }

    public override void Update()
    {
        LiquidUpdate();
    }
}