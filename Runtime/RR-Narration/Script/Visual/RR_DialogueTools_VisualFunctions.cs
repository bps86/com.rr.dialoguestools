using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class RR_DialogueTools_FunctionsVisual
{
    public static Image SetActorSprite(Image actorImage, Sprite targetSprite, Vector3 targetPosition, Vector3 targetScale) {
        actorImage.sprite = targetSprite;
        actorImage.color = Color.white;
        actorImage.gameObject.transform.localPosition = targetPosition;
        actorImage.gameObject.transform.localScale = targetScale;
        return actorImage;
    }

    public static Image ResetActorSprite(Image actorSprite) {
        actorSprite.sprite = null;
        actorSprite.color = Color.clear;
        return actorSprite;
    }
    public static SkeletonGraphic SetActorSkeletonGraphics(SkeletonGraphic actorSkeletonGraphics, SkeletonDataAsset targetSkeletonDataAsset, string animationName, Vector3 targetPosition, Vector3 targetScale, bool isLoop) {
        actorSkeletonGraphics.skeletonDataAsset = targetSkeletonDataAsset;
        actorSkeletonGraphics.startingAnimation = animationName;
        actorSkeletonGraphics.startingLoop = isLoop;
        actorSkeletonGraphics.rectTransform.localPosition = targetPosition;
        actorSkeletonGraphics.rectTransform.localScale = targetScale;
        actorSkeletonGraphics.color = Color.white;
        return actorSkeletonGraphics;
    }

    public static void SetResetAnim(SkeletonGraphic skeletonGraphic, string defaultAnimation) {
        skeletonGraphic.AnimationState.Complete += delegate {
            ResetAnim(skeletonGraphic, defaultAnimation);
        };
    }

    private static void ResetAnim(SkeletonGraphic skeletonGraphic, string defaultAnimation) {
        skeletonGraphic.AnimationState.SetAnimation(0, defaultAnimation, true);
    }
}
