using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhitethornBuild
{
    public class InteractableBuildEndCaller : Interactable
    {
        [SerializeField] GameObject inputIcon;
        [SerializeField] BuildEndCaller buildEndCaller;

        private void Start() 
        {
            HighlightState(false);
        }

        public override void Interaction(PlayerInteractor interactor) 
        {
            base.Interaction(interactor);

            if (!buildEndCaller)
                return;

            buildEndCaller.SetEndBuildOutro(interactor);
            Debug.Log("Hi icon");
        }

        protected override void HighlightState(bool value) 
        {
            if (inputIcon)
                inputIcon.SetActive(value);
        }
    }
}
