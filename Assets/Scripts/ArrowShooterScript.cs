using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShooterScript : MonoBehaviour
{
    CursorGameScript myBoss;

    public static ArrowShooterScript instance;
    public int level = 0;

    public GameObject projectilePrefab;
    public GameObject[] thisLevProjectiles;
    int projectileMadeCounter = 0;

    public Transform leftTarget;
    public Transform rightTarget;
    public Transform upTarget;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
    }

    public void CreateProjectileArray(int howManyToPrep, CursorGameScript boss)
    {
        thisLevProjectiles = new GameObject[howManyToPrep];
        myBoss = boss;
    }


    public void ShootArrow(byte lane)
    {
        GameObject newArrow = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        thisLevProjectiles[projectileMadeCounter] = newArrow;
        projectileMadeCounter++;
        if (projectileMadeCounter == thisLevProjectiles.Length)
        {
            newArrow.GetComponent<ProjectileScript>().last = true;
            newArrow.GetComponent<ProjectileScript>().boss = myBoss;
        }
        switch (lane) {
            case 0:
                newArrow.transform.up = leftTarget.transform.position - newArrow.transform.position;
                newArrow.GetComponent<ProjectileScript>().targetPos = leftTarget.transform.position;
                break;
            case 1:
                newArrow.transform.up = upTarget.transform.position - newArrow.transform.position;
                newArrow.GetComponent<ProjectileScript>().targetPos = upTarget.transform.position;
                break;
            case 2:
                newArrow.transform.up = rightTarget.transform.position - newArrow.transform.position;
                newArrow.GetComponent<ProjectileScript>().targetPos = rightTarget.transform.position;
                break;
            default:
                newArrow.transform.up = leftTarget.transform.position - newArrow.transform.position;
                break;
        }
    }
}
