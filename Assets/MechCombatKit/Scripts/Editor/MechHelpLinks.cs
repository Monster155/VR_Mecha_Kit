using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MechHelpLinks : EditorWindow
{
    [MenuItem("Mech Combat Kit/Help/Tutorial Videos")]
    public static void TutorialVideos()
    {
        Application.OpenURL("https://vimeo.com/user102278338");
    }

    [MenuItem("Mech Combat Kit/Help/Forum")]
    public static void Forum()
    {
        Application.OpenURL("https://forum.unity.com/threads/mech-combat-kit.852241/");
    }
}
