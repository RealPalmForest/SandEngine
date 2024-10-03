using System;
using System.Diagnostics;
using System.Formats.Asn1;
using Microsoft.Xna.Framework;

namespace SandEngine.Particles;


public enum Direction {
    Up,
    Right,
    Down,
    Left
}


public abstract class Particle
{
    public bool StaticState;
    public bool HasBeenUpdated { get; set; }
    public Color Color { get; set; }
    
    public int X { get; set; }
    public int Y { get; set; }

    public float VelocityX { get; set; }
    public float VelocityY { get; set; }
    public float Acceleration = 0.1f;
    public int MaxVelocity = 6;
    public int Gravity = 1;

    public int DispersionAmount = 1;
    
    public bool IsLiquid { get; set; }
    public bool IsGas { get; set; }
    public bool IsImmovable { get; set; }
    public bool DisplaceLiquids { get; set; }

    protected GameMap parentMap;


    public Particle(GameMap parentMap) 
    { 
        this.parentMap = parentMap; 
        VelocityY = Gravity;
    }

    public virtual void Update() {}

    public void UpdateStaticState()
    {
        // If this particle is fully surrounded by those of its type, set its state to static
        if(GetAbove() != null && GetAbove().GetType() == this.GetType() &&
            GetBelow() != null && GetBelow().GetType() == this.GetType() &&
            GetRight() != null && GetRight().GetType() == this.GetType() &&
            GetLeft() != null && GetLeft().GetType() == this.GetType() &&
            GetBelowLeft() != null && GetBelowLeft().GetType() == this.GetType() &&
            GetBelowRight() != null  && GetBelowRight().GetType() == this.GetType() &&
            GetAboveRight() != null && GetAboveRight().GetType() == this.GetType() &&
            GetAboveLeft() != null && GetAboveLeft().GetType() == this.GetType())
            StaticState = true;
    }

    public void Move(int newX, int newY)
    {
        if(!parentMap.IsInBounds(newX, newY))
            return;

        if(newX == X && newY == Y)
            return;

        DisableSurroundingStaticStatus();
        
        Remove();
        parentMap.SetParticleAt(newX, newY, this);
    }

    public void SwapWith(int x, int y)
    {
        if(!parentMap.IsInBounds(x, y))
            return;

        if(X == x && Y == y)
            return;

        DisableSurroundingStaticStatus();
        parentMap.GetParticleAt(x, y).DisableSurroundingStaticStatus();

        parentMap.SetParticleAt(X, Y, parentMap.GetParticleAt(x, y));
        parentMap.SetParticleAt(x, y, this);
    }

    public void Remove()
    {
        parentMap.SetParticleAt(X, Y, null);
    }

    private void DisableSurroundingStaticStatus()
    {
        if(GetAbove() != null) GetAbove().StaticState = false;
        if(GetBelow() != null) GetBelow().StaticState = false;
        if(GetRight() != null) GetRight().StaticState = false;
        if(GetLeft() != null) GetLeft().StaticState = false;
        if(GetAboveRight() != null) GetAboveRight().StaticState = false;
        if(GetAboveLeft() != null) GetAboveLeft().StaticState = false;
        if(GetBelowRight() != null) GetBelowRight().StaticState = false;
        if(GetBelowLeft() != null) GetBelowLeft().StaticState = false;
    }

    /// <summary>
    /// Moves vertically with acceleration
    /// </summary>
    protected void UpdateVerticalMovement()
    {
        // Change vertical velocity with acceleration
        VelocityY = Math.Clamp(VelocityY + Acceleration, -MaxVelocity, MaxVelocity);

        if((Y == parentMap.Height - 1 && Acceleration > 0) || (Y == 0 && Acceleration < 0))
        {
            VelocityY = Gravity;
            return;
        }


        Particle targetParticle;
        if(Acceleration < 0)
            targetParticle = GetAbove();
        else
            targetParticle = GetBelow();
            
        if(targetParticle != null)
        {
            // If the target particle is at a stand-still, then this particle rests too
            if(targetParticle.IsImmovable || targetParticle.VelocityY == Gravity)
            {
                VelocityY = Gravity;
                return;
            }
        }


        int fallDistance = (int) VelocityY;

        // Check all spaces between current position and target position
        for (int i = 1; i <= fallDistance; i++)
        {
            int targetY = Y + i;

            if (targetY >= parentMap.Height)
            {
                Move(X, Y + i - 1);
                return;
            }
            
            Particle particleBelow = parentMap.GetParticleAt(X, targetY);

            // If the space below is empty, continue moving
            if (particleBelow == null)
                continue;
            else
            {
                Move(X, Y + i - 1); // Otherwise move to the lowest empty space
                return;
            }
        }
         
        // If all spaces are empty, move to the maximum fall distance
        Move(X, Y + fallDistance);
        return;
    }


    /// <summary>
    /// Attempts to move the target liquid out of the way of this particle, otherwise swaps them
    /// </summary>
    public void DisplaceLiquidAt(int x, int y)
    {
        if(!parentMap.IsInBounds(x, y))
            return;

        Particle target = parentMap.GetParticleAt(x, y);

        if(!target.IsLiquid)
            return;

        // Attempt to move the liquid out of the way first
        if(target.GetLeft() == null)
            target.Move(x - 1, y);
        else if(target.GetRight() == null)
            target.Move(x + 1, y);
        // ... otherwise swap this particle with the liquid
        else SwapWith(x, y);
    }
    

    /// <summary>
    /// Attempts to move particle as far as possible until specified maximum distance
    /// </summary>
    /// <returns>Returns true if particle was moved successfully</returns>
    private bool DisperseHorizontally(int maxDistance)
    {
        int direction = Math.Sign(maxDistance); // Determine spread direction

        if(X + maxDistance == parentMap.Width - 1 || X + maxDistance == 0)
            return false;

        for (int i = 1; i <= Math.Abs(maxDistance); i++)
        {
            int targetX = X + (i * direction);

            // if the target space is out of bounds, stop at the previous space
            if (!parentMap.IsInBounds(targetX, Y))
            {
                Move(X + ((i - 1) * direction), Y);
                return true;
            }

            Particle targetParticle = parentMap.GetParticleAt(targetX, Y);

            // If the space is empty, continue moving
            if (targetParticle == null)
            {
                continue;
            }
            // ...otherwise, move to the last available space
            else
            {
                Move(X + ((i - 1) * direction), Y);
                return true;
            }
        }

        // If all spaces are empty, move to the farthest valid position
        Move(X + maxDistance, Y);
        return true;
    }



    /// <summary>
    /// Attempts to move particle at position down, left-down, right-down
    /// </summary>
    /// <returns>Returns true if particle was successfully moved</returns>
    protected bool SandUpdate()
    {
        if(GetBelow() == null)
            UpdateVerticalMovement();
        else if(GetBelow().IsLiquid && DisplaceLiquids)
            DisplaceLiquidAt(X, Y + 1); 
        else if(GetBelowLeft() == null)
            Move(X - 1, Y + 1);
        else if(GetBelowLeft().IsLiquid && DisplaceLiquids)
            DisplaceLiquidAt(X - 1, Y + 1);
        else if(GetBelowRight() == null)
            Move(X + 1, Y + 1);
        else if(GetBelowRight().IsLiquid && DisplaceLiquids)
            DisplaceLiquidAt(X + 1, Y + 1);
        else return false;

        return true;
    }


    /// <summary>
    /// Attempts to move particle at position down, left-down, right-down, left, right
    /// </summary>
    /// <returns>Returns true if particle was successfully moved</returns>
    protected bool LiquidUpdate()
    {
        // Attempt to fall like sand (down, down-left, down-right)
        if (!SandUpdate()) 
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

        return true;
    }


    public Particle Clone()
    {
        return (Particle) MemberwiseClone();
    }


    public Particle GetBelow() { return parentMap.GetParticleAt(X, Y + 1); }
    public Particle GetAbove() { return parentMap.GetParticleAt(X, Y - 1); }
    public Particle GetRight() { return parentMap.GetParticleAt(X + 1, Y); }
    public Particle GetLeft() { return parentMap.GetParticleAt(X - 1, Y); }
    public Particle GetBelowLeft() { return parentMap.GetParticleAt(X - 1, Y + 1); }
    public Particle GetBelowRight() { return parentMap.GetParticleAt(X + 1, Y + 1); }
    public Particle GetAboveLeft() { return parentMap.GetParticleAt(X - 1, Y - 1); }
    public Particle GetAboveRight() { return parentMap.GetParticleAt(X + 1, Y - 1); }
}