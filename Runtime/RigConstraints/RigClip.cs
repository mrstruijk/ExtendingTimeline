using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
[Serializable] public class RigClip : PlayableAsset
{
    public ExposedReference<Transform> rigTarget;
    public ExposedReference<Transform> worldTarget;
    public AnimationCurve rigWeight;
    public bool reset = true;

    [Range(0f, 1f)] public float resetToValue = 0f;

    [SerializeField] private bool lookAt;
    [SerializeField] private bool grab;

    private PlayableGraph playableGraph;

    private RigBehaviour template = new();

    private Transform RigTarget => rigTarget.Resolve(playableGraph.GetResolver());

    private Transform WorldTarget => worldTarget.Resolve(playableGraph.GetResolver());

    public TimelineClip TimelineClip { get; set; }

    public RigBehaviour Template => template;


    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) // Here we write our logic for creating the playable behaviour
    {
        var playable = ScriptPlayable<RigBehaviour>.Create(graph, template); // Create a playable, using the constructor

        var behaviour = playable.GetBehaviour(); // Get behaviour

        behaviour.rigClip = this;

        playableGraph = graph;

        SetValuesOnBehaviourFromClip(behaviour);

        SetDisplayName(TimelineClip);

        return playable;
    }


    private void SetValuesOnBehaviourFromClip(RigBehaviour behaviour)
    {
        if (RigTarget == null || WorldTarget == null)
        {
            return;
        }

        behaviour.rigTarget = RigTarget;
        behaviour.rigWeight = rigWeight;
        behaviour.worldTarget = WorldTarget;
        behaviour.reset = reset;
        behaviour.resetToValue = resetToValue;
    }


    /// <summary>
    ///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
    /// </summary>
    private void SetDisplayName(TimelineClip clip)
    {
        if (clip == null)
        {
            return;
        }

        if (WorldTarget == null)
        {
            return;
        }

        var displayName = "";

        if (lookAt)
        {
            displayName += "LookAt: ";
        }

        if (grab)
        {
            displayName += "Grab: ";
        }

        displayName += WorldTarget.name;
        clip.displayName = displayName;
    }
}