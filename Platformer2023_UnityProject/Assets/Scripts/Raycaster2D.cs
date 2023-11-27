using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum BoxCorner
{
    UpperRight,
    UpperLeft,
    LowerRight,
    LowerLeft
}

public enum RayDirection
{
    Left,
    Right,
    Above,
    Below
}

public struct CollisionFlags
{
    public bool left, right, above, below;

    public void Reset()
    {
        left = right = above = below = false;
    }
}

public class Raycaster2D : MonoBehaviour
{
    public Transform selfTransform;
    public BoxCollider2D box;

    public float skinWidth = 0.02f;

    [Range(2, 10)] // affiche la propriété comme un slider
    public int numberOfRays = 5;

    public CollisionFlags collisionFlags;
    public UnityEvent<RayDirection> onCollisionEnter, onCollisionStay, onCollisionExit;

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!selfTransform) return;

        Gizmos.color = Color.yellow;
        DrawCornerGizmo(BoxCorner.LowerLeft);
        DrawCornerGizmo(BoxCorner.UpperLeft);
        Gizmos.color = Color.blue;
        DrawCornerGizmo(BoxCorner.UpperRight);
        DrawCornerGizmo(BoxCorner.LowerRight);
    }

    void DrawCornerGizmo(BoxCorner corner)
    {
        Vector2 cornerPos = GetCorner(corner);
        Vector3 spherePosition = new Vector3(cornerPos.x, cornerPos.y, selfTransform.position.z);
        Gizmos.DrawSphere(spherePosition, 0.1f);
    }
#endif

    public void Start()
    {
        collisionFlags.Reset();
    }

    // Used as ray origins for raycasting
    public Vector2 GetCorner(BoxCorner corner)
    {
        float horizontalSign = 1;
        if (corner == BoxCorner.UpperLeft || corner == BoxCorner.LowerLeft)
            horizontalSign = -1;

        float verticalSign = 1;
        if (corner == BoxCorner.LowerRight || corner == BoxCorner.LowerLeft)
            verticalSign = -1;

        float resultX = selfTransform.position.x + (box.offset.x + 0.5f * box.size.x * horizontalSign) * Mathf.Abs(selfTransform.lossyScale.x);
        float resultY = selfTransform.position.y + (box.offset.y + 0.5f * box.size.y * verticalSign) * Mathf.Abs(selfTransform.lossyScale.y);
        return new Vector2(resultX, resultY);
    }

    public bool CastRays(RayDirection direction, float distance)
    {
        // établir le point de départ du premier ray (un coin)
        Vector2 firstStartPoint = Vector2.zero;
        if (direction == RayDirection.Left) firstStartPoint = GetCorner(BoxCorner.UpperLeft);
        if (direction == RayDirection.Right) firstStartPoint = GetCorner(BoxCorner.UpperRight);
        if (direction == RayDirection.Above) firstStartPoint = GetCorner(BoxCorner.UpperLeft);
        if (direction == RayDirection.Below) firstStartPoint = GetCorner(BoxCorner.LowerLeft);

        // établir le point de départ du dernier ray (aussi un coin)
        Vector2 lastStartPoint = Vector2.zero;
        if (direction == RayDirection.Left) lastStartPoint = GetCorner(BoxCorner.LowerLeft);
        if (direction == RayDirection.Right) lastStartPoint = GetCorner(BoxCorner.LowerRight);
        if (direction == RayDirection.Above) lastStartPoint = GetCorner(BoxCorner.UpperRight);
        if (direction == RayDirection.Below) lastStartPoint = GetCorner(BoxCorner.LowerRight);

        // établir la direction du ray
        Vector2 rayDirection = Vector2.zero;
        if (direction == RayDirection.Left) rayDirection = Vector2.left;
        if (direction == RayDirection.Right) rayDirection = Vector2.right;
        if (direction == RayDirection.Above) rayDirection = Vector2.up;
        if (direction == RayDirection.Below) rayDirection = Vector2.down;

        // appliquer skinWidth pour éviter que l'objet ne s'auto-détecte comme un obstacle
        firstStartPoint += rayDirection * skinWidth;
        lastStartPoint += rayDirection * skinWidth;

        // selon la quantité de rays à envoyer, déduire les autres par interpolation
        for (int i = 0; i < numberOfRays; i++)
        {
            float ratio = (float)i / (float)(numberOfRays-1);

            Vector2 rayOrigin;
            // les deux lignes suivantes sont identiques
            //rayOrigin = firstStartPoint + (lastStartPoint - firstStartPoint) * ratio;
            rayOrigin = Vector2.Lerp(firstStartPoint, lastStartPoint, ratio);

            Debug.DrawRay(rayOrigin, rayDirection, Color.yellow, 0.3f);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, distance);

            // si on a bien heurté un objet, il faut mettre à jour les infos de collision
            if (hit.collider != null)
            {
                if (direction == RayDirection.Left)
                {
                    // s'il n'y avait pas encore de collision par ici, c'est qu'on a "entered"
                    if (!collisionFlags.left) onCollisionEnter?.Invoke(direction);
                    collisionFlags.left = true;
                }
                // idem pour les trois autres directions
                if (direction == RayDirection.Right)
                {
                    if (!collisionFlags.right) onCollisionEnter?.Invoke(direction);
                    collisionFlags.right = true;
                }
                if (direction == RayDirection.Above)
                {
                    if (!collisionFlags.above) onCollisionEnter?.Invoke(direction);
                    collisionFlags.above = true;
                }
                if (direction == RayDirection.Below)
                {
                    if (!collisionFlags.below) onCollisionEnter?.Invoke(direction);
                    collisionFlags.below = true;
                }

                onCollisionStay?.Invoke(direction);

                return true;
            }
        }

        // si on arrive jusqu'ici, c'est qu'aucun obstacle n'a été touché et qu'on n'est plus en collision dans l'autre sens

        if (direction == RayDirection.Left || direction == RayDirection.Right)
        {
            // s'il y avait une collision d'un côté ou de l'autre précédemment, maintenant ça n'est plus le cas 
            if (collisionFlags.left || collisionFlags.right)
                onCollisionExit?.Invoke(direction);
            // on peut donc marquer les flags (avant et arrière) comme false.
            collisionFlags.left = false;
            collisionFlags.right = false;
        }

        // même chose pour la verticalité
        if (direction == RayDirection.Above || direction == RayDirection.Below)
        {
            if (collisionFlags.above || collisionFlags.below)
                onCollisionExit?.Invoke(direction);
            collisionFlags.above = false;
            collisionFlags.below = false;
        }

        return false;
    }
}
