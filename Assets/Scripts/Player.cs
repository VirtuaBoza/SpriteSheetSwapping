using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float playerSpeed = 3;
    public Dictionary<EquipType, int> equippedItemIDByType;
    private Equipper[] equippers;

    void Start()
    {
        equippers = GetComponentsInChildren<Equipper>();
        BaselineEquippedItemIDByType();

        // TEST TEST TEST
        equippedItemIDByType[EquipType.Chestwear] = 0;
        // END OF TEST

        EquipItemSprites();
    }

    private void BaselineEquippedItemIDByType()
    {
        equippedItemIDByType = new Dictionary<EquipType, int>();
        foreach (EquipType equipType in Enum.GetValues(
            typeof(EquipType)))
        {
            equippedItemIDByType.Add(equipType, -1);
        }
    }

    public void EquipItemSprites()
    {
        foreach (Equipper equipper in 
            GetComponentsInChildren<Equipper>())
        {
            equipper.CreateItemAnimatorFromPlayerAnimator(
                equippedItemIDByType[equipper.equipType], 
                GetComponent<Animator>());
        }
    }

    void Update()
    {
        AnimatePlayer();
        MovePlayer();

        // TEST TEST TEST
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            equippedItemIDByType[EquipType.Chestwear] = 0;
            EquipItemSprites();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            equippedItemIDByType[EquipType.Chestwear] = 1;
            EquipItemSprites();
        }
        // END OF TEST
    }

    private void AnimatePlayer()
    {
        float x = 0;
        float y = 0;
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 
            Mathf.Abs(Input.GetAxis("Vertical")))
        {
            x = Input.GetAxis("Horizontal");
        }
        else
        {
            y = Input.GetAxis("Vertical");
        }

        GetComponent<Animator>().SetFloat("horizontalAxis", x);
        GetComponent<Animator>().SetFloat("verticalAxis", y);
        foreach (Equipper equipper in equippers)
        {
            if (equipper.hasItemEquipped)
            {
                equipper.animator.SetFloat("horizontalAxis", x);
                equipper.animator.SetFloat("verticalAxis", y);
            }
        }
    }

    private void MovePlayer()
    {
        if (Input.GetAxis("Horizontal") != 0 || 
            Input.GetAxis("Vertical") != 0)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");
            float xAndY = Mathf.Sqrt(Mathf.Pow(moveX, 2) + 
                Mathf.Pow(moveY, 2));
            transform.Translate(moveX * playerSpeed * Time.deltaTime / 
                xAndY, moveY * playerSpeed * Time.deltaTime / 
                xAndY, transform.position.z, Space.Self);
        }
    }
}
