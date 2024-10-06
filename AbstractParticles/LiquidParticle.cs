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
        if (!UpdateVerticalMovement())
        {
            if (GetBelowRight() == null)
                Move(X + 1, Y + 1);
            else if (GetBelowLeft() == null)
                Move(X - 1, Y + 1);
            else
            {
                bool moveLeft = Globals.Random.Next(2) == 0;

                // Try dispersing left or right up to the max distance
                if (moveLeft)
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