using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VSX.UniversalVehicleCombat;
using System;

public class HUDDisplayedDamageable : MonoBehaviour
{

    [SerializeField]
    protected string damageableID;
    public string DamageableID
    {
        get { return damageableID; }
    }

    protected Damageable connectedDamageable;

    [SerializeField]
    protected GameObject visualElements;

    [SerializeField]
    protected List<Image> images = new List<Image>();

    public Gradient healthColorGradient;

    public Color destroyedColor = new Color(0f, 0f, 0f, 0.33f);

    [Header("Events")]

    public UnityEvent onDamaged;

    protected UnityAction<float, Vector3, HealthModifierType, Transform> onDamagedAction;



    protected virtual void Reset()
    {
        visualElements = gameObject;

        healthColorGradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[] { new GradientColorKey(new Color(1, 0.1f, 0.1f, 1f), 0f), new GradientColorKey(new Color(1, 0.75f, 0.25f, 1f), 1f) };
        healthColorGradient.colorKeys = colorKeys;

        images = new List<Image>(transform.GetComponentsInChildren<Image>());
    }

    public void Connect(Damageable damageable)
    {
        Disconnect();
        connectedDamageable = damageable;
        onDamagedAction = delegate { OnDamaged(); };
        damageable.onDamaged.AddListener(onDamagedAction);
    }

    public void Disconnect()
    {
        if (connectedDamageable != null)
        {
            connectedDamageable.onDamaged.RemoveListener(onDamagedAction);
        }
    }

    protected void OnDamaged()
    {
        onDamaged.Invoke();
    }

    private void Update()
    {
        if (connectedDamageable == null)
        {
            visualElements.SetActive(false);
            return;
        }

        visualElements.SetActive(true);

        float healthFraction = connectedDamageable.HealthCapacity == 0 ? 0 : (connectedDamageable.CurrentHealth / connectedDamageable.HealthCapacity);
        
        for(int i = 0; i < images.Count; ++i)
        {
            if (connectedDamageable.Destroyed)
            {
                images[i].color = destroyedColor;
            }
            else
            {
                images[i].color = healthColorGradient.Evaluate(healthFraction);
            }
        }

    }
}
