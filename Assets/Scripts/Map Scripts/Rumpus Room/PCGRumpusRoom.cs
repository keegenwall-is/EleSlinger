using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCGRumpusRoom : MonoBehaviour
{

    public int rows;
    public int cols;
    public float cellSize;
    public ChordConnector[] mapPieces;
    public GameObject poof;

    private (ChordConnector, int rotation)[,] mapSections;
    private List<GameObject> spawnedPieces = new List<GameObject>();
    private bool allValid;

    void Awake()
    {
        bool mapNoHoles = false;
        allValid = false;

        while (!mapNoHoles || !allValid)
        {
            mapSections = new (ChordConnector, int)[rows, cols];
            allValid = true;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    PlacePiece(i, j);
                }
            }

            mapNoHoles = IsFullyConnected();

            if (!mapNoHoles || !allValid)
            {
                Array.Clear(mapSections, 0, mapSections.Length);
                foreach (GameObject piece in spawnedPieces)
                {
                    Destroy(piece);
                }
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
                if (IsValid(row, col, connections, connector))
                {
                    validPieces.Add((connector, rotation));
                }
            }
        }

        //Item 1 is the ChordConnector Class Instance and Item 2 is the number of rotations
        if (validPieces.Count > 0)
        {
            (ChordConnector, int) selectedPiece = validPieces[UnityEngine.Random.Range(0, validPieces.Count)];
            Vector3 pos = transform.position + new Vector3(col * cellSize, 0, -row * cellSize);
            GameObject thisPiece = Instantiate(selectedPiece.Item1.gameObject, pos, Quaternion.Euler(0, selectedPiece.Item2 * -90f, 0));
            ChordConnector thisConnectorScript = thisPiece.GetComponent<ChordConnector>();
            thisConnectorScript.originalPrefab = selectedPiece.Item1;
            mapSections[row, col] = (thisConnectorScript, selectedPiece.Item2);
            spawnedPieces.Add(thisPiece);
        }
        else
        {
            allValid = false;
        }

    }

    bool IsValid(int row, int col, bool[] connections, ChordConnector connector)
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

        if (((row == 0 && col == 0) || (row == rows - 1 && col == 0) || (row == 0 && col == cols - 1) || (row == rows - 1 && col == cols - 1)) && connector.gameObject.name.Contains("1"))
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

            if ((leftConnections[3] == true && connections[1] == false) || (leftConnections[3] == false && connections[1] == true))
            {
                return false;
            }
        }

        if (row < rows - 1 && mapSections[row + 1, col].Item1 != null)
        {
            var (below, belowRot) = mapSections[row + 1, col];
            bool[] belowConnections = below.GetConnections(belowRot);
            if (belowConnections[0] != connections[2]) return false;
        }

        if (col < cols - 1 && mapSections[row, col + 1].Item1 != null)
        {
            var (right, rightRot) = mapSections[row, col + 1];
            bool[] rightConnections = right.GetConnections(rightRot);
            if (rightConnections[1] != connections[3]) return false;
        }

        return true;
    }

    bool IsFullyConnected()
    {
        bool[,] visited = new bool[rows, cols];
        Queue<(int, int)> queue = new Queue<(int, int)>();

        queue.Enqueue((0, 0));
        visited[0, 0] = true;

        while (queue.Count > 0)
        {
            var (r, c) = queue.Dequeue();
            var (piece, rot) = mapSections[r, c];

            bool[] con = piece.GetConnections(rot);

            // Up
            if (con[0] && r > 0)
            {
                var (p, pr) = mapSections[r - 1, c];

                //if (p == null) return false;

                if (p.GetConnections(pr)[2] && !visited[r - 1, c])
                {
                    visited[r - 1, c] = true;
                    queue.Enqueue((r - 1, c));
                }
            }

            // Left
            if (con[1] && c > 0)
            {
                var (p, pr) = mapSections[r, c - 1];

                //if (p == null) return false;

                if (p.GetConnections(pr)[3] && !visited[r, c - 1])
                {
                    visited[r, c - 1] = true;
                    queue.Enqueue((r, c - 1));
                }
            }

            // Down
            if (con[2] && r < rows - 1)
            {
                var (p, pr) = mapSections[r + 1, c];

                if (p == null) continue;

                if (p.GetConnections(pr)[0] && !visited[r + 1, c])
                {
                    visited[r + 1, c] = true;
                    queue.Enqueue((r + 1, c));
                }
            }

            // Right
            if (con[3] && c < cols - 1)
            {
                var (p, pr) = mapSections[r, c + 1];

                //if (p == null) return false;

                if (p.GetConnections(pr)[1] && !visited[r, c + 1])
                {
                    visited[r, c + 1] = true;
                    queue.Enqueue((r, c + 1));
                }
            }
        }

        // Check for any unreachable cells
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                if (!visited[i, j])
                    return false;

        return true;
    }

    public IEnumerator RegenerateRow(int rowIndex)
    {
        //Save a backup in case a new row can not be generated
        (ChordConnector prefabAsset, int rotation)[] backupRow = new (ChordConnector, int)[cols];

        for (int col = 0; col < cols; col++)
        {
            var (currentPiece, rotation) = mapSections[rowIndex, col];
            if (currentPiece != null)
            {
                // Store the original prefab reference, not the live scene object
                backupRow[col] = (currentPiece.originalPrefab, rotation);
            }
        }

        // Delete old objects
        for (int col = 0; col < cols; col++)
        {
            var (piece, rot) = mapSections[rowIndex, col];
            if (piece != null)
            {
                StartCoroutine(DestroyAfterTime(piece.gameObject));
                spawnedPieces.Remove(piece.gameObject);
            }

            mapSections[rowIndex, col] = (null, 0);
        }

        bool success = false;
        int attempts = 0;
        int maxAttempts = 25;

        while (!success && attempts < maxAttempts)
        {
            attempts++;
            bool rowValid = true;

            // Build the row from scratch
            for (int col = 0; col < cols; col++)
            {
                PlacePiece(rowIndex, col);

                // If a piece failed to place (no validPieces)
                if (mapSections[rowIndex, col].Item1 == null)
                {
                    rowValid = false;
                    break;
                }
            }

            // If row failed OR map becomes disconnected -> try again
            if (!rowValid || !IsFullyConnected())
            {
                // Clear partial row for the next attempt
                for (int col = 0; col < cols; col++)
                {
                    var (piece, rot) = mapSections[rowIndex, col];
                    if (piece != null)
                    {
                        Destroy(piece.gameObject);
                        spawnedPieces.Remove(piece.gameObject);
                    }

                    mapSections[rowIndex, col] = (null, 0);
                }
                yield return null;
                continue; // retry full regeneration
            }

            success = true; // row built successfully
        }

        // if a new map fails to generate in time then we just reuse the old layout
        if (!success)
        {
            for (int col = 0; col < cols; col++)
            {
                var (prefab, savedRotation) = backupRow[col];

                if (prefab != null)
                {
                    Vector3 pos = transform.position + new Vector3(col * cellSize, 0, -rowIndex * cellSize);
                    GameObject newPieceObj = Instantiate(prefab.gameObject, pos, Quaternion.Euler(0, savedRotation * -90f, 0));

                    ChordConnector newConnector = newPieceObj.GetComponent<ChordConnector>();
                    newConnector.originalPrefab = prefab;

                    mapSections[rowIndex, col] = (newConnector, savedRotation);
                    spawnedPieces.Add(newPieceObj);
                }
            }
        }
    }

    private IEnumerator DestroyAfterTime(GameObject blockGroup)
    {

        yield return new WaitForSeconds(0.5f);

        foreach (Transform child in blockGroup.transform)
        {
            if (child.gameObject.name.Contains("Block") || child.gameObject.name.Contains("Stuffy"))
            {
                Instantiate(poof, child.position, Quaternion.identity);
            }
        }

        Destroy(blockGroup);
    }

}
