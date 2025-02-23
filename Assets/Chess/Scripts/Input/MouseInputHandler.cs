using Chess.Scripts.Core;
using UnityEngine;

public class MouseInputHandler : MonoBehaviour
{
    //keeping track of selected placement handler
    private ChessPlayerPlacementHandler _selectedPlacementHandler;

    private void Update()
    {
        ProcessMouseClick();
    }

    //Processes mouse clicks and selection of piece..
    private void ProcessMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Convert mouse position to world position
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0f; // Ensure ray is on the 2D plane

            // Raycast from the camera position to mouse position
            RaycastHit2D hit = Physics2D.Raycast((Vector2)worldPosition, Vector2.zero);

            if (hit.collider != null)
            {
                ChessPlayerPlacementHandler clikedHandler = hit.collider.GetComponent<ChessPlayerPlacementHandler>();

                if (clikedHandler != null)
                {
                    //if selected handler is not clickedHandler remove its highlights
                    if (_selectedPlacementHandler != null && _selectedPlacementHandler != clikedHandler)
                    {
                        _selectedPlacementHandler.ClearAllHighlights();
                    }
                    //set selectedHandler to clikedHandler
                    _selectedPlacementHandler = clikedHandler;
                    _selectedPlacementHandler.ShowPossibleMoves();
                }
            }
            else
            {
                //show nothing if collider not found on tile..
                if (_selectedPlacementHandler != null)
                {
                    _selectedPlacementHandler.ClearAllHighlights();
                    _selectedPlacementHandler = null;
                }
                Debug.Log("No collider found");
            }
        }
    }
}
