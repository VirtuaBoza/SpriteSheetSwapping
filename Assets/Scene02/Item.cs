using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum EquipType
{
    Footwear,
    Legwear,
    Handwear,
    Chestwear,
    Waistwear,
    Backwear,
    Neckwear,
    Headwear,
    RightHanded,
    LeftHanded
}

public enum AnimationType
{
    IdleUp, IdleLeft, IdleDown, IdleRight,
    WalkUp, WalkLeft, WalkDown, WalkRight,
    SlashUp, SlashLeft, SlashDown, SlashRight,
    ThrustUp, ThrustLeft, ThrustDown, ThrustRight,
    LooseUp, LooseLeft, LooseDown, LooseRight,
    SpellcastUp, SpellcastLeft, SpellcastDown, SpellcastRight,
    Fall
}

public class Item
{
    public int ID { get; private set; }
    public string SpriteSheetName { get; private set; }
    public EquipType EquipType { get; private set; }
    public Dictionary<AnimationType, AnimationClip> AnimClipDictionary { get; private set; }

    public Item(int id, string spriteSheetName, string equipTypeString)
    {
        ID = id;
        SpriteSheetName = spriteSheetName;

        if (!Enum.IsDefined(typeof(EquipType), equipTypeString))
        {
            Debug.LogWarning("Trouble in Item constructor");
        }
        else
        {
            EquipType = (EquipType)Enum.Parse(typeof(EquipType), equipTypeString);
        }

        AnimClipDictionary = new Dictionary<AnimationType, AnimationClip>();
        CreateAnimationClips();
    }

    private void CreateAnimationClips()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Spritesheets/" + SpriteSheetName);

        foreach (AnimationType animationType in Enum.GetValues(typeof(AnimationType)))
        {
            AnimationClip animClip = new AnimationClip();
            animClip.name = SpriteSheetName + " " + animationType.ToString();
            

            EditorCurveBinding spriteBinding = new EditorCurveBinding();
            spriteBinding.type = typeof(SpriteRenderer);
            spriteBinding.path = "";
            spriteBinding.propertyName = "m_Sprite";

            int[] startAndRange = GetSpriteStartIndexAndRange(animationType);
            ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[startAndRange[1]];
            float timeValue = 0;
            for (int i = 0; i < startAndRange[1]; i++)
            {
                spriteKeyFrames[i] = new ObjectReferenceKeyframe();
                spriteKeyFrames[i].time = timeValue;
                spriteKeyFrames[i].value = sprites[i + startAndRange[0]];
                timeValue += 1 / 15f;
            }
            AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, spriteKeyFrames);
            
            AnimationClipSettings animClipSett = new AnimationClipSettings();
            animClipSett.loopTime = true;
            
            AnimationUtility.SetAnimationClipSettings(animClip, animClipSett);
            animClip.frameRate = 15f;
            AnimClipDictionary.Add(animationType, animClip);
        }
    }

    private int[] GetSpriteStartIndexAndRange(AnimationType animationType)
    {
        switch (animationType)
        {
            case AnimationType.IdleUp:
                return new int[] { 60, 1 };
            case AnimationType.IdleLeft:
                return new int[] { 69, 1 };
            case AnimationType.IdleDown:
                return new int[] { 78, 1 };
            case AnimationType.IdleRight:
                return new int[] { 87, 1 };

            case AnimationType.WalkUp:
                return new int[] { 61, 8 };
            case AnimationType.WalkLeft:
                return new int[] { 70, 8 };
            case AnimationType.WalkDown:
                return new int[] { 79, 8 };
            case AnimationType.WalkRight:
                return new int[] { 88, 8 };

            case AnimationType.SlashUp:
                return new int[] { 96, 6 };
            case AnimationType.SlashLeft:
                return new int[] { 102, 6 };
            case AnimationType.SlashDown:
                return new int[] { 108, 6 };
            case AnimationType.SlashRight:
                return new int[] { 114, 6 };

            case AnimationType.ThrustUp:
                return new int[] { 28, 8 };
            case AnimationType.ThrustLeft:
                return new int[] { 36, 8 };
            case AnimationType.ThrustDown:
                return new int[] { 44, 8 };
            case AnimationType.ThrustRight:
                return new int[] { 52, 8 };

            case AnimationType.LooseUp:
                return new int[] { 120, 13 };
            case AnimationType.LooseLeft:
                return new int[] { 133, 13 };
            case AnimationType.LooseDown:
                return new int[] { 146, 13 };
            case AnimationType.LooseRight:
                return new int[] { 159, 13 };

            case AnimationType.SpellcastUp:
                return new int[] { 0, 7 };
            case AnimationType.SpellcastLeft:
                return new int[] { 7, 7 };
            case AnimationType.SpellcastDown:
                return new int[] { 14, 7 };
            case AnimationType.SpellcastRight:
                return new int[] { 21, 7 };

            case AnimationType.Fall:
                return new int[] { 172, 6 };

            default:
                Debug.LogWarning("Problem with Item.cs");
                return new int[] { 0, 0 };
        }
    }
}