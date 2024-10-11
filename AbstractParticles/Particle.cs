using System;
using System.Diagnostics;
using System.Formats.Asn1;
using Microsoft.Xna.Framework;

namespace SandEngine.AbstractParticles;

public abstract class Particle
{
    public bool StaticState { get; set; }
    public bool HasBeenUpdated { get; set; }
    public Color Color { get; set; }

    public int X { get; set; }
    public int Y { get; set; }


    protected GameMap parentMap;


    public Particle(GameMap parentMap)
    {
        this.parentMap = parentMap;

    }

    public virtual void Update() { }

    public void UpdateStaticState()
    {
        // If this particle is fully surrounded by those of its type, set its state to static
        if (GetAbove() != null && GetAbove().GetType() == this.GetType() &&
            GetBelow() != null && GetBelow().GetType() == this.GetType() &&
            GetRight() != null && GetRight().GetType() == this.GetType() &&
            GetLeft() != null && GetLeft().GetType() == this.GetType() &&
            GetBelowLeft() != null && GetBelowLeft().GetType() == this.GetType() &&
            GetBelowRight() != null && GetBelowRight().GetType() == this.GetType() &&
            GetAboveRight() != null && GetAboveRight().GetType() == this.GetType() &&
            GetAboveLeft() != null && GetAboveLeft().GetType() == this.GetType())
            StaticState = true;
    }

    public void Move(int newX, int newY)
    {
        if (!parentMap.IsInBounds(newX, newY))
            return;

        if (newX == X && newY == Y)
            return;

        DisableSurroundingStaticState();

        Remove();
        parentMap.SetParticleAt(newX, newY, this);
    }

    public void SwapWith(int x, int y)
    {
        if (!parentMap.IsInBounds(x, y))
            return;

        if (X == x && Y == y)
            return;

        DisableSurroundingStaticState();
        parentMap.GetParticleAt(x, y).DisableSurroundingStaticState();

        parentMap.SetParticleAt(X, Y, parentMap.GetParticleAt(x, y));
        parentMap.SetParticleAt(x, y, this);
    }

    public void Remove()
    {
        parentMap.SetParticleAt(X, Y, null);
    }

    public void DisableSurroundingStaticState()
    {
        if (GetAbove() != null) GetAbove().StaticState = false;
        if (GetBelow() != null) GetBelow().StaticState = false;
        if (GetRight() != null) GetRight().StaticState = false;
        if (GetLeft() != null) GetLeft().StaticState = false;
        if (GetAboveRight() != null) GetAboveRight().StaticState = false;
        if (GetAboveLeft() != null) GetAboveLeft().StaticState = false;
        if (GetBelowRight() != null) GetBelowRight().StaticState = false;
        if (GetBelowLeft() != null) GetBelowLeft().StaticState = false;
    }


    public Particle Clone()
    {
        return (Particle)MemberwiseClone();
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