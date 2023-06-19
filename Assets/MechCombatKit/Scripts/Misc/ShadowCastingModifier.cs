using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCastingModifier : MonoBehaviour
{
    public void CastShadowsOnly()
    {
        GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    }

    public void CastShadows()
    {
        GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }
}
