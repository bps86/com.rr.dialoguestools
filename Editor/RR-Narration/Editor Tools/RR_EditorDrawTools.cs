using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public static class RR_EditorDrawTools
{
    public static void ResizeScrollView(ref Rect cursorChangeRect, Rect defaultRect, ref bool resize, ref float currentScrollViewWidth) {
        GUI.DrawTexture(cursorChangeRect, EditorGUIUtility.whiteTexture);
        EditorGUIUtility.AddCursorRect(cursorChangeRect, MouseCursor.ResizeHorizontal);
        cursorChangeRect.height = defaultRect.height;

        if (Event.current.type == EventType.MouseDown && cursorChangeRect.Contains(Event.current.mousePosition))
            resize = true;
        if (Event.current.type == EventType.MouseUp) resize = false;
        if (resize) {
            currentScrollViewWidth = Event.current.mousePosition.x;
            cursorChangeRect.Set(currentScrollViewWidth, cursorChangeRect.y, cursorChangeRect.width, cursorChangeRect.height);
        }
    }
}