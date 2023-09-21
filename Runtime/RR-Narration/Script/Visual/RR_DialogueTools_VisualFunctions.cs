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
        actorSkeletonGraphics.layoutScaleMode = SkeletonGraphic.LayoutMode.FitInParent;
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

    public static Vector3 GetAdjustedActorPos(Vector3 targetPos, Vector3 targetScale, Vector3 actorPivot) {
        Debug.Log("GetAdjusted Pos");
        Debug.Log("target pos: " + targetPos);
        Debug.Log("target scale: " + targetScale);
        Debug.Log("actor pivot: " + actorPivot);
        return targetPos + (Vector3.Scale(actorPivot, targetScale));
    }

    public static Vector3 GetAdjustedActorScale(Vector3 targetScale, Vector3 actorScale) {
        Debug.Log("GetAdjusted Scale");
        Debug.Log("target scale: " + targetScale);
        Debug.Log("actor scale: " + actorScale);
        return Vector3.Scale(targetScale, actorScale);
    }

    private static void ResetAnim(SkeletonGraphic skeletonGraphic, string defaultAnimation) {
        skeletonGraphic.AnimationState.SetAnimation(0, defaultAnimation, true);
    }
}
