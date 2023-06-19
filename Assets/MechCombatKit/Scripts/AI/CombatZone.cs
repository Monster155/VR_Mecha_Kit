using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatZone : MonoBehaviour
{
    [Tooltip("The bounds that describe the combat zone.")]
    [SerializeField]
    protected Bounds bounds;

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.TransformPoint(bounds.center), transform.rotation, transform.lossyScale);
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(Vector3.zero, bounds.size);
    }
    
    /// <summary>
    /// Clamp a specified position to within the combat zone bounds.
    /// </summary>
    /// <param name="position">The world position.</param>
    /// <param name="referencePosition"></param>
    /// <returns></returns>
    public virtual Vector3 ClampToBounds(Vector3 position)
    {

        Vector3 localPos = transform.InverseTransformPoint(position);

        localPos.x = Mathf.Clamp(localPos.x, -bounds.extents.x, bounds.extents.x);
        localPos.y = Mathf.Clamp(localPos.y, -bounds.extents.y, bounds.extents.y);
        localPos.z = Mathf.Clamp(localPos.z, -bounds.extents.z, bounds.extents.z);

        return transform.TransformPoint(localPos);

    }
}
