using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class RR_DialogueTools_FunctionsVisual
{
    public static Image SetActorSprite(Image actorImage, Vector3 targetPosition, Vector3 targetScale, Color selectedColor) {
        actorImage.color = selectedColor;
        actorImage.rectTransform.anchoredPosition = targetPosition;
        actorImage.rectTransform.localScale = targetScale;
        return actorImage;
    }

    public static SkeletonGraphic SetActorSkeletonGraphics(SkeletonGraphic actorSkeletonGraphics, string animationName, Vector3 targetPosition, Vector3 targetScale, Color selectedColor, bool isLoop) {
        actorSkeletonGraphics.startingAnimation = animationName;
        actorSkeletonGraphics.startingLoop = isLoop;
        actorSkeletonGraphics.rectTransform.anchoredPosition = targetPosition;
        actorSkeletonGraphics.rectTransform.localScale = targetScale;
        actorSkeletonGraphics.color = selectedColor;
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
