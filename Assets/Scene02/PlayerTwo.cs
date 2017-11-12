using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerTwo : MonoBehaviour
{
    public float playerSpeed = 3;

    public Dictionary<EquipType, int> equippedItemIDByType;

    private Animator playerAnimator;
    private Animator[] childAnimators;
    private ItemDatabase itemDatabase;
    private Dictionary<Equipper, bool> isEquippedByEquipper;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        childAnimators = GetComponentsInChildren<Animator>();
        itemDatabase = FindObjectOfType<ItemDatabase>();

        equippedItemIDByType = new Dictionary<EquipType, int>();
        equippedItemIDByType.Add(EquipType.Chestwear, 0); ////////////////////////// For testing TEST TEST TEST

        BaselineIsEquippedByEquipper();
        
        EquipItemSprites();
    }

    private void BaselineIsEquippedByEquipper()
    {
        isEquippedByEquipper = new Dictionary<Equipper, bool>();
        foreach (Equipper equipper in transform.GetComponentsInChildren<Equipper>())
        {
            isEquippedByEquipper.Add(equipper, false);
        }
    }

    private void EquipItemSprites()
    {
        foreach (Equipper equipper in transform.GetComponentsInChildren<Equipper>())
        {
            if (equippedItemIDByType.ContainsKey(equipper.equipType))
            {
                isEquippedByEquipper[equipper] = true;
                equipper.GetComponent<Animator>().runtimeAnimatorController = CreateEquipmentAnimator(equipper.equipType);
            }
            else
            {
                isEquippedByEquipper[equipper] = false;
                equipper.GetComponent<Animator>().runtimeAnimatorController = new RuntimeAnimatorController();
            }
        }
    }

    private AnimatorController CreateEquipmentAnimator(EquipType equipType)
    {
        AnimatorController playerAnimatorController = playerAnimator.runtimeAnimatorController as AnimatorController;
        AnimatorController newController = new AnimatorController();
        newController.name = "newController";
        newController.AddLayer(newController.MakeUniqueLayerName("BaseLayer"));

        AddPlayerAnimatorParametersToNewController(newController);
        AddPlayerAnimatorStatesToNewController(playerAnimatorController, newController, equipType);
        AddPlayerAnimatorTransitionsToNewController(playerAnimatorController, newController);

        return newController;
    }
    
    private void AddPlayerAnimatorParametersToNewController(AnimatorController newController)
    {
        foreach (AnimatorControllerParameter param in playerAnimator.parameters)
        {
            newController.AddParameter(param.name, param.type);
        }
    }

    private void AddPlayerAnimatorStatesToNewController(AnimatorController playerAnimatorController, AnimatorController newController, EquipType equipType)
    {
        AnimatorStateMachine rootStateMachine = newController.layers[0].stateMachine;
        foreach (ChildAnimatorState playerAnimatorState in playerAnimatorController.layers[0].stateMachine.states)
        {
            AnimatorState animatorState = new AnimatorState();
            animatorState.name = playerAnimatorState.state.name;

            animatorState.motion = GetAppropriateAnimationClip(equipType, animatorState.name);

            rootStateMachine.AddState(animatorState, playerAnimatorState.position);

            if (playerAnimatorState.state == playerAnimatorController.layers[0].stateMachine.defaultState)
            {
                rootStateMachine.defaultState = animatorState;
            }
        }
    }

    private AnimationClip GetAppropriateAnimationClip(EquipType equipType, string animStateName)
    {
        AnimationType animType;
        if (!Enum.IsDefined(typeof(AnimationType), animStateName))
        {
            Debug.LogWarning("Trouble in Equipper");
            animType = AnimationType.Fall;
        }
        else
        {
            animType = (AnimationType)Enum.Parse(typeof(AnimationType), animStateName);
        }
        return itemDatabase.itemsDictionary[equippedItemIDByType[equipType]].AnimClipDictionary[animType];
    }

    private void AddPlayerAnimatorTransitionsToNewController(AnimatorController playerAnimatorController, AnimatorController newController)
    {
        AnimatorStateMachine rootStateMachine = newController.layers[0].stateMachine;
        foreach (ChildAnimatorState playerAnimatorState in playerAnimatorController.layers[0].stateMachine.states)
        {
            foreach (ChildAnimatorState newAnimatorState in rootStateMachine.states)
            {
                if (playerAnimatorState.state.name == newAnimatorState.state.name)
                {
                    foreach (AnimatorStateTransition playerStateTransition in playerAnimatorState.state.transitions)
                    {
                        AnimatorStateTransition newStateTransition = new AnimatorStateTransition();

                        newStateTransition.name = playerStateTransition.name;
                        newStateTransition.hasExitTime = playerStateTransition.hasExitTime;
                        newStateTransition.canTransitionToSelf = playerStateTransition.canTransitionToSelf;
                        newStateTransition.conditions = playerStateTransition.conditions;
                        newStateTransition.duration = playerStateTransition.duration;

                        string nameOfDestinationState = playerStateTransition.destinationState.name;
                        foreach (ChildAnimatorState animatorState in rootStateMachine.states)
                        {
                            if (animatorState.state.name == nameOfDestinationState)
                            {
                                newStateTransition.destinationState = animatorState.state;
                                break;
                            }
                        }
                        newAnimatorState.state.AddTransition(newStateTransition);
                    }
                    break;
                }
            }
        }

        foreach (AnimatorStateTransition playerStateTransition in playerAnimatorController.layers[0].stateMachine.anyStateTransitions)
        {
            string nameOfDestinationState = playerStateTransition.destinationState.name;
            AnimatorState destinationState = new AnimatorState();
            foreach (ChildAnimatorState animatorState in rootStateMachine.states)
            {
                if (animatorState.state.name == nameOfDestinationState)
                {
                    destinationState = animatorState.state;
                    break;
                }
            }
            AnimatorStateTransition newStateTransition = rootStateMachine.AddAnyStateTransition(destinationState);
            newStateTransition.name = playerStateTransition.name;
            newStateTransition.hasExitTime = playerStateTransition.hasExitTime;
            newStateTransition.canTransitionToSelf = playerStateTransition.canTransitionToSelf;
            newStateTransition.conditions = playerStateTransition.conditions;
            newStateTransition.duration = playerStateTransition.duration;
        }
    }


    void Update()
    {
        AnimatePlayer();
        MovePlayer();

        ////////THIS IS A TEST/////////////
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
        ////////END OF TEST////////////
    }

    private void AnimatePlayer()
    {
        float x = 0;
        float y = 0;
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Abs(Input.GetAxis("Vertical")))
        {
            x = Input.GetAxis("Horizontal");
        }
        else
        {
            y = Input.GetAxis("Vertical");
        }

        GetComponent<Animator>().SetFloat("horizontalAxis", x);
        GetComponent<Animator>().SetFloat("verticalAxis", y);
        foreach (Equipper equipper in GetComponentsInChildren<Equipper>())
        {
            if (isEquippedByEquipper[equipper])
            {
                equipper.GetComponent<Animator>().SetFloat("horizontalAxis", x);
                equipper.GetComponent<Animator>().SetFloat("verticalAxis", y);
            }
        }
    }

    private void MovePlayer()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");
            float xAndY = Mathf.Sqrt(Mathf.Pow(moveX, 2) + Mathf.Pow(moveY, 2));
            transform.Translate(moveX * playerSpeed * Time.deltaTime / xAndY, moveY * playerSpeed * Time.deltaTime / xAndY, transform.position.z, Space.Self);
        }
    }
}
