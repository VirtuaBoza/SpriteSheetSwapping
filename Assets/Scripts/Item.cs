using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

public class Item
{
    private const float framesPerSecond = 8f;
    private EquipType equipType;
    public string spriteSheetName;

    public int ID { get; private set; }
    public EquipType EquipType => equipType;
    public Dictionary<AnimationType, AnimationClip> AnimClipDictionary 
    { get; private set; }

    public Item(int id, string spriteSheetName, string equipTypeString)
    {
        if(!Enum.TryParse(equipTypeString, out equipType))
        {
            throw new ArgumentException();
        }

        ID = id;
        this.spriteSheetName = spriteSheetName;

        AnimClipDictionary = 
            new Dictionary<AnimationType, AnimationClip>();

        CreateAnimationClips();
    }

    private void CreateAnimationClips()
    {
        var sprites = Resources.LoadAll<Sprite>("Spritesheets/" + 
            spriteSheetName);

        foreach (AnimationType animationType in Enum.GetValues(
            typeof(AnimationType)))
        {
            var animClip = new AnimationClip
            {
                name = $"{spriteSheetName} {animationType}"
            };

            var spriteBinding = new EditorCurveBinding
            {
                type = typeof(SpriteRenderer),
                path = string.Empty,
                propertyName = "m_Sprite"
            };

            var startAndRange = GetSpriteStartIndexAndRange(
                animationType);
            var spriteKeyFrames = 
                new ObjectReferenceKeyframe[startAndRange.Range];
            float timeValue = 0;
            for (int i = 0; i < startAndRange.Range; i++)
            {
                spriteKeyFrames[i] = new ObjectReferenceKeyframe();
                spriteKeyFrames[i].time = timeValue;
                spriteKeyFrames[i].value = 
                    sprites[i + startAndRange.StartIndex];
                timeValue += 1 / framesPerSecond;
            }
            AnimationUtility.SetObjectReferenceCurve(
                animClip, spriteBinding, spriteKeyFrames);
            
            var animClipSettings = new AnimationClipSettings
            {
                loopTime = true
            };
            
            AnimationUtility.SetAnimationClipSettings(
                animClip, animClipSettings);
            animClip.frameRate = framesPerSecond;
            AnimClipDictionary.Add(animationType, animClip);
        }
    }

    private SpriteSheetAnimationInfo GetSpriteStartIndexAndRange(
        AnimationType animationType)
    {
        switch (animationType)
        {
            case AnimationType.IdleUp:
                return new SpriteSheetAnimationInfo(60, 1);
            case AnimationType.IdleLeft:
                return new SpriteSheetAnimationInfo(69, 1);
            case AnimationType.IdleDown:
                return new SpriteSheetAnimationInfo(78, 1);
            case AnimationType.IdleRight:
                return new SpriteSheetAnimationInfo(87, 1);

            case AnimationType.WalkUp:
                return new SpriteSheetAnimationInfo(61, 8);
            case AnimationType.WalkLeft:
                return new SpriteSheetAnimationInfo(70, 8);
            case AnimationType.WalkDown:
                return new SpriteSheetAnimationInfo(79, 8);
            case AnimationType.WalkRight:
                return new SpriteSheetAnimationInfo(88, 8);

            case AnimationType.SlashUp:
                return new SpriteSheetAnimationInfo(96, 6);
            case AnimationType.SlashLeft:
                return new SpriteSheetAnimationInfo(102, 6);
            case AnimationType.SlashDown:
                return new SpriteSheetAnimationInfo(108, 6);
            case AnimationType.SlashRight:
                return new SpriteSheetAnimationInfo(114, 6);

            case AnimationType.ThrustUp:
                return new SpriteSheetAnimationInfo(28, 8);
            case AnimationType.ThrustLeft:
                return new SpriteSheetAnimationInfo(36, 8);
            case AnimationType.ThrustDown:
                return new SpriteSheetAnimationInfo(44, 8);
            case AnimationType.ThrustRight:
                return new SpriteSheetAnimationInfo(52, 8);

            case AnimationType.LooseUp:
                return new SpriteSheetAnimationInfo(120, 13);
            case AnimationType.LooseLeft:
                return new SpriteSheetAnimationInfo(133, 13);
            case AnimationType.LooseDown:
                return new SpriteSheetAnimationInfo(146, 13);
            case AnimationType.LooseRight:
                return new SpriteSheetAnimationInfo(159, 13);

            case AnimationType.SpellcastUp:
                return new SpriteSheetAnimationInfo(0, 7);
            case AnimationType.SpellcastLeft:
                return new SpriteSheetAnimationInfo(7, 7);
            case AnimationType.SpellcastDown:
                return new SpriteSheetAnimationInfo(14, 7);
            case AnimationType.SpellcastRight:
                return new SpriteSheetAnimationInfo(21, 7);

            case AnimationType.Fall:
                return new SpriteSheetAnimationInfo(172, 6);

            default:
                throw new InvalidEnumArgumentException();
        }
    }
}

public struct SpriteSheetAnimationInfo
{
    public SpriteSheetAnimationInfo(int startIndex, int range)
    {
        StartIndex = startIndex;
        Range = range;
    }

    public int StartIndex { get; private set; }
    public int Range { get; private set; }
}