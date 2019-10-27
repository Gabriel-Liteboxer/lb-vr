using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindCircumcenter : MonoBehaviour
{

    // This pair is used to store the X and Y 
    // coordinate of a point respectively 
    //#define pdd pair<float, float> 

    // Function to find the line given two points 
    void lineFromPoints(Vector2 P, Vector2 Q, ref float a,
                            ref float b, ref float c)
    {
        a = Q.y - P.y;
        b = P.x - Q.x;
        c = a * (P.x) + b * (P.y);
    }

    // Function which converts the input line to its 
    // perpendicular bisector. It also inputs the points 
    // whose mid-point lies on the bisector 
    void perpendicularBisectorFromLine(Vector2 P, Vector2 Q,
                    ref float a, ref float b, ref float c)
    {
        Vector2 mid_point = new Vector2((P.x + Q.x) / 2,
                                (P.y + Q.y) / 2);

        // c = -bx + ay 
        c = -b * (mid_point.x) + a * (mid_point.y);

        float temp = a;
        a = -b;
        b = temp;
    }

    // Returns the intersection point of two lines 
    Vector2 lineLineIntersection(float a1, float b1, float c1,
                            float a2, float b2, float c2)
    {
        float determinant = a1 * b2 - a2 * b1;
        if (determinant == 0)
        {
            // The lines are parallel. This is simplified 
            // by returning a pair of FLT_MAX 
            return new Vector2(float.MaxValue, float.MaxValue);
        }

        else
        {
            float x = (b2 * c1 - b1 * c2) / determinant;
            float y = (a1 * c2 - a2 * c1) / determinant;
            return new Vector2(x, y);
        }
    }

    public Vector2 FindCircumCenter(Vector2 P, Vector2 Q, Vector2 R)
    {
        // Line PQ is represented as ax + by = c 
        float a = 0, b = 0, c = 0;
        lineFromPoints(P, Q, ref a, ref b, ref c);

        // Line QR is represented as ex + fy = g 
        float e = 0, f = 0, g = 0;
        lineFromPoints(Q, R, ref e, ref f, ref g);

        // Converting lines PQ and QR to perpendicular 
        // vbisectors. After this, L = ax + by = c 
        // M = ex + fy = g 
        perpendicularBisectorFromLine(P, Q, ref a, ref b, ref c);
        perpendicularBisectorFromLine(Q, R, ref e, ref f, ref g);

        // The point of intersection of L and M gives 
        // the circumcenter 
        Vector2 circumcenter =
            lineLineIntersection(a, b, c, e, f, g);

        if (circumcenter.x == float.MaxValue &&
            circumcenter.y == float.MaxValue)
        {
            Debug.Log("The two perpendicular bisectors ");

            Debug.Log("found come parallel");
            Debug.Log("Thus, the given points do not form ");

            Debug.Log("a triangle and are collinear");
        }
        else
        {
            Debug.Log("The circumcenter of the triangle PQR is: "
            + "(" + circumcenter.x + ", "
            + circumcenter.y + ")");

            return circumcenter;
        }
        return new Vector2(float.MaxValue, float.MaxValue);
    }

    /*
    void findCircumCenter(Vector2 P, Vector2 Q, Vector2 R)
    {
        // Line PQ is represented as ax + by = c 
        float a = 0, b = 0, c = 0;
        lineFromPoints(P, Q, ref a, ref b, ref c);

        // Line QR is represented as ex + fy = g 
        float e = 0, f = 0, g = 0;
        lineFromPoints(Q, R, ref e, ref f, ref g);

        // Converting lines PQ and QR to perpendicular 
        // vbisectors. After this, L = ax + by = c 
        // M = ex + fy = g 
        perpendicularBisectorFromLine(P, Q, ref a, ref b, ref c);
        perpendicularBisectorFromLine(Q, R, ref e, ref f, ref g);

        // The point of intersection of L and M gives 
        // the circumcenter 
        Vector2 circumcenter =
            lineLineIntersection(a, b, c, e, f, g);

        if (circumcenter.x == float.MaxValue &&
            circumcenter.y == float.MaxValue)
        {
            Debug.Log("The two perpendicular bisectors ");

            Debug.Log("found come parallel");
            Debug.Log("Thus, the given points do not form ");

            Debug.Log("a triangle and are collinear");
        }
        else
        {
            Debug.Log("The circumcenter of the triangle PQR is: "
            + "(" + circumcenter.x + ", "
            + circumcenter.y + ")");
        }
    }
    */
    /*
    // Driver code. 
    public Vector2 FindCircleCenterpoint(Vector2 P, Vector2 Q, Vector2 R)
    {
        Vector2 P = new Vector2(6, 0);
        Vector2 Q = new Vector2(0, 0);
        Vector2 R = new Vector2(0, 8);
        findCircumCenter(P, Q, R);
        
        re
    }*/

}
