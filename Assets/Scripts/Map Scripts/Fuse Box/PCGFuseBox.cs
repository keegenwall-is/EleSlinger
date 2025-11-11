using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCGFuseBox : MonoBehaviour
{

    public int rows;
    public int cols;
    public float cellSize;
    public ChordConnector[] mapPieces;
    public GameObject[] specialPieces;
    public float chordPercent;
    public float specialPercent;

    private (ChordConnector, int rotation)[,] mapSections;
    private List<GameObject> specialsPlaced = new List<GameObject>();
    private bool mainSpawned = false;
    private int chordTarget;
    private int specialTarget;
    private int chordCount = 0;
    private int specialCount = 0;
    private float chordWeight = 1f;
    private float specialWeight = 1f;
    private float totalWeight;
    private int littleSwitchCount = 0;
    private int itemSpawnerCount = 0;
    private bool mapSuccess = false;

    void Awake()
    {
        mapSections = new (ChordConnector, int)[rows, cols];

        chordTarget = Mathf.RoundToInt(rows * cols * chordPercent);
        specialTarget = Mathf.RoundToInt(rows * cols * specialPercent);

        while (!mapSuccess)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    PlacePiece(i, j);
                }
            }

            if (!mainSpawned || littleSwitchCount <= 6 || littleSwitchCount >= 10 || itemSpawnerCount <=3)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (mapSections[i, j].Item1 != null)
                        {
                            Destroy(mapSections[i, j].Item1.gameObject);
                            mapSections[i, j] = (null, 0);
                        }
                    }
                }

                for (int i = 0; i < specialsPlaced.Count; i++)
                {
                    Destroy(specialsPlaced[i]);
                }

                chordCount = 0;
                specialCount = 0;
                chordWeight = 1f;
                specialWeight = 1f;
                mainSpawned = false;
                littleSwitchCount = 0;
                itemSpawnerCount = 0;
            }
            else
            {
                mapSuccess = true;
            }
        }
    }

    void PlacePiece(int row, int col)
    {
        List<(ChordConnector, int)> validPieces = new List<(ChordConnector, int)>();

        bool mustBeStraight = false;
        bool mustBeChord = false;

        //Check the row above if straight and can connect to a straight
        if (row > 0 && mapSections[row - 1, col].Item1 != null)
        {
            var (above, aboveRot) = mapSections[row - 1, col];
            bool[] aboveConnections = above.GetConnections(aboveRot);

            if (row > 1 && mapSections[row - 2, col].Item1 == null)
            {
                mustBeChord = true;
            }
            if (mapSections[row - 1, col].Item1.IsNotStraight() && aboveConnections[2])
            {
                mustBeStraight = true;
            }
        }

        //Check the col on the left if straight and can connect to a straight
        if (col > 0 && mapSections[row, col - 1].Item1 != null)
        {
            var (left, leftRot) = mapSections[row, col - 1];
            bool[] leftConnections = left.GetConnections(leftRot);

            if (col > 1 && mapSections[row, col - 2].Item1 == null)
            {
                mustBeChord = true;
            }
            if (mapSections[row, col - 1].Item1.IsNotStraight() && leftConnections[3])
            {
                mustBeStraight = true;
            }
        }

        chordWeight = 1f;
        specialWeight = 1f;

        if (chordCount >= chordTarget)
        {
            chordWeight = 0.1f;
        }

        if (specialCount >= specialTarget)
        {
            specialWeight = 0.1f;
        }

        totalWeight = chordWeight + specialWeight;

        float isEmpty = Random.value * totalWeight;

        if (isEmpty > chordWeight && !mustBeChord)
        {
            PlaceSpecial(row, col);
            return;
        }
        else
        {
            foreach (ChordConnector connector in mapPieces)
            {
                if (mustBeStraight)
                {
                    if (!connector.IsNotStraight())
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
                }
                else
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
            }

            if (validPieces.Count == 0)
            {
                PlaceSpecial(row, col);
                return;
            }

            //Item 1 is the ChordConnector Class Instance and Item 2 is the number of rotations
            (ChordConnector, int) selectedPiece = validPieces[Random.Range(0, validPieces.Count)];
            Vector3 pos = transform.position + new Vector3(col * cellSize, 0, -row * cellSize);
            GameObject thisPiece = Instantiate(selectedPiece.Item1.gameObject, pos, Quaternion.Euler(0, selectedPiece.Item2 * -90f, 90f));
            //thisPiece.GetComponent<ChordConnector>().ApplyRotation(selectedPiece.Item2);
            mapSections[row, col] = (thisPiece.GetComponent<ChordConnector>(), selectedPiece.Item2);
            chordCount++;
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

        //Checks the neighbouring pieces for connections
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

    private void PlaceSpecial(int row, int col)
    {
        float isEmpty = Random.value * totalWeight;
        if (isEmpty < specialWeight)
        {
            int randSpecial = Random.Range(0, specialPieces.Length);

            while (mainSpawned && randSpecial == 0)
            {
                randSpecial = Random.Range(0, specialPieces.Length);
            }

            Vector3 pos = transform.position + new Vector3(col * cellSize, 0, -row * cellSize);

            //if a main switch is to be generated
            if (randSpecial == 0)
            {
                if (row > 1 && col > 1 && mapSections[row, col - 1].Item2 != -2 && mapSections[row - 1, col - 1].Item2 != -2)
                {
                    mainSpawned = true;
                    //pos.z += cellSize;
                    //pos.x -= cellSize;
                }
                else
                {
                    mapSections[row, col] = (null, -2);
                    return;
                }
            }
            else if (randSpecial == 1)
            {
                itemSpawnerCount++;
            }
            else if (randSpecial == 2)
            {
                littleSwitchCount++;
            }

            specialsPlaced.Add(Instantiate(specialPieces[randSpecial], pos, Quaternion.Euler(0, 0, 0)));
            //Set as empty with a special
            mapSections[row, col] = (null, -1);
            specialCount++;
        }
        else
        {
            //Set as true empty
            mapSections[row, col] = (null, -2);
        }
    }
}
