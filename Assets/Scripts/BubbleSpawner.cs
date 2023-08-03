using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    [SerializeField] private BoundariesInfo boundaries;
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private GameObject goalPrefab;

    public void SpawnBubbles()
    {
        // Get the bubble size
        Vector3 bubbleSize = bubblePrefab.GetComponent<SpriteRenderer>().bounds.size;

        // Get startpos and endpos
        Vector3 startPos = boundaries.corner1;
        Vector3 endPos = boundaries.corner2;

        // Calculate the number of rows and columns
        float totalWidth = Mathf.Abs(endPos.x - startPos.x);
        float totalHeight = Mathf.Abs(endPos.y - startPos.y);
        int rows = Mathf.FloorToInt(totalHeight / bubbleSize.y);
        int columns = Mathf.FloorToInt(totalWidth / bubbleSize.x);

        // Calculate the offset for the bubbles in the grid
        float offsetX = (totalWidth - (columns * bubbleSize.x)) / 2;
        float offsetY = (totalHeight - (rows * bubbleSize.y)) / 2;

        Debug.Log("Grid Dimensions: Rows=" + rows + ", Columns=" + columns);

        // Get a random row & column to put the goal in
        int goalRow = Random.Range(0, rows);
        int goalCol = Random.Range(0, columns);
        
        // Spawn the bubbles in a grid
        for (int row = 0; row < rows; row++)
        {

            for (int col = 0; col < columns; col++)
            {
                // Skip the center block
                if (col == columns / 2 && row == rows / 2)
                    continue;
                
                // Spawn the goal in the random row & column
                if (col == goalCol && row == goalRow)
                {
                    // Calculate the position for the current bubble
                    // Invert the offsetY to flip the Y-coordinate
                    Vector3 goalSpawnPos = new Vector3(startPos.x + offsetX + col * bubbleSize.x,
                        startPos.y - offsetY - row * bubbleSize.y, 0f);

                    // Instantiate the goal at the calculated position
                    Instantiate(goalPrefab, goalSpawnPos, Quaternion.identity);
                    
                    Debug.Log("Spawned goal at Row=" + row + ", Column=" + col + ", Position=" + goalSpawnPos);
                    continue;
                }

                // Calculate the position for the current bubble
                // Invert the offsetY to flip the Y-coordinate
                Vector3 spawnPos = new Vector3(startPos.x + offsetX + col * bubbleSize.x,
                    startPos.y - offsetY - row * bubbleSize.y, 0f);

                // Instantiate the bubble at the calculated position
                Instantiate(bubblePrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}
