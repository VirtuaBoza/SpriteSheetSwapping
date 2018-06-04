using System;
using UnityEditor.Animations;
using UnityEngine;

public class Equipper : MonoBehaviour
{
    public Animator animator;
    public EquipType equipType;
    public bool hasItemEquipped = false;

    private ItemDatabase itemDatabase;

    void Start()
    {
        animator = GetComponent<Animator>();
        itemDatabase = FindObjectOfType<ItemDatabase>();
    }

    public void CreateItemAnimatorFromPlayerAnimator(int itemID, 
        Animator playerAnimator)
    {
        if (itemID >= 0)
        {
            hasItemEquipped = true;
            var newController = new AnimatorController
            {
                name = "newController"
            };
            newController.AddLayer(
                newController.MakeUniqueLayerName("BaseLayer"));

            AddPlayerAnimatorParametersToNewController(
                playerAnimator, newController);

            AddPlayerAnimatorStatesToNewController(
                playerAnimator, newController, itemID);

            AddPlayerAnimatorTransitionsToNewController(
                playerAnimator, newController);

            GetComponent<Animator>().runtimeAnimatorController = 
                newController;
        }
        else
        {
            hasItemEquipped = false;
            GetComponent<Animator>().runtimeAnimatorController = null;
        }
    }

    private void AddPlayerAnimatorParametersToNewController(
        Animator playerAnimator, AnimatorController newController)
    {
        foreach (var param in playerAnimator.parameters)
        {
            newController.AddParameter(param.name, param.type);
        }
    }

    private void AddPlayerAnimatorStatesToNewController(
        Animator playerAnimator, 
        AnimatorController newController, 
        int itemID)
    {
        var playerAnimatorController = 
            (AnimatorController)playerAnimator
            .runtimeAnimatorController;

        var rootStateMachine = 
            newController.layers[0].stateMachine;

        foreach (var playerAnimatorState in 
            playerAnimatorController.layers[0].stateMachine.states)
        {
            AnimatorState animatorState = new AnimatorState();
            animatorState.name = playerAnimatorState.state.name;

            animatorState.motion = GetAppropriateAnimationClip(
                animatorState.name, itemID);

            rootStateMachine.AddState(animatorState, 
                playerAnimatorState.position);

            if (playerAnimator.GetCurrentAnimatorStateInfo(0)
                .IsName(playerAnimatorState.state.name))
            {
                rootStateMachine.defaultState = animatorState;
            }
        }
    }

    private Motion GetAppropriateAnimationClip(string stateName, 
        int itemID)
    {
        AnimationType animType;
        if (!Enum.TryParse(stateName, out animType))
        {
            throw new ArgumentException("GetAppropriateAnimationClip " +
            "was passed a state name from which an AnimationType " +
            "cannot be parsed.");
        }

        return itemDatabase.itemsDictionary[itemID]
            .AnimClipDictionary[animType];
    }

    private void AddPlayerAnimatorTransitionsToNewController(
        Animator playerAnimator, AnimatorController newController)
    {
        var playerAnimatorController = 
            (AnimatorController)playerAnimator
            .runtimeAnimatorController;

        var rootStateMachine = newController.layers[0].stateMachine;

        foreach (var playerAnimatorState in 
            playerAnimatorController.layers[0].stateMachine.states)
        {
            foreach (var newAnimatorState in rootStateMachine.states)
            {
                if (playerAnimatorState.state.name == 
                    newAnimatorState.state.name)
                {
                    foreach (var playerStateTransition in 
                        playerAnimatorState.state.transitions)
                    {
                        var newStateTransition = 
                            new AnimatorStateTransition
                        {
                            name = playerStateTransition
                                .name,
                            hasExitTime = playerStateTransition
                                .hasExitTime,
                            canTransitionToSelf = playerStateTransition
                                .canTransitionToSelf,
                            conditions = playerStateTransition
                                .conditions,
                            duration = playerStateTransition
                                .duration
                        };

                        var nameOfDestinationState = 
                            playerStateTransition.destinationState.name;
                        foreach (var animatorState in 
                            rootStateMachine.states)
                        {
                            if (animatorState.state.name == 
                                nameOfDestinationState)
                            {
                                newStateTransition.destinationState = 
                                    animatorState.state;
                                break;
                            }
                        }
                        newAnimatorState.state.AddTransition(
                            newStateTransition);
                    }
                    break;
                }
            }
        }

        foreach (var playerStateTransition in playerAnimatorController
            .layers[0].stateMachine.anyStateTransitions)
        {
            var nameOfDestinationState = 
                playerStateTransition.destinationState.name;
            var destinationState = new AnimatorState();
            foreach (var animatorState in rootStateMachine.states)
            {
                if (animatorState.state.name == nameOfDestinationState)
                {
                    destinationState = animatorState.state;
                    break;
                }
            }
            var newStateTransition = 
                rootStateMachine
                .AddAnyStateTransition(destinationState);
            newStateTransition.name = 
                playerStateTransition.name;
            newStateTransition.hasExitTime = 
                playerStateTransition.hasExitTime;
            newStateTransition.canTransitionToSelf = 
                playerStateTransition.canTransitionToSelf;
            newStateTransition.conditions = 
                playerStateTransition.conditions;
            newStateTransition.duration = 
                playerStateTransition.duration;
        }
    }
}
