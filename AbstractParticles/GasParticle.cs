namespace SandEngine.AbstractParticles;

public abstract class GasParticle : MovingParticle
{
    /// <summary>
    /// Determines which gasses float above others, and which switch
    /// Lower density gasses rise above higher density gasses
    /// </summary>
    public float GasDensity { get; protected set; } = 1;

    public GasParticle(GameMap parentMap) : base(parentMap) { }


    /// <summary>
    /// Attempts to move particle at position up, left-up, right-up, left, right
    /// </summary>
    /// <returns>Returns true if particle was successfully moved</returns>
    protected virtual bool DefaultGasBehaviour()
    {
        if (!UpdateVerticalMovement())
        {
            // If there is another gas particle above, and it's more dense than this one, displace
            if (GetAbove() is GasParticle && (GetAbove() as GasParticle).GasDensity > GasDensity)
            {
                Displace(X, Y - 1);
                return true;
            }

            // Check for above-left and above-right for gasses to displace
            if (GetAboveLeft() is GasParticle && (GetAboveLeft() as GasParticle).GasDensity > GasDensity)
                SwapWith(X - 1, Y - 1);
            else if (GetAboveRight() is GasParticle && (GetAboveRight() as GasParticle).GasDensity > GasDensity)
                SwapWith(X + 1, Y - 1);
            // Check for normal above-left and above-right movement
            else if (GetLeft() == null && GetAboveLeft() == null)
                Move(X - 1, Y - 1);
            else if (GetRight() == null && GetAboveRight() == null)
                Move(X + 1, Y - 1);

            if (Globals.Random.Next(2) == 0)
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

            if (targetParticle is GasParticle)
                continue;

            if (!parentMap.IsInBounds(X - dist, Y) || targetParticle != null)
            {
                if (parentMap.GetParticleAt(X - dist + 1, Y) == null ||  // If the space is empty or...
                (parentMap.GetParticleAt(X - dist + 1, Y) is GasParticle && // it's a gas,
                parentMap.GetParticleAt(X - dist + 1, Y).GetType() != this.GetType() && // of a different type,
                (parentMap.GetParticleAt(X - dist + 1, Y) as GasParticle).GasDensity < GasDensity)) // and it's less dense
                { SwapWith(X - dist + 1, Y); } // Then swap with it

                return;
            }
        }

        if (parentMap.GetParticleAt(X - maxDistance, Y) == null ||  // If the space is empty or...
        (parentMap.GetParticleAt(X - maxDistance, Y) is GasParticle && // it's a gas,
        parentMap.GetParticleAt(X - maxDistance, Y).GetType() != this.GetType() && // of a different type,
        (parentMap.GetParticleAt(X - maxDistance, Y) as GasParticle).GasDensity < GasDensity)) // and it's less dense
        { SwapWith(X - maxDistance, Y); } // Then swap with it
    }

    public void DisperseRight(int maxDistance)
    {
        // Attempt to disperse right up to the given distance
        for (int dist = 1; dist <= maxDistance; dist++)
        {
            Particle targetParticle = parentMap.GetParticleAt(X + dist, Y);

            if (targetParticle is GasParticle)
                continue;

            if (!parentMap.IsInBounds(X + dist, Y) || targetParticle != null)
            {
                if (parentMap.GetParticleAt(X + dist - 1, Y) == null ||  // If the space is empty or...
                (parentMap.GetParticleAt(X + dist - 1, Y) is GasParticle && // it's a gas,
                parentMap.GetParticleAt(X + dist - 1, Y).GetType() != this.GetType() && // of a different type,
                (parentMap.GetParticleAt(X + dist - 1, Y) as GasParticle).GasDensity < GasDensity)) // and it's less dense
                { SwapWith(X + dist - 1, Y); } // Then swap with it

                return;
            }
        }

        if (parentMap.GetParticleAt(X + maxDistance, Y) == null ||  // If the space is empty or...
        (parentMap.GetParticleAt(X + maxDistance, Y) is GasParticle && // it's a gas,
        parentMap.GetParticleAt(X + maxDistance, Y).GetType() != this.GetType() && // of a different type,
        (parentMap.GetParticleAt(X + maxDistance, Y) as GasParticle).GasDensity < GasDensity)) // and it's less dense
        { SwapWith(X + maxDistance, Y); } // Then swap with it
    }
}