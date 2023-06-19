using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace VSX.UniversalVehicleCombat.Mechs
{
    public class CreateAnimatorController : EditorWindow
    {

        // Movement

        private AnimationClip idleClip;
        private AnimationClip walkForwardClip;
        private AnimationClip walkBackwardClip;
        private AnimationClip runClip;

        // Landing

        private AnimationClip landingClip;
        private AnimationClip landingDamageClip;

        // Airborne

        private AnimationClip airborneClip;

        private bool reverseWalkBackAnimation;

        [MenuItem("Mech Combat Kit/Animator Controller Creator")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            CreateAnimatorController window = (CreateAnimatorController)EditorWindow.GetWindow(typeof(CreateAnimatorController));
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

            reverseWalkBackAnimation = EditorGUILayout.Toggle("Reverse Walk Back Animation", reverseWalkBackAnimation);

            idleClip = EditorGUILayout.ObjectField("Idle", idleClip, typeof(AnimationClip), false) as AnimationClip;
            walkForwardClip = EditorGUILayout.ObjectField("Walk Forward", walkForwardClip, typeof(AnimationClip), false) as AnimationClip;
            walkBackwardClip = EditorGUILayout.ObjectField("Walk Backward", walkBackwardClip, typeof(AnimationClip), false) as AnimationClip;
            runClip = EditorGUILayout.ObjectField("Run Clip", runClip, typeof(AnimationClip), false) as AnimationClip;

            landingClip = EditorGUILayout.ObjectField("Landing Clip", landingClip, typeof(AnimationClip), false) as AnimationClip;
            landingDamageClip = EditorGUILayout.ObjectField("Landing Damage Clip", landingDamageClip, typeof(AnimationClip), false) as AnimationClip;

            airborneClip = EditorGUILayout.ObjectField("Airborne Clip", airborneClip, typeof(AnimationClip), false) as AnimationClip;

            if (GUILayout.Button("Create Animator Controller"))
            {
                CreateMechAnimatorController();
            }
        }



        private void CreateMechAnimatorController()
        {

            AnimatorController animatorController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/AnimatorController.controller");

            AnimatorControllerParameter groundedParameter = new AnimatorControllerParameter();
            groundedParameter.name = "Grounded";
            groundedParameter.type = AnimatorControllerParameterType.Bool;
            animatorController.AddParameter(groundedParameter);

            AnimatorControllerParameter forwardParameter = new AnimatorControllerParameter();
            forwardParameter.name = "Forward";
            forwardParameter.type = AnimatorControllerParameterType.Float;
            animatorController.AddParameter(forwardParameter);

            AnimatorControllerParameter turnParameter = new AnimatorControllerParameter();
            turnParameter.name = "Turn";
            turnParameter.type = AnimatorControllerParameterType.Float;
            animatorController.AddParameter(turnParameter);

            AnimatorControllerParameter jumpParameter = new AnimatorControllerParameter();
            jumpParameter.name = "Jump";
            jumpParameter.type = AnimatorControllerParameterType.Trigger;
            animatorController.AddParameter(jumpParameter);

            AnimatorControllerParameter landedParameter = new AnimatorControllerParameter();
            landedParameter.name = "Landed";
            landedParameter.type = AnimatorControllerParameterType.Trigger;
            animatorController.AddParameter(landedParameter);

            AnimatorControllerParameter landDamageAmountParameter = new AnimatorControllerParameter();
            landDamageAmountParameter.name = "LandDamageAmount";
            landDamageAmountParameter.type = AnimatorControllerParameterType.Float;
            animatorController.AddParameter(landDamageAmountParameter);

            AnimatorStateMachine stateMachine = animatorController.layers[0].stateMachine;

            AnimatorStateMachine locomotionStateMachine = stateMachine.AddStateMachine("Locomotion", new Vector3(350, 100, 0));
            stateMachine.AddEntryTransition(locomotionStateMachine);

            // Grounded state - as it's first, it will have an entry transition by default       
            AnimatorState groundedState = CreateGroundedState(animatorController, locomotionStateMachine);

            // Landing state

            AnimatorState landingState = CreateLandingState(animatorController, locomotionStateMachine);

            // Airborne state

            AnimatorState airborneState = CreateAirborneState(animatorController, locomotionStateMachine);

            // Transitions

            AnimatorStateTransition groundedToAirborneTransition = groundedState.AddTransition(airborneState);
            groundedToAirborneTransition.hasExitTime = false;
            groundedToAirborneTransition.hasFixedDuration = true;
            groundedToAirborneTransition.duration = 0.25f;
            groundedToAirborneTransition.AddCondition(AnimatorConditionMode.IfNot, 0, "Grounded");

            AnimatorStateTransition airborneToLandingTransition = airborneState.AddTransition(landingState);
            airborneToLandingTransition.hasExitTime = false;
            airborneToLandingTransition.hasFixedDuration = true;
            airborneToLandingTransition.duration = 0.1f;
            airborneToLandingTransition.AddCondition(AnimatorConditionMode.If, 0, "Grounded");

            AnimatorStateTransition landingToGroundedTransition = landingState.AddTransition(groundedState);
            landingToGroundedTransition.hasExitTime = true;
            landingToGroundedTransition.exitTime = 0.5f;
            landingToGroundedTransition.hasFixedDuration = true;
            landingToGroundedTransition.duration = 0.25f;

            AssetDatabase.SaveAssets();

        }

        protected AnimatorState CreateGroundedState(AnimatorController controller, AnimatorStateMachine stateMachine)
        {
            // Grounded
            AnimatorState groundedState = stateMachine.AddState("Grounded", new Vector3(300, 0, 0));

            BlendTree groundedMotionBlendTree = new BlendTree();
            groundedMotionBlendTree.name = "Movement";
            groundedMotionBlendTree.useAutomaticThresholds = false;
            groundedMotionBlendTree.blendParameter = "Forward";

            ChildMotion[] childMotions = new ChildMotion[4];

            childMotions[0].motion = walkBackwardClip;
            childMotions[0].timeScale = reverseWalkBackAnimation ? -1 : 1;
            childMotions[0].threshold = -0.5f;

            childMotions[1].motion = idleClip;
            childMotions[1].timeScale = 1;
            childMotions[1].threshold = 0f;

            childMotions[2].motion = walkForwardClip;
            childMotions[2].timeScale = 1;
            childMotions[2].threshold = 0.5f;

            childMotions[3].motion = runClip;
            childMotions[3].timeScale = 1;
            childMotions[3].threshold = 1f;


            groundedMotionBlendTree.children = childMotions;

            AssetDatabase.AddObjectToAsset(groundedMotionBlendTree, controller);
            groundedMotionBlendTree.hideFlags = HideFlags.HideInHierarchy;

            groundedState.motion = groundedMotionBlendTree;

            return groundedState;
        }

        protected AnimatorState CreateLandingState(AnimatorController controller, AnimatorStateMachine stateMachine)
        {
            AnimatorState landingState = stateMachine.AddState("Landing", new Vector3(450, 100, 0));

            BlendTree landingBlendTree = new BlendTree();
            landingBlendTree.name = "Landing";
            landingBlendTree.useAutomaticThresholds = false;
            landingBlendTree.blendParameter = "LandDamageAmount";

            ChildMotion[] childMotions = new ChildMotion[2];

            childMotions[0].motion = landingClip;
            childMotions[0].timeScale = 1f;
            childMotions[0].threshold = 0f;

            childMotions[1].motion = landingDamageClip;
            childMotions[1].timeScale = 1f;
            childMotions[1].threshold = 1f;

            landingBlendTree.children = childMotions;

            AssetDatabase.AddObjectToAsset(landingBlendTree, controller);
            landingBlendTree.hideFlags = HideFlags.HideInHierarchy;

            landingState.motion = landingBlendTree;

            return landingState;
        }

        protected AnimatorState CreateAirborneState(AnimatorController controller, AnimatorStateMachine stateMachine)
        {
            AnimatorState airborneState = stateMachine.AddState("Airborne", new Vector3(300, 200, 0));

            airborneState.motion = airborneClip;

            return airborneState;
        }
    }
}

