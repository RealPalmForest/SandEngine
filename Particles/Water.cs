using Microsoft.Xna.Framework;

namespace SandEngine.Particles;

public class Water : Particle
{
    public Water(GameMap parentMap) : base(parentMap) 
    { 
        Color = Color.DodgerBlue; 
        IsLiquid = true;
        DispersionAmount = 7;
    }

    public override void Update()
    {
        LiquidUpdate();
    }
}