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
            if (GetAboveLeft() == null)
                Move(X - 1, Y - 1);
            else if (GetAboveRight() == null)
                Move(X + 1, Y - 1);
            else
            {
                // Try dispersing left or right up to the max distance
                if (Globals.Random.Next(2) == 0)
                {
                    if (!DisperseHorizontally(-DispersionAmount))
                        return DisperseHorizontally(DispersionAmount); // If left fails, try right
                }
                else
                {
                    if (!DisperseHorizontally(DispersionAmount))
                        return DisperseHorizontally(-DispersionAmount); // If right fails, try left
                }
            }
        }

        return true;
    }
}