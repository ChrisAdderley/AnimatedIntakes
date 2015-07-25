using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace AnimatedIntakes
{
    public class AnimatedIntake: PartModule
    {
        [KSPField(isPersistant = false)]
        public string CloseAnimationName;

        [KSPField(isPersistant = false)]
        public string LoopAnimationName;


        [KSPField(isPersistant = false)]
        public float CloseAnimationSpeedScalar = 1.0f;

        [KSPField(isPersistant = false)]
        public float LoopAnimationSpeedScalar = 1.0f;

        private bool intakeState = true;

        private bool openAnim = false;
        private bool loopAnim = false;

        private AnimationState[] openStates;
        private AnimationState[] loopStates;

        private ModuleResourceIntake intake;

        public override void OnStart(PartModule.StartState state)
        {
            if (CloseAnimationName != "")
            {
                
                openAnim = true;
            }
            if (LoopAnimationName != "")
            {
                loopAnim = true;
            }
            if (HighLogic.LoadedSceneIsFlight)
            {
                intake = gameObject.GetComponent<ModuleResourceIntake>();
                if (intake == null)
                {
                    Utils.LogError("AnimatedIntake requires an intake module!");
                    return;
                }
                if (openAnim)
                {
                    Utils.Log("Setting up close animation");
                    openStates = Utils.SetUpAnimation(CloseAnimationName, this.part);
                }
                if (loopAnim)
                {
                    Utils.Log("Setting up loop animation");
                    loopStates = Utils.SetUpAnimation(LoopAnimationName, this.part);
                }
                
            }
        }

        public override void OnUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight && intake != null)
            {

                if (openAnim)
                    HandleOpenAnimation();
                if (loopAnim)
                    HandleLoopAnimation();
            }
        }

        protected void HandleOpenAnimation()
        {
            if (intake.intakeEnabled != intakeState)
            {
                if (!intake.intakeEnabled)
                {
                    Utils.Log("Closing intake...");
                    foreach (AnimationState a in openStates)
                    {
                        a.enabled = true;
                        a.speed = 1.0f;
                    }
                }
                else
                {
                    Utils.Log("Opening intake...");
                    foreach (AnimationState a in openStates)
                    {
                        a.enabled = true;
                        a.speed = -1.0f;
                    }
                }


                intakeState = intake.intakeEnabled;
            }
        }

        protected void HandleLoopAnimation()
        {
            if (intake.intakeEnabled)
            {
                foreach (AnimationState a in loopStates)
                {
                    
                    a.speed = vessel.ctrlState.mainThrottle;
                    a.wrapMode = WrapMode.Loop;
                } 
            }
        }
    }
}
