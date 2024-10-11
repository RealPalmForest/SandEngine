using System;
using System.Diagnostics;

namespace SandEngine.AbstractParticles;

public abstract class LiquidParticle : MovingParticle
{
    // Determines which liquids float above others, and which switch
    // Lower density rises above higher density liquids
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
            else
            {
                // If all horizontal movement fails, disperse
                if (Globals.Random.Next(2) == 0)
                    DisperseLeft(DispersionAmount);
                else
                    DisperseRight(DispersionAmount);
            }
        }

        return true;
    }


    public void DisperseLeft(int maxDistance)
    {
        // Attempt to disperse left up to the given distance
        for (int dist = 1; dist <= maxDistance; dist++)
        {
            Particle targetParticle = parentMap.GetParticleAt(X - dist, Y);

            if (targetParticle is LiquidParticle && parentMap.GetParticleAt(X - dist - 1, Y) == null && dist < maxDistance)
                continue;

            if (targetParticle is LiquidParticle)
            {
                // If the target  particle is the same liquid, continue
                if (targetParticle.GetType() == this.GetType())
                {
                    continue;
                }
                // If it's a different liquid, they swap
                else
                {
                    SwapWith(X - dist, Y);
                    return;
                }
            }

            if (!parentMap.IsInBounds(X - dist, Y) || targetParticle != null/* || (targetParticle != null && !(targetParticle is LiquidParticle))*/)
            {
                // Swap with the previous particle if it's either empty or a particle of a different type
                if (parentMap.GetParticleAt(X - dist + 1, Y) == null || (parentMap.GetParticleAt(X - dist + 1, Y) != null && parentMap.GetParticleAt(X - dist + 1, Y).GetType() != this.GetType()))
                    SwapWith(X - dist + 1, Y);
                return;
            }
        }

        if (parentMap.GetParticleAt(X - maxDistance, Y) == null)
            SwapWith(X - maxDistance, Y);
        else if (parentMap.GetParticleAt(X - maxDistance, Y).GetType() != this.GetType())
            SwapWith(X - maxDistance, Y);
    }

    public void DisperseRight(int maxDistance)
    {
        // Attempt to disperse right up to the given distance
        for (int dist = 1; dist <= maxDistance; dist++)
        {
            Particle targetParticle = parentMap.GetParticleAt(X + dist, Y);

            if (targetParticle is LiquidParticle)
            {
                // If the target  particle is the same liquid, continue
                if (targetParticle.GetType() == this.GetType())
                {
                    continue;
                }
                // If it's a different liquid, they swap
                else
                {
                    SwapWith(X - dist, Y);
                    return;
                }
            }

            if (targetParticle is LiquidParticle && parentMap.GetParticleAt(X + dist + 1, Y) == null && dist < maxDistance)
                continue;

            if (!parentMap.IsInBounds(X + dist, Y) || targetParticle != null/* && !(targetParticle is LiquidParticle)*/)
            {
                // Swap with the previous particle if it's either empty or a particle of a different type
                if (parentMap.GetParticleAt(X + dist - 1, Y) == null || (parentMap.GetParticleAt(X + dist - 1, Y) != null && parentMap.GetParticleAt(X + dist - 1, Y).GetType() != this.GetType()))
                    SwapWith(X + dist - 1, Y);
                return;
            }
        }

        if (parentMap.GetParticleAt(X + maxDistance, Y) == null)
            SwapWith(X + maxDistance, Y);
        else if (parentMap.GetParticleAt(X + maxDistance, Y).GetType() != this.GetType())
            SwapWith(X + maxDistance, Y);
    }




    [Obsolete("I don't think it works")]
    /// <summary>
    /// Counts the amount of particles in a horizontal line of the specified direction
    /// </summary>
    private int CountLiquidsInDirection(int direction, int maxDistance)
    {
        int liquidCount = 0;

        // Check each space in the specified direction up to the max distance
        for (int i = 1; i <= maxDistance; i++)
        {
            int targetX = X + (i * direction);

            if (!parentMap.IsInBounds(targetX, Y))
                break;

            // Count the liquid particles in a row
            Particle particle = parentMap.GetParticleAt(targetX, Y);
            if (particle == null)
                continue;
            else if (particle is LiquidParticle)
                liquidCount++;
            else break;
        }

        return liquidCount;
    }
}