using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Older representation of SoftBody Spring - kept for legacy reasons
/// 
/// Strut is only represented by damped spring - no torsional springs for volume conservation.
/// </summary>
public class SBFace
{
    private List<SBDampedSpring> springs;

    private SBNode n1;
    private SBNode n2;
    private SBNode n3;
    
    private float angle1;
    private float angle2;
    private float angle3;

    public SBFace(List<SBDampedSpring> springs)
    {
        this.springs = springs;

        n1 = springs[0].start;
        n2 = springs[0].end;

        if (springs[1].start == n1 || springs[1].start == n2) {
            n3 = springs[1].end;
        } else {
            n3 = springs[1].start;
        }
        
        CalculateAngles();
    }

    void CalculateAngles()
    {
        angle1 = angleFromThreePoints(n1.Position, n2.Position, n3.Position);
        angle2 = angleFromThreePoints(n2.Position, n3.Position, n1.Position);
        angle3 = angleFromThreePoints(n3.Position, n1.Position, n2.Position);
    }

    float angleFromThreePoints(Vector3 P1, Vector3 P2, Vector3 P3)
    {
        Vector3 v1 = (P1 - P2).normalized;
        Vector3 v2 = (P3 - P2).normalized;
        return Vector3.Angle(v1, v2);
    }


}
