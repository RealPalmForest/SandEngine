namespace SandEngine.AbstractParticles;

public abstract class FallingParticle : MovingParticle
{
    public FallingParticle(GameMap parentMap) : base(parentMap) { }

    /// <summary>
    /// Attempts to move particle at position down, left-down, right-down
    /// </summary>
    /// <returns>Returns true if particle was successfully moved</returns>
    protected virtual bool DefaultFallingBehaviour()
    {
        if (!UpdateVerticalMovement())
        {
            if (GetBelow() != null && GetBelow() is LiquidParticle && DisplaceLiquids)
                DisplaceLiquidAt(X, Y + 1);
            else if (GetBelowLeft() == null)
                Move(X - 1, Y + 1);
            else if (GetBelowLeft() is LiquidParticle && DisplaceLiquids)
                DisplaceLiquidAt(X - 1, Y + 1);
            else if (GetBelowRight() == null)
                Move(X + 1, Y + 1);
            else if (GetBelowRight() is LiquidParticle && DisplaceLiquids)
                DisplaceLiquidAt(X + 1, Y + 1);
            else return false;
        }

        return true;
    }
}