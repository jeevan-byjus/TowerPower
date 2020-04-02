using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Byjus.Gamepod.Template.Verticals;
using Byjus.Gamepod.Template.Util;

#if !CC_STANDALONE
using Osmo.SDK;
using Osmo.Container.Common;
using Osmo.SDK.Internal;

namespace Byjus.Gamepod.Template.Externals {

    /// <summary>
    /// The top most parent in game hierarchy in case the setup is for Osmo
    /// </summary>
    public class OsmoExternalParent : OsmoGameBase, IOsmoEditorVisionHelper {
        [SerializeField] TangibleManager mManager;
        [SerializeField] OsmoVisionService osmoVisionServiceView;
        [SerializeField] HierarchyManager hierarchyManager;

        public TangibleManager tangibleManager { get { return mManager; } }

        public Vector2 GetCameraDimens() {
            return new Vector2(TangibleCamera.Width, TangibleCamera.Height);
        }

        void AssignRefs() {
            mManager = FindObjectOfType<TangibleManager>();
            osmoVisionServiceView = FindObjectOfType<OsmoVisionService>();
            hierarchyManager = FindObjectOfType<HierarchyManager>();
        }

        protected override void GameStart() {
            if (Bridge != null) {
                Bridge.Helper.SetOnMainMenuScreen(false);
                Bridge.Helper.OnSettingsButtonClick += OnSettingsButtonClicked;
                Bridge.Helper.SetSettingsButtonVisibility(true);
                Bridge.Helper.SetVisionActive(true);
                Bridge.Helper.SetOsmoWorldStickersAllowed(true);

                AssignRefs();

#if UNITY_EDITOR
                Factory.SetVisionService(new OsmoEditorVisionService(this));
#else
                Factory.SetVisionService(osmoVisionServiceView);
#endif
                hierarchyManager.Setup();

            } else {
                Debug.LogWarning("[VisionTest] You are running without the Osmo bridge. No Osmo services will be loaded. Bridge.Helper will be null");
            }
        }

        void OnSettingsButtonClicked() {
            Debug.LogWarning("Settings Clicked");
        }
    }

    public interface IOsmoEditorVisionHelper {
        TangibleManager tangibleManager { get; }
        Vector2 GetCameraDimens();
    }


    
}
#endif