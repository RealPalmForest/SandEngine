namespace SandEngine.AbstractParticles;

public abstract class GasParticle : MovingParticle
{
    public GasParticle(GameMap parentMap) : base(parentMap) { }


    /// <summary>
    /// Attempts to move particle at position up, left-up, right-up, left, right
    /// </summary>
    /// <returns>Returns true if particle was successfully moved</returns>
    protected virtual bool DefaultGasBehaviour()
    {
        if (!UpdateVerticalMovement())
        {
            if (GetLeft() == null && GetAboveLeft() == null)
                Move(X - 1, Y - 1);
            else if (GetRight() == null && GetAboveRight() == null)
                Move(X + 1, Y - 1);

            if (Globals.Random.Next(2) == 0)
                DisperseLeft(DispersionAmount);
            else
                DisperseRight(DispersionAmount);
        }

        return true;
    }

    public void DisperseLeft(int maxDistance)
    {
        // Attempt to disperse left up to the given distance
        for (int dist = 1; dist <= maxDistance; dist++)
        {
            if (parentMap.GetParticleAt(X - dist, Y) is GasParticle && parentMap.GetParticleAt(X - dist - 1, Y) == null && dist < maxDistance)
                continue;

            if (!parentMap.IsInBounds(X - dist, Y) || parentMap.GetParticleAt(X - dist, Y) != null)
            {
                Move(X - dist + 1, Y);
                return;
            }
        }

        Move(X - maxDistance, Y);
    }

    public void DisperseRight(int maxDistance)
    {
        // Attempt to disperse right up to the given distance
        for (int dist = 1; dist <= maxDistance; dist++)
        {
            if (parentMap.GetParticleAt(X + dist, Y) is GasParticle && parentMap.GetParticleAt(X + dist + 1, Y) == null && dist < maxDistance)
                continue;

            if (!parentMap.IsInBounds(X + dist, Y) || parentMap.GetParticleAt(X + dist, Y) != null)
            {
                Move(X + dist - 1, Y);
                return;
            }
        }

        Move(X + maxDistance, Y);
    }
}