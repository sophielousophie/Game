/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private GameObject PipePrefab;
    [SerializeField] private float delai = 2f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnPipe), 0f, delai);
    }

    void SpawnPipe()
    {
        GameObject pipe = Instantiate(
            PipePrefab,
            transform.position,
            Quaternion.identity
        );

        Debug.Log("Tuyau créé en X = " + pipe.transform.position.x);
    }

    public void StopSpawn()
    {
        CancelInvoke(nameof(SpawnPipe));
    }
}*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField]
    GameObject PipePrefab;
    [SerializeField]
    float delai = 1f;
    
    void Start()
    {
        InvokeRepeating("SpawnPipe", 0f, delai);
    }

   void SpawnPipe()
    {
        Instantiate(PipePrefab);
    }

    public void StopSpawn()
    {
        CancelInvoke();
    }

    
}
