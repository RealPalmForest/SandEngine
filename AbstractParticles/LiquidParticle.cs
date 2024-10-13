using System;
using System.Diagnostics;

namespace SandEngine.AbstractParticles;

public abstract class LiquidParticle : MovingParticle
{

    /// <summary>
    /// Determines which liquids float above others, and which switch
    /// Lower density rises above higher density liquids
    /// </summary>
    public float LiquidDensity { get; protected set; } = 1;

    public LiquidParticle(GameMap parentMap) : base(parentMap) { }


    /// <summary>
    /// Attempts to move particle at position down, left-down, right-down, left, right
    /// </summary>
    /// <returns>Returns true if particle was successfully moved</returns>
    protected virtual bool DefaultLiquidBehaviour()
    {
        if (!UpdateVerticalMovement())
        {
            // If there is another liquid particle below, and it's less dense than this one, displace
            if (GetBelow() is LiquidParticle && (GetBelow() as LiquidParticle).LiquidDensity < LiquidDensity)
            {
                Displace(X, Y + 1);
                return true;
            }
            // Liquids fall through gasses
            else if (GetBelow() is GasParticle)
            {
                Displace(X, Y + 1);
                return true;
            }

            // Check for below-left and below-right for liquids to displace
            if (GetBelowLeft() is LiquidParticle && (GetBelowLeft() as LiquidParticle).LiquidDensity < LiquidDensity)
                SwapWith(X - 1, Y + 1);
            else if (GetBelowRight() is LiquidParticle && (GetBelowRight() as LiquidParticle).LiquidDensity < LiquidDensity)
                SwapWith(X + 1, Y + 1);
            // Check for normal below-left and below-right movement
            else if (GetLeft() == null && GetBelowLeft() == null)
                Move(X - 1, Y + 1);
            else if (GetRight() == null && GetBelowRight() == null)
                Move(X + 1, Y + 1);

            if (/*Lifetime % 2 == 0*/Globals.Random.Next(2) == 0)
                DisperseRight(DispersionAmount);
            else
                DisperseLeft(DispersionAmount);
        }

        return true;
    }


    public void DisperseLeft(int maxDistance)
    {
        // Attempt to disperse left up to the given distance
        for (int dist = 1; dist <= maxDistance; dist++)
        {
            Particle targetParticle = parentMap.GetParticleAt(X - dist, Y);

            if (targetParticle is LiquidParticle)
                continue;

            if (!parentMap.IsInBounds(X - dist, Y) || targetParticle != null)
            {
                if (parentMap.GetParticleAt(X - dist + 1, Y) == null ||  // If the space is empty or...
                (parentMap.GetParticleAt(X - dist + 1, Y) is LiquidParticle && // it's a liquid,
                parentMap.GetParticleAt(X - dist + 1, Y).GetType() != this.GetType() && // of a different type,
                (parentMap.GetParticleAt(X - dist + 1, Y) as LiquidParticle).LiquidDensity < LiquidDensity)) // and it's less dense
                { SwapWith(X - dist + 1, Y); } // Then swap with it

                return;
            }
        }

        if (parentMap.GetParticleAt(X - maxDistance, Y) == null ||  // If the space is empty or...
        (parentMap.GetParticleAt(X - maxDistance, Y) is LiquidParticle && // it's a liquid,
        parentMap.GetParticleAt(X - maxDistance, Y).GetType() != this.GetType() && // of a different type,
        (parentMap.GetParticleAt(X - maxDistance, Y) as LiquidParticle).LiquidDensity < LiquidDensity)) // and it's less dense
        { SwapWith(X - maxDistance, Y); } // Then swap with it
    }

    public void DisperseRight(int maxDistance)
    {
        // Attempt to disperse right up to the given distance
        for (int dist = 1; dist <= maxDistance; dist++)
        {
            Particle targetParticle = parentMap.GetParticleAt(X + dist, Y);

            if (targetParticle is LiquidParticle)
                continue;

            if (!parentMap.IsInBounds(X + dist, Y) || targetParticle != null)
            {
                if (parentMap.GetParticleAt(X + dist - 1, Y) == null ||  // If the space is empty or...
                (parentMap.GetParticleAt(X + dist - 1, Y) is LiquidParticle && // it's a liquid,
                parentMap.GetParticleAt(X + dist - 1, Y).GetType() != this.GetType() && // of a different type,
                (parentMap.GetParticleAt(X + dist - 1, Y) as LiquidParticle).LiquidDensity < LiquidDensity)) // and it's less dense
                { SwapWith(X + dist - 1, Y); } // Then swap with it

                return;
            }
        }

        if (parentMap.GetParticleAt(X + maxDistance, Y) == null ||  // If the space is empty or...
        (parentMap.GetParticleAt(X + maxDistance, Y) is LiquidParticle && // it's a liquid,
        parentMap.GetParticleAt(X + maxDistance, Y).GetType() != this.GetType() && // of a different type,
        (parentMap.GetParticleAt(X + maxDistance, Y) as LiquidParticle).LiquidDensity < LiquidDensity)) // and it's less dense
        { SwapWith(X + maxDistance, Y); } // Then swap with it
    }
}