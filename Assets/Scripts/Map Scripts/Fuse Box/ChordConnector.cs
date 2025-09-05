using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType { Straight, Corner, T, Cross }

public class ChordConnector : MonoBehaviour
{

    [Header("Base Orientation Connections")]
    public bool up;
    public bool left;
    public bool down;
    public bool right;

    [Header("Type of this piece")]
    public PieceType pieceType;

    public bool[] GetConnections(int rotation)
    {
        bool[] dirs = { up, left, down, right };
        bool[] rotated = new bool[4];

        for (int i = 0; i < 4; i++)
        {
            rotated[(i + rotation) % 4] = dirs[i];
        }

        return rotated;
    }

    public bool IsConnector()
    {
        if (pieceType == PieceType.Corner || pieceType == PieceType.Cross || pieceType == PieceType.T)
        {
            return true;
        }

        return false;
    }

    public void ApplyRotation(int rot)
    {
        bool[] rotated = GetConnections(rot);

        up = rotated[0];
        left = rotated[1];
        down = rotated[2];
        right = rotated[3];
    }
}
