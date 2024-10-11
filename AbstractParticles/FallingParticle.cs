using System.Diagnostics;
using System.Formats.Asn1;

namespace SandEngine.AbstractParticles;

public abstract class FallingParticle : MovingParticle
{
    public float InertialResistance { get; protected set; } = 0.1f;
    public bool IsFreeFalling { get; set; } = true;

    public FallingParticle(GameMap parentMap) : base(parentMap)
    {
        IsFreeFalling = Globals.Random.NextDouble() > InertialResistance;
    }

    /// <summary>
    /// Attempts to move particle at position down, left-down, right-down
    /// </summary>
    /// <returns>Returns true if particle was successfully moved</returns>
    protected virtual bool DefaultFallingBehaviour()
    {
        int oldX = X;
        int oldY = Y;
        if (!UpdateVerticalMovement())
        {
            if ((GetBelow() is LiquidParticle && DisplaceLiquids) || GetBelow() is GasParticle)
                Displace(X, Y + 1);
            else if (IsFreeFalling)
            {
                if (GetLeft() == null && GetBelowLeft() == null)
                    Move(X - 1, Y + 1);
                else if (GetLeft() is LiquidParticle && GetBelowLeft() is LiquidParticle && DisplaceLiquids)
                    Displace(X - 1, Y + 1);
                else if (GetRight() == null && GetBelowRight() == null)
                    Move(X + 1, Y + 1);
                else if (GetRight() is LiquidParticle && GetBelowRight() is LiquidParticle && DisplaceLiquids)
                    Displace(X + 1, Y + 1);
            }
            else return false;
        }

        if (oldX == X && oldY == Y) IsFreeFalling = false;
        else
        {
            // Update IsFreeFalling for all touched particles
            for (int y = 0; y <= Y - oldY; y++)
            {
                foreach (Particle particle in parentMap.GetParticlesAround(X, oldY + y))
                {
                    if (particle != null && particle is FallingParticle)
                    {
                        (particle as FallingParticle).IsFreeFalling = Globals.Random.NextDouble() > (particle as FallingParticle).InertialResistance;
                    }
                }
            }
        }

        return true;
    }
}