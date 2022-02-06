using System;
using System.Net;
using UnityEngine;

/// <summary>
/// Older representation of SoftBody Spring - kept for legacy reasons
///
/// Strut is only represented by damped spring - no torsional springs for volume conservation.
/// </summary>
public class SBDampedSpring
{
    private float restLength;

    /// <summary> Strength constant </summary>
    private float k = 4.0f * (float) (Math.PI * Math.PI);

    /// <summary> Damping constant </summary>
    private float d = 0.8f * (float) Math.PI;

    public SBNode start;
    public SBNode end;

    public SBDampedSpring(SBNode start, SBNode end)
    {
        this.start = start;
        this.end = end;

        restLength = Vector3.Distance(start.Position, end.Position);
    }

    public SBDampedSpring(SBNode start, SBNode end, float k, float d)
    {
        this.start = start;
        this.end = end;
        this.k = k;
        this.d = d;

        restLength = Vector3.Distance(start.Position, end.Position);
    }

    public void ApplyForces()
    {
        float length = Vector3.Distance(start.Position, end.Position);

        Vector3 dir = (end.Position - start.Position).normalized;

        // apply spring forces
        Vector3 f_s = k * (length - restLength) * dir;
        start.AddForce(f_s);
        end.AddForce(-f_s);

        // apply damping force
        Vector3 f_d = d * Vector3.Dot((end.Velocity - start.Velocity), dir) * dir;
        start.AddForce(f_d);
        end.AddForce(-f_d);
    }

    public Vector3 GetStartPosition()
    {
        return start.Position;
    }

    public Vector3 GetEndPosition()
    {
        return end.Position;
    }

}
