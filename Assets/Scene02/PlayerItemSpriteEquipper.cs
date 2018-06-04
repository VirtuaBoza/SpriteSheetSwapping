using System;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerItemSpriteEquipper : MonoBehaviour
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

    public void CreateItemAnimatorFromPlayerAnimator(int itemID, Animator playerAnimator)
    {
        if (itemID >= 0)
        {
            hasItemEquipped = true;
            AnimatorController newController = new AnimatorController();
            newController.name = "newController";
            newController.AddLayer(newController.MakeUniqueLayerName("BaseLayer"));

            AddPlayerAnimatorParametersToNewController(playerAnimator, newController);
            AddPlayerAnimatorStatesToNewController(playerAnimator, newController, itemID);
            AddPlayerAnimatorTransitionsToNewController(playerAnimator, newController);

            GetComponent<Animator>().runtimeAnimatorController = newController;
        }
        else
        {
            hasItemEquipped = false;
            GetComponent<Animator>().runtimeAnimatorController = null;
        }
    }

    private void AddPlayerAnimatorParametersToNewController(Animator playerAnimator, AnimatorController newController)
    {
        foreach (AnimatorControllerParameter param in playerAnimator.parameters)
        {
            newController.AddParameter(param.name, param.type);
        }
    }

    private void AddPlayerAnimatorStatesToNewController(Animator playerAnimator, AnimatorController newController, int itemID)
    {
        AnimatorController playerAnimatorController = playerAnimator.runtimeAnimatorController as AnimatorController;
        AnimatorStateMachine rootStateMachine = newController.layers[0].stateMachine;
        foreach (ChildAnimatorState playerAnimatorState in playerAnimatorController.layers[0].stateMachine.states)
        {
            AnimatorState animatorState = new AnimatorState();
            animatorState.name = playerAnimatorState.state.name;

            animatorState.motion = GetAppropriateAnimationClip(animatorState.name, itemID);

            rootStateMachine.AddState(animatorState, playerAnimatorState.position);

            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(playerAnimatorState.state.name))
            {
                rootStateMachine.defaultState = animatorState;
            }
        }
    }

    private Motion GetAppropriateAnimationClip(string stateName, int itemID)
    {
        AnimationType animType;
        if (!Enum.IsDefined(typeof(AnimationType), stateName))
        {
            throw new ArgumentException("GetAppropriateAnimationClip was passed a state name from which an AnimationType cannot be parsed.");
        }
        else
        {
            animType = (AnimationType)Enum.Parse(typeof(AnimationType), stateName);
        }
        return itemDatabase.itemsDictionary[itemID].AnimClipDictionary[animType];
    }

    private void AddPlayerAnimatorTransitionsToNewController(Animator playerAnimator, AnimatorController newController)
    {
        AnimatorController playerAnimatorController = playerAnimator.runtimeAnimatorController as AnimatorController;
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
}
