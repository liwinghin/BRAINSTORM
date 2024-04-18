using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera mainCam;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayerMask;
    

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCam.nearClipPlane;

        Ray ray = mainCam.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
}