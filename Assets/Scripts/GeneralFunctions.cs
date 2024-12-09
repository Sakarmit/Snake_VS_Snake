using System;
using UnityEngine;

public static class GeneralFunctions
{
    public static float manhattanDistance(Vector3 point1, Vector3 point2) {
        //Calculate manhattan distance between two points on the looped grid
        float xChange = Mathf.Abs(point2.x - point1.x); 
        float yChange = Mathf.Abs(point2.y - point1.y); 
        //If the points are on opposite sides of the grid used looped distance
        if (xChange > Global.widthMinMax[1]) { 
            xChange = 2 * Global.widthMinMax[1] - xChange + 0.5f; 
        } 
        if (yChange > Global.heightMinMax[1]) { 
            yChange = 2 * Global.heightMinMax[1] - yChange  + 0.5f; 
        } 
        return xChange + yChange;
    }

    public static Vector3 loopToBoard(Vector3 pos) {
        float loopTolerance = Global.loopTolerance;
        float[] xRange = Global.widthMinMax;
        float[] yRange = Global.heightMinMax;
        //If the snake is off grid move it back on the other size relatively
        Vector3 newPos = pos;
        if (newPos.x - xRange[1] > loopTolerance) {
            newPos.x = ((newPos.x + xRange[0])%(2*xRange[1]))+xRange[0]-0.5f;
        } else if (xRange[0] - newPos.x > loopTolerance) {
            newPos.x = ((newPos.x + xRange[1])%(2*xRange[1]))+xRange[1]+0.5f;
        } 

        if (newPos.y - yRange[1] > loopTolerance) {
            newPos.y = ((newPos.y + yRange[0])%(2*yRange[1]))+yRange[0]-0.5f;
        } else if (yRange[0] - newPos.y > loopTolerance) {
            newPos.y = ((newPos.y + yRange[1])%(2*yRange[1]))+yRange[1]+0.5f;
        }

        return newPos;
    }

    public static Vector2 nextLocation(float x, float y, float angle) {
        if (angle == -90 || angle == 270) {
            return new Vector2(x - 0.5f, y);
        } else if (angle == 0) {
            return new Vector2(x, y + 0.5f);
        } else if (angle == 90) {
            return new Vector2(x + 0.5f, y);
        } else if (angle == 180 || angle == -180) {
            return new Vector2(x, y - 0.5f);
        } else {
            throw new ArgumentException("Invalid angle");
        }
    }
}