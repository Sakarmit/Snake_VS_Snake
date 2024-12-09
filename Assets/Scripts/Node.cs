using System;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    public float x { get; set; }
    public float y { get; set; }
    public float f { get; set; }
    public float g { get; set; }
    public float angle { get; set; }
    public Node nodeParent { get; set; }

    public Node(float _x, float _y, float _angle, Vector2 target, Node parent) {
        if (parent != null && Math.Abs(parent.angle - _angle) > 90) {
            throw new ArgumentException("Node is behind parent...path finding is wrong");
        }

        if (_angle == -90 || _angle == 0 || _angle == 90 || _angle == 180) {
            angle = _angle;
        } else if (_angle == -180) {
            angle = 180;
        } else if (_angle == 270) {
            angle = -90;
        } else{
            throw new ArgumentException("Invalid angle");
        }
        x = _x;
        y = _y;
        nodeParent = parent;
        calculateValues(target, parent);
    }

    public Node(Vector2 position, float _angle, Vector2 target, Node parent)
        : this(position.x, position.y, _angle, target, parent) {}

    public Vector3 vector3Position() {
        return new Vector3(x, y, 0);
    }
    public void calculateValues(Vector2 target, Node parentG) {
        float h = GeneralFunctions.manhattanDistance(nextLocation(), new Vector2(x, y));
		if (parentG != null) {
			g = parentG.g;
			g += 0.5f;
		}
		f = g + h;
    }

    Vector2 nextLocation() {
        if (angle == -90) {
            return new Vector2(x - 0.5f, y);
        } else if (angle == 0) {
            return new Vector2(x, y + 0.5f);
        } else if (angle == 90) {
            return new Vector2(x + 0.5f, y);
        } else if (angle == 180) {
            return new Vector2(x, y - 0.5f);
        } else {
            throw new ArgumentException("Invalid angle");
        }
    }
}