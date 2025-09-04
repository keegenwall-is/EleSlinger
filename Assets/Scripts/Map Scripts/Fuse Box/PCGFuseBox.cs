using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCGFuseBox : MonoBehaviour
{

    public int rows;
    public int cols;
    public float cellSize;
    public ChordConnector[] mapPieces;

    private ChordConnector[,] mapSections;
    //private Vector3 currentPos;
    //private float startXPos;

    void Awake()
    {
     /*   mapSections = new GameObject[rows, cols];
        startXPos = transform.position.x;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (mapSections[i,j] == null)
                {
                    //sort pieces into lists of if they have entries on each side
                    mapSections[i, j] = mapPieces[Random.Range(0, mapPieces.Length)];

                    //stops crosses from spawning on the edges of the map
                    if (i == 0 || j == 0 || i == rows - 1 || j == cols - 1)
                    {
                        while (mapSections[i, j] == mapPieces[2])
                        {
                            mapSections[i, j] = mapPieces[Random.Range(0, mapPieces.Length)];
                        }
                    }

                    if (mapSections[i, j] == mapPieces[2] || mapSections[i, j] == mapPieces[4] || mapSections[i, j] == mapPieces[5] || mapSections[i, j] == mapPieces[6])
                    {

                    }

                    Instantiate(mapSections[i, j], transform.position, mapSections[i, j].transform.rotation);
                    currentPos = transform.position;
                    currentPos.x += 10;
                    transform.position = currentPos;
                }
            }
            currentPos = transform.position;
            currentPos.x = startXPos;
            currentPos.z -= 10;
            transform.position = currentPos;
        }*/

        mapSections = new ChordConnector[rows, cols];

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
            print("no vlaid pieces");
            return;
        }

        var selectedPiece = validPieces[Random.Range(0, validPieces.Count)];
        Vector3 pos = new Vector3(col * cellSize, 0, -row * cellSize);
        GameObject thisPiece = Instantiate(selectedPiece.Item1.gameObject, pos, Quaternion.Euler(0, selectedPiece.Item2 * 90f, 0));
        mapSections[row, col] = thisPiece.GetComponent<ChordConnector>();
    }

    bool IsValid(int row, int col, bool[] connections)
    {
        return true;
    }
}
