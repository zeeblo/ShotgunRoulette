using UnityEngine;
using UnityEngine.InputSystem;

namespace ShotgunRoulette.Players
{
    internal class Controls
    {

        private static InputActionAsset actionAsset = ScriptableObject.CreateInstance<InputActionAsset>();
        public static InputActionMap userControls = actionAsset.AddActionMap("UserControls");
        public static InputActionReference? rouletteRef;

        public static void InitControls()
        {
            if (Plugin.gunRotationBind == null)
            {
                return;
            }

            InputAction gunRotation = userControls.AddAction("roulette", InputActionType.Button, binding: "<Keyboard>/" + Plugin.gunRotationBind.Value);
            gunRotation.performed += GunRotation_performed;
            userControls.Enable();

            rouletteRef = InputActionReference.Create(gunRotation);
        }


        private static void GunRotation_performed(InputAction.CallbackContext context)
        {
            if (GameNetworkManager.Instance.localPlayerController != null)
            {
                Plugin.ToggleGunRotation(GameNetworkManager.Instance.localPlayerController);
            }
            
        }


    }
}
