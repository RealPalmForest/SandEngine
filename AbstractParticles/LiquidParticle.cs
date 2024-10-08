using System.Diagnostics;

namespace SandEngine.AbstractParticles;

public abstract class LiquidParticle : MovingParticle
{
    public LiquidParticle(GameMap parentMap) : base(parentMap) { }


    /// <summary>
    /// Attempts to move particle at position down, left-down, right-down, left, right
    /// </summary>
    /// <returns>Returns true if particle was successfully moved</returns>
    protected virtual bool DefaultLiquidBehaviour()
    {
        UpdateVerticalMovement();

        if (GetRight() == null && GetBelowRight() == null)
            Move(X + 1, Y + 1);
        else if (GetLeft() == null && GetBelowLeft() == null)
            Move(X - 1, Y + 1);
        else
        {
            // Disperse in the less filled direction
            if (/*CountLiquidsInDirection(1, DispersionAmount * 2) > CountLiquidsInDirection(-1, DispersionAmount * 2)*/Globals.Random.Next(2) == 0)
                DisperseHorizontally(-DispersionAmount);
            else
                DisperseHorizontally(DispersionAmount);
        }




        return true;
    }

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
            if (particle != null && particle is LiquidParticle)
                liquidCount++;
            else break;
        }

        return liquidCount;
    }
}