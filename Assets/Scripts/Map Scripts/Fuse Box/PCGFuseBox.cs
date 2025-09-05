using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCGFuseBox : MonoBehaviour
{

    public int rows;
    public int cols;
    public float cellSize;
    public ChordConnector[] mapPieces;

    private (ChordConnector, int rotation)[,] mapSections;
    //private Vector3 currentPos;
    //private float startXPos;

    void Awake()
    {
        mapSections = new (ChordConnector, int)[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                PlacePiece(i, j);
            }
        }
    }

    void PlacePiece(int row, int col)
    {

        int isEmpty = Random.Range(0, 2);

        if (isEmpty == 0)
        {
            return;
        }
        else
        {
            List<(ChordConnector, int)> validPieces = new List<(ChordConnector, int)>();

            foreach (ChordConnector connector in mapPieces)
            {
                for (int rotation = 0; rotation < 4; rotation++)
                {
                    bool[] connections = connector.GetConnections(rotation);
                    if (IsValid(row, col, connections))
                    {
                        validPieces.Add((connector, rotation));
                    }
                }
            }

            if (validPieces.Count == 0)
            {
                print("no valid pieces");
                return;
            }

            //Item 1 is the ChordConnector Class Instance and Item 2 is the number of rotations
            (ChordConnector, int) selectedPiece = validPieces[Random.Range(0, validPieces.Count)];
            Vector3 pos = transform.position + new Vector3(col * cellSize, 0, -row * cellSize);
            GameObject thisPiece = Instantiate(selectedPiece.Item1.gameObject, pos, Quaternion.Euler(0, selectedPiece.Item2 * -90f, 90f));
            //thisPiece.GetComponent<ChordConnector>().ApplyRotation(selectedPiece.Item2);
            mapSections[row, col] = (thisPiece.GetComponent<ChordConnector>(), selectedPiece.Item2);
        }
    }

    bool IsValid(int row, int col, bool[] connections)
    {
        //Checking if connectors have pieces leading to the edge of the map
        if (row == 0 && connections[0])
        {
            return false;
        }
        if (col == 0 && connections[1])
        {
            return false;
        }
        if (row == rows - 1 && connections[2])
        {
            return false;
        }
        if (col == cols - 1 && connections[3])
        {
            return false;
        }

        //Checks the neighbouring pieces
        if (row > 0 && mapSections[row - 1, col].Item1 != null)
        {
            var (above, aboveRot) = mapSections[row - 1, col];
            bool[] aboveConnections = above.GetConnections(aboveRot);

            if ((aboveConnections[2] == true && connections[0] == false) || (aboveConnections[2] == false && connections[0] == true))
            {
                return false;
            }
        }

        if (col > 0 && mapSections[row, col - 1].Item1 != null)
        {
            var (left, leftRot) = mapSections[row, col - 1];
            bool[] leftConnections = left.GetConnections(leftRot);

            if ((leftConnections[3] ==  true && connections[1] == false) || (leftConnections[3] == false && connections[1] == true))
            {
                return false;
            }
        }

        return true;
    }
}
