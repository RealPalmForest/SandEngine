using System;

namespace SandEngine.AbstractParticles;

public abstract class MovingParticle : Particle
{
    public float VelocityX { get; set; }
    public float VelocityY { get; set; }

    public float Acceleration { get; protected set; } = 0.1f;
    public int MaxVelocity { get; protected set; } = 6;
    public int Gravity { get; protected set; } = 1;
    public int DispersionAmount { get; protected set; } = 1;
    public bool DisplaceLiquids { get; protected set; }

    public MovingParticle(GameMap parentMap) : base(parentMap) { VelocityY = Gravity; }


    /// <summary>
    /// Moves vertically with acceleration
    /// </summary>
    /// <returns>Returns true if particle was successfully moved</returns>
    protected bool UpdateVerticalMovement()
    {
        int direction = Math.Sign(Acceleration);

        if (direction == 0)
            return false;

        // Change vertical velocity with acceleration
        VelocityY = Math.Clamp(VelocityY + Acceleration, -MaxVelocity, MaxVelocity);

        if ((Y == parentMap.Height - 1 && direction > 0) || (Y == 0 && direction < 0))
        {
            VelocityY = Gravity;
            return false;
        }


        Particle targetParticle;
        if (direction < 0)
            targetParticle = GetAbove();
        else
            targetParticle = GetBelow();

        if (targetParticle != null)
        {
            // If the target particle is at a stand-still, then this particle rests too
            if (targetParticle is StableParticle)
            {
                VelocityY = Gravity;
                return false;
            }

            if (((MovingParticle)targetParticle).Acceleration < 0 && Acceleration < 0)
                VelocityY = ((MovingParticle)targetParticle).VelocityY;
            else if (((MovingParticle)targetParticle).Acceleration > 0 && Acceleration > 0)
                VelocityY = ((MovingParticle)targetParticle).VelocityY;
            else VelocityY = Gravity;
            return false;
        }


        int fallDistance = (int)Math.Abs(VelocityY);

        // Check all spaces between current position and target position
        for (int i = 1; i <= fallDistance; i++)
        {
            int targetY = Y + i * direction;

            if (targetY >= parentMap.Height || targetY < 0)
            {
                Move(X, Y + i * direction - direction);
                return i > 1 ? true : false;
            }

            Particle target = parentMap.GetParticleAt(X, targetY);

            // If the target space is empty, continue moving
            if (target == null)
                continue;
            else
            {
                // Otherwise move to the furthest empty space
                Move(X, Y + i * direction - direction);
                return i > 1 ? true : false;
            }
        }

        // If all spaces are empty, move to the maximum fall distance
        Move(X, Y + fallDistance * direction);
        return true;
    }


    /// <summary>
    /// Attempts to move the target particle out of the way of this particle, otherwise swaps them
    /// </summary>
    public void Displace(int x, int y)
    {
        if (!parentMap.IsInBounds(x, y))
            return;

        Particle target = parentMap.GetParticleAt(x, y);

        // Attempt to move the liquid out of the way first
        if (target.GetLeft() == null)
            target.Move(x - 1, y);
        else if (target.GetRight() == null)
            target.Move(x + 1, y);
        // ... otherwise swap this particle with the liquid
        else SwapWith(x, y);
    }


    /// <summary>
    /// Attempts to move particle as far as possible until specified maximum distance
    /// </summary>
    /// <returns>Returns true if particle was moved successfully</returns>
    protected bool DisperseHorizontally(int maxDistance)
    {
        int direction = Math.Sign(maxDistance); // Determine spread direction

        if (X + maxDistance >= parentMap.Width - 1 || X + maxDistance <= 0)
            return false;

        for (int i = 1; i <= Math.Abs(maxDistance); i++)
        {
            int targetX = X + (i * direction);

            // if the target space is out of bounds, stop at the previous space
            if (!parentMap.IsInBounds(targetX, Y))
            {
                Move(X + ((i - 1) * direction), Y);
                return i > 1 ? true : false;
            }


            Particle targetParticle = parentMap.GetParticleAt(targetX, Y);

            // If the space is not empty, move to the previous space
            if (targetParticle != null)
            {
                Move(X + ((i - 1) * direction), Y);
                return true;
            }
        }

        // If all spaces are empty, move to the farthest valid position
        Move(X + maxDistance, Y);
        return true;
    }
}