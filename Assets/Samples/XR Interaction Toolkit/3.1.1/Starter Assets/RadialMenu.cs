using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class RadialMenu : MonoBehaviour
{
    ObjectSpawner m_ObjectSpawner;
    ARPlane _arPlane;
    public GameObject menuUi;
    public Animator animator;
    ARRaycastHit _arRaycastHit;

    public void Start()
    {
        m_ObjectSpawner = FindAnyObjectByType<ObjectSpawner>();
    }

    public void SpawnSelectedObject()
    {
        m_ObjectSpawner.TrySpawnObject(_arRaycastHit.pose.position, _arPlane.normal);
        HideRadialMenu();
    }

    public void ShowRadialMenu(ARRaycastHit arRaycastHit, ARPlane arPlane)
    {
        _arPlane = arPlane;
        _arRaycastHit = arRaycastHit;
        menuUi.SetActive(true);
        animator.SetBool("Show", true);
    }

    public void HideRadialMenu()
    {
        animator.SetBool("Show", false);
        StartCoroutine(HideWithDelay());
    }

    public IEnumerator HideWithDelay()
    {
        yield return new WaitForSeconds(0.2f);
        menuUi.SetActive(false);
    }
}
