using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MyLayerMask
{
    Default         = 0 << 0,   // 00000000 00000000 00000000 00000000   
    BuiltInLayer0   = 1 << 0,   // ... 00000001
    BuiltInLayer1   = 1 << 1,   // ... 00000010
    BuiltInLayer2   = 1 << 2,   // ... 00000100 
    UserLayer3      = 1 << 3,   // ... 00001000 // ground 
    BuiltInLayer4   = 1 << 4,   // ... 00010000 // water
    BuiltInLayer5   = 1 << 5,   // ... 00100000
    UserLayer6      = 1 << 6,   // ... 01000000
    UserLayer7      = 1 << 7,   // ... 10000000

    UserLayer3_Or_BuiltInLayer4 = UserLayer3 | BuiltInLayer4, // ... 00001100
}

public class Caster : MonoBehaviour
{
    [SerializeField] LayerMask _targetMask;

    private void OnEnable()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit,float.PositiveInfinity, _targetMask))
        { 
            if(hit.collider.TryGetComponent(out IHp hpInterface))
            {
                hpInterface.DepleteHp(10.0f);
            }
        }
    }
}
